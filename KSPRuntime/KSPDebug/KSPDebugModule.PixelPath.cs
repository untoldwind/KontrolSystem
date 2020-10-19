using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    public partial class KSPDebugModule {
        [KSClass("PixelPath",
            Description = "Represents a pixel path."
        )]
        public class PixelPath : IMarker {
            [KSField(Description = "Controls if the ground marker is currently visible (initially `true`)")]
            public bool Visible { get; set; }

            [KSField] public Vector3d[] Path { get; set; }

            [KSField] public bool Dashed { get; set; }

            [KSField(Description = "The color of the ground marker vector")]
            public KSPConsoleModule.RgbaColor Color { get; set; }

            public PixelPath(Vector3d[] path, KSPConsoleModule.RgbaColor color, bool dashed) {
                Color = color;
                Path = path;
                Dashed = dashed;
                Visible = true;
            }

            public void Update() {
            }

            public void OnRender() {
                bool map = MapView.MapIsEnabled;
                Vessel vessel = FlightGlobals.ActiveVessel;

                if (vessel == null) return;

                GLUtils.DrawPath(Path, vessel.mainBody.position, vessel.mainBody.Radius, Color.Color, GLUtils.Colored,
                    Dashed, map);
            }
        }
    }
}
