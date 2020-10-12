using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Targetable")]
        public interface IKSPTargetable {
            [KSField] string Name { get; }

            [KSField] KSPOrbitModule.IOrbit Orbit { get; }

            [KSField("body")] Option<KSPOrbitModule.IBody> AsBody { get; }

            [KSField("vessel")] Option<VesselAdapter> AsVessel { get; }

            [KSField("docking_port")] Option<ModuleDockingNodeAdapter> AsDockingPort { get; }

            ITargetable Underlying { get; }
        }
    }
}
