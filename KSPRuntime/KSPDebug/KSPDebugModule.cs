using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    [KSModule("ksp::debug",
        Description = "Provides utility functions to draw in-game markers that can be helpful to visualize why an algorithm went haywire."
    )]
    public partial class KSPDebugModule {
        IKSPContext context;

        public KSPDebugModule(IContext _context, Dictionary<string, object> modules) => context = _context as IKSPContext;

        [KSFunction(Description =
            @"Draws a line from `start` to `end` with a specified `color` and `width` in the current game scene.
              The line may have a `label` at its mid-point.

              The result of the function is a `DebugVector` that can be modified or `None` if the current game scene does not support debugging vectors.
             "
        )]
        public Option<VectorRenderer> AddLine(Vector3d start, Vector3d end, KSPConsoleModule.RgbaColor color, string label, double width) {
            Vessel vessel = FlightGlobals.ActiveVessel;

            if (vessel == null) return new Option<VectorRenderer>();

            VectorRenderer renderer = new VectorRenderer(vessel, start, end - start, color, label, width, false);

            renderer.Visible = true;
            context?.AddMarker(renderer);

            return new Option<VectorRenderer>(renderer);
        }

        [KSFunction(Description =
            @"Draws a `vector` positioned at `start` with a specified `color` and `width` in the current game scene.
              The vector may have a `label` at its mid-point.

              The result of the function is a `DebugVector` that can be modified or `None` if the current game scene does not support debugging vectors.
             "
        )]
        public Option<VectorRenderer> AddVector(Vector3d start, Vector3d vector, KSPConsoleModule.RgbaColor color, string label, double width) {
            Vessel vessel = FlightGlobals.ActiveVessel;

            if (vessel == null) return new Option<VectorRenderer>();

            VectorRenderer renderer = new VectorRenderer(vessel, start, vector, color, label, width, true);

            renderer.Visible = true;
            context?.AddMarker(renderer);

            return new Option<VectorRenderer>(renderer);
        }

        [KSFunction]
        public GroundMarker AddGroundMarker(KSPOrbitModule.GeoCoordinates geoCoordinates, KSPConsoleModule.RgbaColor color, double rotation) {
            GroundMarker groundMarker = new GroundMarker(geoCoordinates, color, rotation);

            context?.AddMarker(groundMarker);

            return groundMarker;
        }

        [KSFunction(
            Description = "Remove all markers from the game-scene."
        )]
        public void ClearMarkers() {
            context?.ClearMarkers();
        }
    }
}
