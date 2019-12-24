using System;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public class BodyWrapper : KSPOrbitModule.IBody {
        public readonly CelestialBody body;

        public BodyWrapper(CelestialBody _body) => body = _body;

        public string Name => body.name;

        public double GravParameter => body.gravParameter;

        public double SOIRadius => body.sphereOfInfluence;

        public bool HasAtmosphere => body.atmosphere;

        public double AtmosphereDepth => body.atmosphereDepth;

        public KSPOrbitModule.IOrbit Orbit => new OrbitWrapper(body.orbit);

        public double Radius => body.Radius;

        public Vector3d Position => body.position - (FlightGlobals.ActiveVessel?.CoMD ?? Vector3d.zero);

        public Vector3d Up => body.transform.up;

        public Vector3d GetSurfaceNormal(double lat, double lon) => body.GetSurfaceNVector(lat, lon);

        public double GetSurfaceHeight(double lat, double lon) =>
            body.pqsController?.GetSurfaceHeight(QuaternionD.AngleAxis(lon, Vector3d.down) * QuaternionD.AngleAxis(lat, Vector3d.forward) * Vector3d.right) ?? body.Radius;

        public double GetLatitude(Vector3d position) => DirectBindingMath.Clamp_Degrees_180(body.GetLatitude(position));

        public double GetLongitude(Vector3d position) => DirectBindingMath.Clamp_Degrees_180(body.GetLongitude(position));

        public KSPOrbitModule.GeoCoordinates GetGeoCoordinates(double latitude, double longitude) => new KSPOrbitModule.GeoCoordinates(this, latitude, longitude);
        
        public KSPOrbitModule.IOrbit CreateOrbit(Vector3d relPos, Vector3d vel, double UT) {
            Orbit ret = new Orbit();

            ret.UpdateFromStateVectors(relPos.SwapYZ(), vel.SwapYZ(), body, UT);
            if (double.IsNaN(ret.argumentOfPeriapsis)) {
                Vector3d vectorToAN = Quaternion.AngleAxis(-(float)ret.LAN, Planetarium.up) * Planetarium.right;
                Vector3d vectorToPe = ret.eccVec.SwapYZ();
                double cosArgumentOfPeriapsis = Vector3d.Dot(vectorToAN, vectorToPe) / (vectorToAN.magnitude * vectorToPe.magnitude);
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
    }

}
