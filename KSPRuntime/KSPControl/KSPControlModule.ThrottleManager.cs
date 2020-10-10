using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    public partial class KSPControlModule {
        [KSClass("ThrottleManager")]
        public class ThrottleManager {
            private readonly IKSPContext context;
            private readonly Vessel vessel;
            private Func<double> throttleProvider;

            public ThrottleManager(IKSPContext context, Vessel vessel, Func<double> throttleProvider) {
                this.context = context;
                this.vessel = vessel;
                this.throttleProvider = throttleProvider;

                this.context.HookAutopilot(this.vessel, UpdateAutopilot);
            }

            [KSField]
            public double Throttle {
                get => throttleProvider();
                set => throttleProvider = () => value;
            }

            [KSMethod]
            public void SetThrottleProvider(Func<double> newThrottleProvider) => throttleProvider = newThrottleProvider;

            [KSMethod]
            public void Release() => context.UnhookAutopilot(vessel, UpdateAutopilot);

            public void UpdateAutopilot(FlightCtrlState c) {
                c.mainThrottle = (float) DirectBindingMath.Clamp(throttleProvider(), 0, 1);
            }
        }
    }
}
