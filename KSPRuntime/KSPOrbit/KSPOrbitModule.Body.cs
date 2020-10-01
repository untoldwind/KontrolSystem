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
            double SOIRadius { get; }

            [KSField(Description = "The orbit of the celestial body itself (around the parent body)")]
            IOrbit Orbit { get; }

            [KSField(Description = "`true` if the celestial body has an atmosphere to deal with.")]
            bool HasAtmosphere { get; }

            [KSField(Description = "Depth/height of the atmosphere if present.")]
            double AtmosphereDepth { get; }

            [KSField(Description = "Radius of the body at sea level")]
            double Radius { get; }

            [KSField] Vector3d Position { get; }

            [KSField] Vector3d Up { get; }

            [KSMethod]
            Vector3d GetSurfaceNormal(double lat, double lon);

            [KSMethod]
            double GetSurfaceHeight(double lat, double lon);

            [KSFunction]
            double GetLatitude(Vector3d position);

            [KSFunction]
            double GetLongitude(Vector3d position);

            [KSFunction]
            GeoCoordinates GetGeoCoordinates(double latitude, double longitude);

            [KSMethod(Description =
                "Create a new orbit around this body starting at a given relative `position` and `velocity` at universal time `UT`")]
            IOrbit CreateOrbit(Vector3d position, Vector3d velocity, double UT);
        }
    }
}
