using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    public partial class KSPControlModule {
        // For the most part this is a rip-off from KOS
        [KSClass("TorquePI")]
        public class TorquePI {
            [KSField] public PIDLoop Loop { get; set; }

            [KSField] public double I { get; private set; }

            [KSField(IncludeSetter = true)] public MovingAverage TorqueAdjust { get; set; }

            private double tr;

            [KSField]
            public double Tr {
                get { return tr; }
                set {
                    tr = value;
                    ts = 4.0 * tr / 2.76;
                }
            }

            private double ts;

            [KSField(IncludeSetter = true)]
            public double Ts {
                get { return ts; }
                set {
                    ts = value;
                    tr = 2.76 * ts / 4.0;
                }
            }

            public TorquePI() {
                Loop = new PIDLoop();
                Ts = 2;
                TorqueAdjust = new MovingAverage();
            }

            [KSMethod]
            public double Update(double sampleTime, double input, double setpoint, double momentOfInertia,
                double maxOutput) {
                I = momentOfInertia;

                Loop.Ki = momentOfInertia * Math.Pow(4.0 / ts, 2);
                Loop.Kp = 2 * Math.Pow(momentOfInertia * Loop.Ki, 0.5);
                return Loop.Update(sampleTime, input, setpoint, maxOutput);
            }

            [KSMethod]
            public void ResetI() {
                Loop.ResetI();
            }

            public override string ToString() {
                return string.Format("TorquePI[Kp:{0}, Ki:{1}, Output:{2}, Error:{3}, ErrorSum:{4}, Tr:{5}, Ts:{6}",
                    Loop.Kp, Loop.Ki, Loop.Output, Loop.Error, Loop.ErrorSum, Tr, Ts);
            }
        }
    }
}
