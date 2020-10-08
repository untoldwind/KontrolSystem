using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public partial class KSPOrbitModule {
        [KSClass]
        public class GeoCoordinates {
            private readonly IBody body;

            [KSField] public IBody Body => body;

            [KSField] public double Latitude { get; set; }

            [KSField] public double Longitude { get; set; }

            public GeoCoordinates(IBody body, double latitude, double longitude) {
                this.body = body;
                Latitude = latitude;
                Longitude = longitude;
            }

            [KSField] public Vector3d SurfaceNormal => body.SurfaceNormal(Latitude, Longitude);

            [KSField] public double SurfaceAltitude => body.SurfaceAltitude(Latitude, Longitude);

            [KSField] public double TerrainAltitude => body.TerrainAltitude(Latitude, Longitude);
        }
    }
}
