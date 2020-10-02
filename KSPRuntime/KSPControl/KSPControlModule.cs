using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    [KSModule("ksp::control")]
    public partial class KSPControlModule {
        [KSFunction(
            Description = "Create a new PIDLoop with given parameters."
        )]
        public static PIDLoop PidLoop(double kp, double ki, double kd, double minOutput, double maxOutput) =>
            new PIDLoop(kp, ki, kd, minOutput, maxOutput);
    }
}
