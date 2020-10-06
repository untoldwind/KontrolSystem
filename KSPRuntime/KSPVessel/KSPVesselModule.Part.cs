using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Part")]
        public class PartAdapter {
            private VesselAdapter vesselAdapter;
            private readonly Part part;

            internal PartAdapter(VesselAdapter vesselAdapter, Part part) {
                this.vesselAdapter = vesselAdapter;
                this.part = part;
            }

            [KSField] public string PartName => part.partName;

            [KSField]
            public PartModuleAdapter[] Modules {
                get {
                    PartModuleList modules = part.Modules;
                    PartModuleAdapter[] adapters = new PartModuleAdapter[part.Modules.Count];

                    for (int i = 0; i < adapters.Length; i++)
                        adapters[i] = new PartModuleAdapter(vesselAdapter, modules[i]);

                    return adapters;
                }
            }
        }
    }
}
