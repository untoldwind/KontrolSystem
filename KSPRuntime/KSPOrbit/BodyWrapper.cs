using System;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public class BodyWrapper : KSPOrbitModule.IBody, KSPVesselModule.IKSPTargetable {
        private const int TerrainMaskBIT = 15;

        private readonly CelestialBody body;

        public BodyWrapper(CelestialBody body) => this.body = body;

        public string Name => body.name;

        public double GravParameter => body.gravParameter;

        public double SoiRadius => body.sphereOfInfluence;

        public bool HasAtmosphere => body.atmosphere;

        public double AtmosphereDepth => body.atmosphereDepth;

        public KSPOrbitModule.IOrbit Orbit => new OrbitWrapper(body.orbit);

        public double Radius => body.Radius;

        public double RotationPeriod => body.rotationPeriod;

        // ReSharper disable once Unity.NoNullPropagation
        public Vector3d Position => body.position - (FlightGlobals.ActiveVessel?.CoMD ?? Vector3d.zero);

        public Vector3d Up => body.transform.up;

        public Vector3d Right => body.transform.right;

        public Vector3d AngularVelocity => body.angularVelocity;
        
        public Vector3d SurfaceNormal(double lat, double lon) => body.GetSurfaceNVector(lat, lon);
        
        public double TerrainHeight(double lat, double lon) {
            double alt = 0.0;
            PQS bodyPqs = body.pqsController;
            if (bodyPqs != null) {
                // The sun has no terrain.  Everything else has a PQScontroller.
                // The PQS controller gives the theoretical ideal smooth surface curve terrain.
                // The actual ground that exists in-game that you land on, however, is the terrain
                // polygon mesh which is built dynamically from the PQS controller's altitude values,
                // and it only approximates the PQS controller.  The discrepancy between the two
                // can be as high as 20 meters on relatively mild rolling terrain and is probably worse
                // in mountainous terrain with steeper slopes.  It also varies with the user terrain detail
                // graphics setting.

                // Therefore the algorithm here is this:  Get the PQS ideal terrain altitude first.
                // Then try using RayCast to get the actual terrain altitude, which will only work
                // if the LAT/LONG is near the active vessel so the relevant terrain polygons are
                // loaded.  If the RayCast hit works, it overrides the PQS altitude.

                // PQS controller ideal altitude value:
                // -------------------------------------

                // The vector the pqs GetSurfaceHeight method expects is a vector in the following
                // reference frame:
                //     Origin = body center.
                //     X axis = LATLNG(0,0), Y axis = LATLNG(90,0)(north pole), Z axis = LATLNG(0,-90).
                // Using that reference frame, you tell GetSurfaceHeight what the "up" vector is pointing through
                // the spot on the surface you're querying for.
                var bodyUpVector = new Vector3d(1, 0, 0);
                bodyUpVector = QuaternionD.AngleAxis(lat, Vector3d.forward /*around Z axis*/) * bodyUpVector;
                bodyUpVector = QuaternionD.AngleAxis(lon, Vector3d.down /*around -Y axis*/) * bodyUpVector;

                alt = bodyPqs.GetSurfaceHeight(bodyUpVector) - bodyPqs.radius;

                // Terrain polygon raycasting:
                // ---------------------------
                const double highAgl = 1000.0;
                const double pointAgl = 800.0;
                // a point hopefully above the terrain:
                Vector3d worldRayCastStart = body.GetWorldSurfacePosition(lat, lon, alt + highAgl);
                // a point a bit below it, to aim down to the terrain:
                Vector3d worldRayCastStop = body.GetWorldSurfacePosition(lat, lon, alt + pointAgl);
                RaycastHit hit;
                if (Physics.Raycast(worldRayCastStart, (worldRayCastStop - worldRayCastStart), out hit, float.MaxValue,
                    1 << TerrainMaskBIT)) {
                    // Ensure hit is on the topside of planet, near the worldRayCastStart, not on the far side.
                    if (Mathf.Abs(hit.distance) < 3000) {
                        // Okay a hit was found, use it instead of PQS alt:
                        alt = alt + highAgl - hit.distance;
                    }
                }
            }

            return alt;
        }

        public double Latitude(Vector3d position) => DirectBindingMath.ClampDegrees180(body.GetLatitude(position));

        public double Longitude(Vector3d position) =>
            DirectBindingMath.ClampDegrees180(body.GetLongitude(position));

        public KSPOrbitModule.GeoCoordinates GeoCoordinates(double latitude, double longitude) =>
            new KSPOrbitModule.GeoCoordinates(this, latitude, longitude);

        public Vector3d SurfacePosition(double latitude, double longitude, double altitude) =>
            // ReSharper disable once Unity.NoNullPropagation
            body.GetWorldSurfacePosition(latitude, longitude, altitude) -
            (FlightGlobals.ActiveVessel?.CoMD ?? Vector3d.zero);

        public Vector3d RelativeVelocity(Vector3d position) => body.getRFrmVel(position);
        
        public double AltitudeOf(Vector3d position) {
            // ReSharper disable once Unity.NoNullPropagation
            Vector3d unityWorldPosition = (FlightGlobals.ActiveVessel?.CoMD ?? Vector3d.zero) + position;
            return body.GetAltitude(unityWorldPosition);
        }

        public double RealMaxAtmosphereAltitude => !body.atmosphere ? 0 : body.atmosphereDepth;

        public KSPOrbitModule.IOrbit CreateOrbit(Vector3d relPos, Vector3d vel, double ut) {
            Orbit ret = new Orbit();

            ret.UpdateFromStateVectors(relPos.SwapYZ(), vel.SwapYZ(), body, ut);
            if (double.IsNaN(ret.argumentOfPeriapsis)) {
                Vector3d vectorToAn = Quaternion.AngleAxis(-(float) ret.LAN, Planetarium.up) * Planetarium.right;
                Vector3d vectorToPe = ret.eccVec.SwapYZ();
                double cosArgumentOfPeriapsis =
                    Vector3d.Dot(vectorToAn, vectorToPe) / (vectorToAn.magnitude * vectorToPe.magnitude);
                //Squad's UpdateFromStateVectors is missing these checks, which are needed due to finite precision arithmetic:
                if (cosArgumentOfPeriapsis > 1) {
                    ret.argumentOfPeriapsis = 0;
                } else if (cosArgumentOfPeriapsis < -1) {
                    ret.argumentOfPeriapsis = 180;
                } else {
                    ret.argumentOfPeriapsis = Math.Acos(cosArgumentOfPeriapsis);
                }
            }

            return new OrbitWrapper(ret);
        }

        public KSPOrbitModule.IOrbit CreateOrbitFromParameters(double inclination, double eccentricity, double semiMajorAxis, double lan,
            double argumentOfPeriapsis, double meanAnomalyAtEpoch, double epoch) => new OrbitWrapper(new Orbit(inclination, eccentricity, semiMajorAxis, lan, argumentOfPeriapsis,
                meanAnomalyAtEpoch, epoch, body));

        public Option<KSPOrbitModule.IBody> AsBody => new Option<KSPOrbitModule.IBody>(this);

        public Option<KSPVesselModule.VesselAdapter> AsVessel => new Option<KSPVesselModule.VesselAdapter>();

        public Option<KSPVesselModule.ModuleDockingNodeAdapter> AsDockingPort =>
            new Option<KSPVesselModule.ModuleDockingNodeAdapter>();

        public ITargetable Underlying => body;
    }
}
