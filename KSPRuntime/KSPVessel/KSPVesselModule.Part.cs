using System.Collections.Generic;
using System.Linq;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Part")]
        public class PartAdapter {
            private VesselAdapter vesselAdapter;
            private readonly Part part;

            internal PartAdapter(VesselAdapter _vesselAdapter, Part _part) {
                vesselAdapter = _vesselAdapter;
                part = _part;
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
