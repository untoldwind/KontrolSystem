using System;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    public partial class KSPControlModule {
        [KSClass("ThrottleManager")]
        public class ThrottleManager {
            private IKSPContext context;
            private Vessel vessel;
            private Func<double> throttleProvider;


            public ThrottleManager(IKSPContext context, Vessel vessel, Func<double> throttleProvider) {
                this.context = context;
                this.vessel = vessel;
                this.throttleProvider = throttleProvider;

                this.context.HookAutopilot(this.vessel, UpdateAutopilot);
            }

            [KSField] public double CurrentThrottle => throttleProvider();

            [KSMethod]
            public void SetThrottle(double throttle) => throttleProvider = () => throttle;

            [KSMethod]
            public void SetThrottleProvider(Func<double> throttleProvider) => this.throttleProvider = throttleProvider;

            [KSMethod]
            public void Release() => context.UnhookAutopilot(vessel, UpdateAutopilot);

            public void UpdateAutopilot(FlightCtrlState c) {
                c.mainThrottle = (float) throttleProvider();
            }
        }
    }
}
