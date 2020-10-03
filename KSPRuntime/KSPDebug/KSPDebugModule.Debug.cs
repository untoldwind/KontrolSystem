﻿using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPDebug {
    public partial class KSPDebugModule {
        [KSField(IncludeSetter = true)]
        public bool Enabled { get; set; }
        
        [KSClass("Debug", Description = "Collection of debug helper")]
        public class Debug {
            [KSFunction(Description =
                @"Draws a line from `start` to `end` with a specified `color` and `width` in the current game scene.
              The line may have a `label` at its mid-point.

              The result of the function is a `DebugVector` that can be modified or `None` if the current game scene does not support debugging vectors.
             "
            )]
            public static Option<VectorRenderer> AddLine(Vector3d start, Vector3d end,
                KSPConsoleModule.RgbaColor color,
                string label, double width) {
                Vessel vessel = FlightGlobals.ActiveVessel;

                if (vessel == null) return Option.None<VectorRenderer>();

                VectorRenderer renderer = new VectorRenderer(vessel, start, end - start, color, label, width, false);

                renderer.Visible = true;
                KSPContext.CurrentContext.AddMarker(renderer);

                return Option.Some(renderer);
            }

            [KSFunction(Description =
                @"Draws a `vector` positioned at `start` with a specified `color` and `width` in the current game scene.
              The vector may have a `label` at its mid-point.

              The result of the function is a `DebugVector` that can be modified or `None` if the current game scene does not support debugging vectors.
             "
            )]
            public static Option<VectorRenderer> AddVector(Vector3d start, Vector3d vector,
                KSPConsoleModule.RgbaColor color, string label, double width) {
                Vessel vessel = FlightGlobals.ActiveVessel;

                if (vessel == null) return Option.None<VectorRenderer>();

                VectorRenderer renderer = new VectorRenderer(vessel, start, vector, color, label, width, true);

                renderer.Visible = true;
                KSPContext.CurrentContext.AddMarker(renderer);

                return Option.Some(renderer);
            }

            [KSFunction]
            public static Option<GroundMarker> AddGroundMarker(KSPOrbitModule.GeoCoordinates geoCoordinates,
                KSPConsoleModule.RgbaColor color, double rotation) {
                GroundMarker groundMarker = new GroundMarker(geoCoordinates, color, rotation);

                KSPContext.CurrentContext.AddMarker(groundMarker);

                return Option.Some(groundMarker);
            }

            [KSFunction(
                Description = "Remove all markers from the game-scene."
            )]
            public static void ClearMarkers() {
                KSPContext.CurrentContext.ClearMarkers();
            }
        }
    }
}