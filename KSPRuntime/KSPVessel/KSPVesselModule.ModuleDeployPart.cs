using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("DeployablePart")]
        public class ModuleDeployablePartAdapter : PartModuleAdapter {
            private readonly ModuleDeployablePart moduleDeployablePart;

            public ModuleDeployablePartAdapter(VesselAdapter vesselAdapter, ModuleDeployablePart moduleDeployablePart)
                : base(vesselAdapter, moduleDeployablePart) => this.moduleDeployablePart = moduleDeployablePart;

            [KSMethod]
            public void Extend() => moduleDeployablePart.Extend();

            [KSMethod]
            public void Retract() => moduleDeployablePart.Retract();

            [KSField] public bool IsMoving => moduleDeployablePart.IsMoving();
        }
    }
}
