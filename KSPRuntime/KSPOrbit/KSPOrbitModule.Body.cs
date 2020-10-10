using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public partial class KSPOrbitModule {
        [KSClass("Body",
            Description = "Represents an in-game celestial body."
        )]
        public interface IBody {
            [KSField(Description = "Name of the celestial body.")]
            string Name { get; }

            [KSField(Description = "Standard gravitation parameter of the body.")]
            double GravParameter { get; }

            [KSField("SOI_radius", Description = "Radius of the sphere of influence of the body")]
            double SoiRadius { get; }

            [KSField(Description = "The orbit of the celestial body itself (around the parent body)")]
            IOrbit Orbit { get; }

            [KSField(Description = "`true` if the celestial body has an atmosphere to deal with.")]
            bool HasAtmosphere { get; }

            [KSField(Description = "Depth/height of the atmosphere if present.")]
            double AtmosphereDepth { get; }

            [KSField(Description = "Radius of the body at sea level")]
            double Radius { get; }

            [KSField(Description = "Rotation period of the planet.")]
            double RotationPeriod { get; }
            
            [KSField] Vector3d Position { get; }

            [KSField] Vector3d Up { get; }

            [KSMethod]
            Vector3d SurfaceNormal(double lat, double lon);

            [KSMethod]
            double SurfaceAltitude(double lat, double lon);

            [KSMethod]
            double TerrainAltitude(double lat, double lon);

            [KSMethod]
            double Latitude(Vector3d position);

            [KSMethod]
            double Longitude(Vector3d position);

            [KSMethod]
            GeoCoordinates GeoCoordinates(double latitude, double longitude);

            [KSMethod]
            Vector3d SurfacePosition(double latitude, double longitude, double altitude);
            
            [KSMethod]
            double AltitudeOf(Vector3d position);

            [KSMethod(Description =
                "Create a new orbit around this body starting at a given relative `position` and `velocity` at universal time `UT`")]
            IOrbit CreateOrbit(Vector3d position, Vector3d velocity, double ut);
        }
    }
}
