using System;
using System.Linq;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Part")]
        public class PartAdapter {
            private readonly VesselAdapter vesselAdapter;
            internal readonly Part part;

            internal PartAdapter(VesselAdapter vesselAdapter, Part part) {
                this.vesselAdapter = vesselAdapter;
                this.part = part;
            }

            [KSField] public string PartName => part.name;

            [KSField] public string PartId => part.flightID.ToString();

            [KSField] public bool HasPhysics => part.physicalSignificance == Part.PhysicalSignificance.FULL;

            [KSField] public double Mass => HasPhysics && part.rb != null ? part.rb.mass : part.mass;

            [KSField] public double DryMass => Mass - part.resourceMass;

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

            [KSMethod]
            public bool HasModule(string moduleName) {
                foreach (var module in part.Modules) {
                    if (string.Equals(module.moduleName, moduleName, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }

                return false;
            }

            [KSMethod]
            public Option<PartModuleAdapter> GetModule(string moduleName) {
                foreach (var module in part.Modules) {
                    if (string.Equals(module.moduleName, moduleName, StringComparison.InvariantCultureIgnoreCase))
                        return new Option<PartModuleAdapter>(new PartModuleAdapter(vesselAdapter, module));
                }

                return new Option<PartModuleAdapter>();
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
