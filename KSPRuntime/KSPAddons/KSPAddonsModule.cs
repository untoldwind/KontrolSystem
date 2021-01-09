using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPAddons {
    [KSModule("ksp::addons",
        Description = "Collection of types and functions to get information and control in-game vessels."
    )]
    public partial class KSPAddonsModule {
        [KSConstant("ALARM_CLOCK", Description = "Wrapper to KerbalAlarmClock API")]
        public static readonly AlarmClockAPIWrapper AlarmClockInstance = new AlarmClockAPIWrapper();
    }
}
