using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.KSP.Runtime.KSPDebug;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    public partial class KSPControlModule {
        [KSClass("ThrottleManager")]
        public class ThrottleManager {
            private IKSPContext context;
            private Vessel vessel;
            private Func<double> throttleProvider;


            public ThrottleManager(IKSPContext _context, Vessel _vessel, Func<double> _throttleProvider) {
                context = _context;
                vessel = _vessel;
                throttleProvider = _throttleProvider;

                context.HookAutopilot(vessel, UpdateAutopilot);
            }

            [KSField] public double CurrentThrottle => throttleProvider();

            [KSMethod]
            public void SetThrottle(double throttle) => throttleProvider = () => throttle;

            [KSMethod]
            public void SetThrottleProvider(Func<double> _throttleProvider) => throttleProvider = _throttleProvider;

            [KSMethod]
            public void Release() => context.UnhookAutopilot(vessel, UpdateAutopilot);

            public void UpdateAutopilot(FlightCtrlState c) {
                c.mainThrottle = (float) throttleProvider();
            }
        }
    }
}
