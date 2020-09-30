using System;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public partial class KSPOrbitModule {
        [KSClass]
        public class GeoCoordinates {
            private readonly IBody body;

            [KSField] public IBody Body => body;

            [KSField(IncludeSetter = true)] public double Latitude { get; set; }

            [KSField(IncludeSetter = true)] public double Longitude { get; set; }

            public GeoCoordinates(IBody _body, double latitude, double longitude) {
                body = _body;
                Latitude = latitude;
                Longitude = longitude;
            }

            [KSField] public Vector3d SurfaceNormal => body.GetSurfaceNormal(Latitude, Longitude);

            [KSField] public double SurfaceHeight => body.GetSurfaceHeight(Latitude, Longitude);
        }
    }
}
