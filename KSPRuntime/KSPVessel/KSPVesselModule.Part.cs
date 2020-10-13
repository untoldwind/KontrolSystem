using System.Linq;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Part")]
        public class PartAdapter {
            private readonly VesselAdapter vesselAdapter;
            private readonly Part part;

            internal PartAdapter(VesselAdapter vesselAdapter, Part part) {
                this.vesselAdapter = vesselAdapter;
                this.part = part;
            }

            [KSField] public string PartName => part.name;

            [KSField] public string PartId => part.flightID.ToString();

            [KSField] public bool HasPhysics => part.physicalSignificance == Part.PhysicalSignificance.FULL;

            [KSField] public double Mass => HasPhysics && part.rb != null ? part.rb.mass : part.mass;

            [KSField] public double DryMass => Mass - part.resourceMass;

            public PartModuleAdapter[] Modules {
                get {
                    PartModuleList modules = part.Modules;
                    PartModuleAdapter[] adapters = new PartModuleAdapter[part.Modules.Count];

                    for (int i = 0; i < adapters.Length; i++)
                        adapters[i] = new PartModuleAdapter(vesselAdapter, modules[i]);

                    return adapters;
                }
            }

            [KSMethod]
            public bool HasModule(string moduleName) {
                foreach (var module in part.Modules) {
                    if (module.moduleName == moduleName) return true;
                }

                return false;
            }

            [KSField]
            public string Tag {
                get {
                    INameTag nameTag = part.Modules.GetModules<INameTag>().FirstOrDefault();
                    return nameTag?.Tag ?? "";
                }
                set {
                    INameTag nameTag = part.Modules.GetModules<INameTag>().FirstOrDefault();
                    if (nameTag != null) nameTag.Tag = value;
                }
            }

            [KSField] public VesselAdapter Vessel => vesselAdapter;
        }
    }
}
