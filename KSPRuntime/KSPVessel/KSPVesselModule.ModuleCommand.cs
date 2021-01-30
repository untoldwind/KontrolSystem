using KontrolSystem.TO2.Binding;
using UniLinq;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Control")]
        public class ModuleCommandAdapter : PartModuleAdapter {
            private readonly ModuleCommand moduleCommand;

            public ModuleCommandAdapter(VesselAdapter vesselAdapter, ModuleCommand moduleDeployablePart)
                : base(vesselAdapter, moduleDeployablePart) => this.moduleCommand = moduleDeployablePart;
            
            [KSMethod]
            public void ControlFrom() => moduleCommand.MakeReference();

            [KSField]
            public string[] ControlPoints => moduleCommand.controlPoints != null
                ? moduleCommand.controlPoints.Keys.ToArray()
                : new string[0];

            [KSField]
            public string ActiveControlPoint {
                get => moduleCommand.ActiveControlPointName;
                set => moduleCommand.SetControlPoint(value);
            }
        }
    }
}
