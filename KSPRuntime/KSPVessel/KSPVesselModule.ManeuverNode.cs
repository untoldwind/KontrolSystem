using KontrolSystem.TO2.Binding;
using KontrolSystem.KSP.Runtime.KSPOrbit;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ManeuverNode")]
        public class ManeuverNodeAdapter {
            private readonly Vessel vessel;
            private readonly ManeuverNode maneuverNode;

            public ManeuverNodeAdapter(Vessel vessel, ManeuverNode maneuverNode) {
                this.vessel = vessel;
                this.maneuverNode = maneuverNode;
            }

            [KSField]
            public double Time {
                get => maneuverNode.UT;
                set => UpdateNode(maneuverNode.DeltaV.x, maneuverNode.DeltaV.y, maneuverNode.DeltaV.z, value);
            }

            [KSField]
            public double Prograde {
                get => maneuverNode.DeltaV.z;
                set => UpdateNode(maneuverNode.DeltaV.x, maneuverNode.DeltaV.y, value, maneuverNode.UT);
            }

            [KSField]
            public double Normal {
                get => maneuverNode.DeltaV.y;
                set => UpdateNode(maneuverNode.DeltaV.x, value, maneuverNode.DeltaV.z, maneuverNode.UT);
            }

            [KSField]
            public double RadialOut {
                get => maneuverNode.DeltaV.x;
                set => UpdateNode(value, maneuverNode.DeltaV.y, maneuverNode.DeltaV.z, maneuverNode.UT);
            }

            [KSField("ETA")]
            public double ETA {
                get => maneuverNode.UT - Planetarium.GetUniversalTime();
                set => UpdateNode(maneuverNode.DeltaV.x, maneuverNode.DeltaV.y, maneuverNode.DeltaV.z,
                    value + Planetarium.GetUniversalTime());
            }

            [KSField]
            public Vector3d BurnVector {
                get => maneuverNode.GetBurnVector(vessel.orbit);
                set {
                    KSPOrbitModule.IOrbit orbit = new OrbitWrapper(vessel.orbit);
                    UpdateNode(
                        Vector3d.Dot(orbit.RadialPlus(maneuverNode.UT), value),
                        Vector3d.Dot(-orbit.NormalPlus(maneuverNode.UT), value),
                        Vector3d.Dot(orbit.Prograde(maneuverNode.UT), value),
                        maneuverNode.UT);
                }
            }

            [KSMethod]
            public void Remove() => maneuverNode.RemoveSelf();

            private void UpdateNode(double radialOut, double normal, double prograde, double UT) {
                if (maneuverNode.attachedGizmo == null) {
                    maneuverNode.DeltaV = new Vector3d(radialOut, normal, prograde);
                    maneuverNode.UT = UT;
                    maneuverNode.solver.UpdateFlightPlan();
                } else {
                    maneuverNode.OnGizmoUpdated(new Vector3d(radialOut, normal, prograde), UT);
                }
            }
        }
    }
}
