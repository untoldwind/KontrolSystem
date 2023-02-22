using FinePrint;
using KontrolSystem.TO2.Binding;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public partial class KSPOrbitModule {
        [KSClass("Waypoint")]
        public class WaypointAdapter {
            private readonly Waypoint waypoint;
            private IBody cachedBody;

            internal WaypointAdapter(Waypoint waypoint) => this.waypoint = waypoint;

            [KSField]
            public string Name => waypoint.name;

            [KSField]
            public IBody Body => cachedBody ??=
                KSPContext.CurrentContext.Bodies.FirstOrDefault(body => body.Name == waypoint.celestialName);

            [KSField]
            public GeoCoordinates Coordinates => new GeoCoordinates(Body, waypoint.latitude, waypoint.longitude);

            [KSField]
            public Vector3d Position => Body.SurfacePosition(waypoint.latitude, waypoint.longitude, Altitude);

            [KSField]
            public double Altitude => Body.TerrainHeight(waypoint.latitude, waypoint.longitude) + waypoint.altitude;

            [KSField]
            public bool IsGrounded => waypoint.landLocked;

            [KSField]
            public bool IsOnSurface => waypoint.isOnSurface;

            [KSField]
            public bool IsClustered => waypoint.isClustered;
        }
    }
}
