using System.Collections.Generic;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ModuleDeployablePart")]
        public class ModuleDeployablePartAdapter : PartModuleAdapter {
            private readonly ModuleDeployablePart moduleDeployablePart;

            public ModuleDeployablePartAdapter(VesselAdapter vesselAdapter, ModuleDeployablePart _moduleDeployablePart) : base(vesselAdapter, _moduleDeployablePart) => moduleDeployablePart = _moduleDeployablePart;

            [KSMethod]
            public void Extend() => moduleDeployablePart.Extend();

            [KSMethod]
            public void Retract() => moduleDeployablePart.Retract();

            [KSField]
            public bool IsMoving => moduleDeployablePart.IsMoving();
        }
    }
}
