using System.Collections.Generic;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;
using KontrolSystem.KSP.Runtime.KSPOrbit;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ManeuverNode")]
        public class ManeuverNodeAdapter {
            private readonly Vessel vessel;
            private readonly ManeuverNode maneuverNode;

            public ManeuverNodeAdapter(Vessel _vessel, ManeuverNode _maneuverNode) {
                vessel = _vessel;
                maneuverNode = _maneuverNode;
            }

            [KSField(IncludeSetter = true)]
            public double Time {
                get => maneuverNode.UT;
                set => maneuverNode.UT = value;
            }

            [KSField(IncludeSetter = true)]
            public double Prograde {
                get => maneuverNode.DeltaV.z;
                set => maneuverNode.DeltaV.z = value;
            }

            [KSField(IncludeSetter = true)]
            public double Normal {
                get => maneuverNode.DeltaV.y;
                set => maneuverNode.DeltaV.y = value;
            }

            [KSField(IncludeSetter = true)]
            public double RadialOut {
                get => maneuverNode.DeltaV.x;
                set => maneuverNode.DeltaV.x = value;
            }

            [KSField("ETA", IncludeSetter = true)]
            public double ETA {
                get => maneuverNode.UT - Planetarium.GetUniversalTime();
                set => maneuverNode.UT = value + Planetarium.GetUniversalTime();
            }

            [KSField(IncludeSetter = true)]
            public Vector3d BurnVector {
                get => maneuverNode.GetBurnVector(vessel.orbit);
                set {
                    KSPOrbitModule.IOrbit orbit = new OrbitWrapper(vessel.orbit);
                    maneuverNode.DeltaV = new Vector3d(
                        Vector3d.Dot(orbit.RadialPlus(maneuverNode.UT), value),
                        Vector3d.Dot(-orbit.NormalPlus(maneuverNode.UT), value),
                        Vector3d.Dot(orbit.Prograde(maneuverNode.UT), value)
                    );
                }
            }

            [KSMethod]
            public void Remove() => maneuverNode.RemoveSelf();
        }
    }
}
