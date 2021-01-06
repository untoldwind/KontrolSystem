using System;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    public partial class KSPControlModule {
        [KSClass("WheelSteeringManager")]
        public class WheelSteeringManager {
            private readonly IKSPContext context;
            private readonly KSPVesselModule.VesselAdapter vessel;
            private Func<double> bearingProvider;

            public WheelSteeringManager(IKSPContext context, KSPVesselModule.VesselAdapter vessel,
                Func<double> bearingProvider) {
                this.context = context;
                this.vessel = vessel;
                this.bearingProvider = bearingProvider;

                this.context.HookAutopilot(this.vessel.vessel, UpdateAutopilot);
            }

            [KSField]
            public double Bearing {
                get => bearingProvider();
                set => bearingProvider = () => value;
            }

            [KSMethod]
            public void SetBearingProvider(Func<double> newBearingProvider) => bearingProvider = newBearingProvider;

            [KSMethod]
            public void Release() => context.UnhookAutopilot(vessel.vessel, UpdateAutopilot);

            [KSMethod]
            public void Resume() => context.HookAutopilot(vessel.vessel, UpdateAutopilot);
            
            public void UpdateAutopilot(FlightCtrlState c) {
                if (!(vessel.vessel.horizontalSrfSpeed > 0.1f)) return;

                if (Math.Abs(ExtraMath.AngleDelta(vessel.Heading, vessel.VelocityHeading)) <= 90) {
                    c.wheelSteer = (float) DirectBindingMath.Clamp(bearingProvider() / -10, -1, 1);
                } else {
                    c.wheelSteer = -(float) DirectBindingMath.Clamp(bearingProvider() / -10, -1, 1);
                }
            }
        }
    }
}
