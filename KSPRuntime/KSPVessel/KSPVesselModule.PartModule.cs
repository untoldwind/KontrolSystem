using System;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("PartModule")]
        public class PartModuleAdapter {
            protected readonly VesselAdapter vesselAdapter;
            protected readonly PartModule partModule;

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
                        return new Option<ModuleDeployablePartAdapter>(
                            new ModuleDeployablePartAdapter(vesselAdapter, deployable));
                    return new Option<ModuleDeployablePartAdapter>();
                }
            }

            [KSField]
            public Option<ModuleExperimentAdapter> AsExperiment {
                get {
                    if (partModule is ModuleScienceExperiment experiment)
                        return new Option<ModuleExperimentAdapter>(
                            new ModuleExperimentAdapter(vesselAdapter, experiment));
                    return new Option<ModuleExperimentAdapter>();
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
                    if (string.Equals(field.name, fieldName, StringComparison.InvariantCultureIgnoreCase)) return true;
                }

                return false;
            }

            [KSMethod]
            public bool HasAction(string actionName) {
                foreach (var action in partModule.Actions) {
                    if (string.Equals(action.name, actionName, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }

                return false;
            }

            [KSMethod]
            public Result<object, string> DoAction(string actionName, bool activate) {
                foreach (var action in partModule.Actions) {
                    if (string.Equals(action.name, actionName, StringComparison.InvariantCultureIgnoreCase)) {
                        action.Invoke(new KSPActionParam(action.actionGroup,
                            (activate ? KSPActionType.Activate : KSPActionType.Deactivate)));
                    }
                }

                return Result.Err<object, string>($"No action {actionName} found");
            }

            [KSMethod]
            public bool HasEvent(string eventName) {
                foreach (var evt in partModule.Events) {
                    if (string.Equals(evt.name, eventName, StringComparison.InvariantCultureIgnoreCase)) return true;
                }

                return false;
            }

            [KSMethod]
            public Result<object, string> DoEvent(string eventName) {
                foreach (var evt in partModule.Events) {
                    if (string.Equals(evt.name, eventName, StringComparison.InvariantCultureIgnoreCase)) {
                        evt.Invoke();
                        return Result.Ok<object, string>(null);
                    }
                }

                return Result.Err<object, string>($"No event {eventName} found");
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
