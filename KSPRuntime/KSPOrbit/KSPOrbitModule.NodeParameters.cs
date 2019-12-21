using System;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public partial class KSPOrbitModule {
        [KSClass]
        public class NodeParameters {
            public static readonly NodeParameters zero = new NodeParameters(0, 0, 0, 0, Vector3d.zero);

            public readonly double time;

            public readonly double radialOut;

            public readonly double normal;

            public readonly double prograde;

            public readonly Vector3d deltaV;

            public NodeParameters(double time, double radialOut, double normal, double prograde, Vector3d deltaV) {
                this.time = time;
                this.radialOut = radialOut;
                this.normal = normal;
                this.prograde = prograde;
                this.deltaV = deltaV;
            }

            public bool Valid {
                get {
                    return time >= Planetarium.GetUniversalTime() &&
                           !double.IsNaN(radialOut) && !double.IsInfinity(radialOut) &&
                           !double.IsNaN(normal) && !double.IsInfinity(normal) &&
                           !double.IsNaN(prograde) && !double.IsInfinity(prograde);
                }
            }

            public Vector3d NodeDeltaV => new Vector3d(radialOut, normal, prograde);

            public ManeuverNode AddToVessel(Vessel vessel) {
                if (!Valid) {
                    throw new Exception("Invalid NodeParameters");
                }

                ManeuverNode node = vessel.patchedConicSolver.AddManeuverNode(this.time);

                node.DeltaV = NodeDeltaV;

                vessel.patchedConicSolver.UpdateFlightPlan();

                return node;
            }


            public override string ToString() {
                return $"NodeParameters(time={time},dV={deltaV},r={radialOut},n={normal},p={prograde})";
            }
        }
    }
}
