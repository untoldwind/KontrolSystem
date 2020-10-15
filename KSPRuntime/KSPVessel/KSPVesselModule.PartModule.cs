using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("PartModule")]
        public class PartModuleAdapter {
            protected readonly VesselAdapter vesselAdapter;
            private readonly PartModule partModule;

            internal static PartModuleAdapter Wrap(VesselAdapter vesselAdapter, PartModule partModule) {
                switch (partModule) {
                case ModuleDeployablePart deployable:
                    return new ModuleDeployablePartAdapter(vesselAdapter, deployable);
                case ModuleEngines engines:
                    return new ModuleEngineAdapter(vesselAdapter, engines);
                default:
                    return new PartModuleAdapter(vesselAdapter, partModule);
                }
            }

            internal PartModuleAdapter(VesselAdapter vesselAdapter, PartModule partModule) {
                this.vesselAdapter = vesselAdapter;
                this.partModule = partModule;
            }
            
            [KSField]
            public Option<ModuleEngineAdapter> AsEngine {
                get {
                    if (partModule is ModuleEngines engines)
                        return new Option<ModuleEngineAdapter>(new ModuleEngineAdapter(vesselAdapter, engines));
                    return new Option<ModuleEngineAdapter>();
                }
            }

            [KSField]
            public Option<ModuleDeployablePartAdapter> AsDeployable {
                get {
                    if (partModule is ModuleDeployablePart deployable)
                        return new Option<ModuleDeployablePartAdapter>(new ModuleDeployablePartAdapter(vesselAdapter, deployable));
                    return new Option<ModuleDeployablePartAdapter>();
                    
                }
            }

            [KSField] public string ModuleName => partModule.moduleName;

            [KSField] public string ClassName => partModule.ClassName;

            [KSField]
            public Vector3d Position => partModule.part.transform.position -
                                        (FlightGlobals.ActiveVessel?.CoMD ?? vesselAdapter.vessel.CoMD);

            [KSMethod]
            public bool HasField(string fieldName) {
                foreach (var field in partModule.Fields) {
                    if (field.name == fieldName) return true;
                }

                return false;
            }

            [KSMethod]
            public bool HasAction(string actionName) {
                foreach (var action in partModule.Actions) {
                    if (action.name == actionName) return true;
                }

                return false;
            }

            [KSMethod]
            public bool HasEvent(string eventName) {
                foreach (var evt in partModule.Events) {
                    if (evt.name == eventName) return true;
                }

                return false;
            }

            [KSField] public PartAdapter Part => new PartAdapter(vesselAdapter, partModule.part);

            [KSField] public VesselAdapter Vessel => vesselAdapter;

            [KSField]
            public string Tag {
                get => Part.Tag;
                set => Part.Tag = value;
            }
        }
    }
}
