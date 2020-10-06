using System;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPGame {
    [KSModule("ksp::game::warp",
        Description = "Collection of functions to control time warp."
    )]
    public class KSPGameWarpModule {
        [KSConstant("RAILS",
            Description = "Value of `current_warp_mode` if warp is on rails."
        )]
        public static readonly string WARP_RAILS = "RAILS";

        [KSConstant("PHYSICS",
            Description = "Value of `current_warp_mode` if in physics warp."
        )]
        public static readonly string WARP_PHYSICS = "PHYSICS";

        [KSFunction(Description = "Get the current warp mode (RAILS/PHYSICS).")]
        public static string CurrentMode() {
            switch (TimeWarp.WarpMode) {
            case TimeWarp.Modes.LOW:
                return WARP_PHYSICS;
            case TimeWarp.Modes.HIGH:
                return WARP_RAILS;
            default:
                return "UNDEFINED"; // This should never happen unless SQUAD adds more to the Modes enum.
            }
        }

        [KSFunction(Description = "Set the warp mode (RAILS/PHYSICS).")]
        public static void SetMode(string warpMode) {
            switch (warpMode.ToUpperInvariant()) {
            case "PHYSICS":
                TimeWarp.fetch.Mode = TimeWarp.Modes.LOW;
                break;
            case "RAILS":
                TimeWarp.fetch.Mode = TimeWarp.Modes.HIGH;
                break;
            }
        }

        [KSFunction(Description = "Get the current warp index. Actual factor depends on warp mode.")]
        public static long CurrentIndex() => TimeWarp.CurrentRateIndex;

        [KSFunction(Description = "Set warp index. Actual factor depends on warp mode.")]
        public static void SetIndex(long warpIndex) {
            switch (TimeWarp.WarpMode) {
            case TimeWarp.Modes.HIGH:
                SetWarpRate(warpIndex, TimeWarp.fetch.warpRates.Length - 1);
                break;
            case TimeWarp.Modes.LOW:
                SetWarpRate(warpIndex, TimeWarp.fetch.physicsWarpRates.Length - 1);
                break;
            }
        }

        [KSFunction(Description = "Get the current warp rate (i.e. actual time multiplier).")]
        public static double CurrentRate() => TimeWarp.CurrentRate;

        [KSFunction(Description = "Warp forward to a specific universal time.")]
        public static void WarpTo(double UT) => TimeWarp.fetch.WarpTo(UT);

        [KSFunction(Description = "Check if time warp has settled down")]
        public static bool IsSettled() {
            float expectedRate = GetRateArrayForMode(TimeWarp.WarpMode)[TimeWarp.CurrentRateIndex];

            // The comparison has to have a large-ish epsilon because sometimes the
            // current rate will eventually settle steady on something like 99.9813 or so
            // for 100x. (far bigger than one would expect for a normal floating point
            // equality epsilon):
            return Math.Abs(expectedRate - TimeWarp.CurrentRate) < 0.05;
        }

        [KSFunction(Description = "Cancel time warp")]
        public static void Cancel() {
            TimeWarp.fetch.CancelAutoWarp();
            SetIndex(0);
        }

        // Return which of the two rates arrays is to be used for the given mode:
        private static float[] GetRateArrayForMode(TimeWarp.Modes whichMode) {
            switch (whichMode) {
            case TimeWarp.Modes.HIGH:
                return TimeWarp.fetch.warpRates;
            case TimeWarp.Modes.LOW:
                return TimeWarp.fetch.physicsWarpRates;
            }

            return new float[0];
        }

        private static void SetWarpRate(long newRate, int maxRate) {
            int clampedValue = (int) Math.Max(Math.Min(newRate, maxRate), 0);
            TimeWarp.SetRate(clampedValue, false);
        }
    }
}
