using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public partial class KSPOrbitModule {
        [KSClass]
        public class GeoCoordinates {
            private readonly IBody body;

            [KSField] public IBody Body => body;

            [KSField(IncludeSetter = true)] public double Latitude { get; set; }

            [KSField(IncludeSetter = true)] public double Longitude { get; set; }

            public GeoCoordinates(IBody body, double latitude, double longitude) {
                this.body = body;
                Latitude = latitude;
                Longitude = longitude;
            }

            [KSField] public Vector3d SurfaceNormal => body.GetSurfaceNormal(Latitude, Longitude);

            [KSField] public double SurfaceAltitude => body.GetSurfaceAltitude(Latitude, Longitude);

            [KSField] public double TerrainHeight => body.GetTerrainHeight(Latitude, Longitude);
        }
    }
}
