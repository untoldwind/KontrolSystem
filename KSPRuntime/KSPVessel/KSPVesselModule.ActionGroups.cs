using System.Collections.Generic;
using KontrolSystem.TO2.Binding;
using System.Linq;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ActionGroups")]
        public class ActionGroupsAdapter {
            private readonly Vessel vessel;

            public ActionGroupsAdapter(Vessel vessel) => this.vessel = vessel;

            [KSField]
            public bool Sas {
                get => vessel.ActionGroups[KSPActionGroup.SAS];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.SAS, value);
            }

            [KSField]
            public bool Rcs {
                get => vessel.ActionGroups[KSPActionGroup.RCS];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.RCS, value);
            }

            [KSField]
            public bool Breaks {
                get => vessel.ActionGroups[KSPActionGroup.Brakes];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.Brakes, value);
            }

            [KSField]
            public bool Gear {
                get => vessel.ActionGroups[KSPActionGroup.Gear];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.Gear, value);
            }

            [KSField]
            public bool Light {
                get => vessel.ActionGroups[KSPActionGroup.Light];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.Light, value);
            }

            [KSField]
            public bool Abort {
                get => vessel.ActionGroups[KSPActionGroup.Abort];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.Abort, value);
            }

            [KSField]
            public bool Bays {
                get {
                    foreach (var p in vessel.parts) {
                        foreach (var c in p.FindModulesImplementing<ModuleCargoBay>()) {
                            var m = p.Modules[
                                    c.DeployModuleIndex] as
                                ModuleAnimateGeneric; //apparently, it's referenced by the number
                            if (m != null) {
                                //bays have ModuleAnimateGeneric, fairings have their own, but they all use ModuleCargoBay
                                if (m.animSwitch == (c.closedPosition != 0)) {
                                    //even one open bay may be critical, therefore return true if any found
                                    return true;
                                }
                            }
                        }
                    }

                    return false;
                }
                set {
                    foreach (var p in vessel.parts) {
                        foreach (var c in p.FindModulesImplementing<ModuleCargoBay>()) {
                            var m = p.Modules[
                                    c.DeployModuleIndex] as
                                ModuleAnimateGeneric; //apparently, it's referenced by the number
                            if (m != null) {
                                if ((m.animSwitch == (c.closedPosition == 0))) {
                                    //closed/closing
                                    if (value) {
                                        m.Toggle();
                                    } //open
                                } else {
                                    //open/opening
                                    if (!value) {
                                        m.Toggle();
                                    } //close
                                }
                            }
                        }
                    }
                }
            }

            [KSField]
            public bool Intakes {
                get {
                    var atLeastOneIntake = false; // No intakes at all? Always return false

                    foreach (var p in vessel.parts) {
                        foreach (var c in p.FindModulesImplementing<ModuleResourceIntake>()) {
                            atLeastOneIntake = true;

                            if (!c.intakeEnabled) {
                                // If just one intake is not open return false
                                return false;
                            }
                        }
                    }

                    return atLeastOneIntake;
                }
                set {
                    foreach (var p in vessel.parts) {
                        foreach (var c in p.FindModulesImplementing<ModuleResourceIntake>()) {
                            if (value) {
                                c.Activate();
                            } else {
                                c.Deactivate();
                            }
                        }
                    }
                }
            }

            [KSField]
            public bool Custom1 {
                get => vessel.ActionGroups[KSPActionGroup.Custom01];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.Custom01, value);
            }

            [KSField]
            public bool Custom2 {
                get => vessel.ActionGroups[KSPActionGroup.Custom02];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.Custom02, value);
            }

            [KSField]
            public bool Custom3 {
                get => vessel.ActionGroups[KSPActionGroup.Custom03];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.Custom03, value);
            }

            [KSField]
            public bool Custom4 {
                get => vessel.ActionGroups[KSPActionGroup.Custom04];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.Custom04, value);
            }

            [KSField]
            public bool Custom5 {
                get => vessel.ActionGroups[KSPActionGroup.Custom05];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.Custom05, value);
            }

            [KSField]
            public bool Custom6 {
                get => vessel.ActionGroups[KSPActionGroup.Custom06];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.Custom06, value);
            }

            [KSField]
            public bool Custom7 {
                get => vessel.ActionGroups[KSPActionGroup.Custom07];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.Custom07, value);
            }

            [KSField]
            public bool Custom8 {
                get => vessel.ActionGroups[KSPActionGroup.Custom08];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.Custom08, value);
            }

            [KSField]
            public bool Custom9 {
                get => vessel.ActionGroups[KSPActionGroup.Custom09];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.Custom09, value);
            }

            [KSField]
            public bool Custom10 {
                get => vessel.ActionGroups[KSPActionGroup.Custom10];
                set => vessel.ActionGroups.SetGroup(KSPActionGroup.Custom10, value);
            }

            [KSField]
            public bool Panels {
                get {
                    bool atLeastOneSolarPanel = false;
                    foreach (Part p in vessel.parts) {
                        foreach (ModuleDeployableSolarPanel c in p.FindModulesImplementing<ModuleDeployableSolarPanel>()
                            .Where(m => m.useAnimation)) {
                            atLeastOneSolarPanel = true;

                            if (c.deployState == ModuleDeployablePart.DeployState.RETRACTED) return false;
                        }
                    }

                    return atLeastOneSolarPanel;
                }
                set {
                    foreach (Part p in vessel.parts) {
                        foreach (ModuleDeployableSolarPanel c in p.FindModulesImplementing<ModuleDeployableSolarPanel>()
                            .Where(m => m.useAnimation)) {
                            if (value) c.Extend();
                            else c.Retract();
                        }
                    }
                }
            }

            [KSField]
            public bool Antennas {
                get {
                    bool atLeastOneAntenna = false;
                    foreach (Part p in vessel.parts) {
                        foreach (ModuleDeployableAntenna c in p.FindModulesImplementing<ModuleDeployableAntenna>()) {
                            atLeastOneAntenna = true;

                            if (c.deployState == ModuleDeployablePart.DeployState.RETRACTED) return false;
                        }
                    }

                    return atLeastOneAntenna;
                }
                set {
                    foreach (Part p in vessel.parts) {
                        foreach (ModuleDeployableAntenna c in p.FindModulesImplementing<ModuleDeployableAntenna>()) {
                            if (value) c.Extend();
                            else c.Retract();
                        }
                    }
                }
            }

            [KSField]
            public bool Radiators {
                get {
                    bool atLeastOneRadiator = false;
                    foreach (Part p in vessel.parts) {
                        foreach (ModuleActiveRadiator c in p.FindModulesImplementing<ModuleActiveRadiator>()) {
                            atLeastOneRadiator = true;

                            if (!c.IsCooling) return false;
                        }
                    }

                    return atLeastOneRadiator;
                }
                set {
                    foreach (Part p in vessel.parts) {
                        List<ModuleActiveRadiator> radiators = p.FindModulesImplementing<ModuleActiveRadiator>();
                        if (radiators.Count == 0) continue;
                        List<ModuleDeployableRadiator> deployables =
                            p.FindModulesImplementing<ModuleDeployableRadiator>();

                        if (deployables.Count > 0) {
                            foreach (ModuleDeployableRadiator deployable in deployables) {
                                if (value) deployable.Extend();
                                else deployable.Retract();
                            }
                        } else {
                            foreach (ModuleActiveRadiator radiator in radiators) {
                                if (value) radiator.Activate();
                                else radiator.Shutdown();
                            }
                        }
                    }
                }
            }

            [KSMethod]
            public void DeployFairings() {
                foreach (Part p in vessel.parts) {
                    foreach (ModuleProceduralFairing c in p.FindModulesImplementing<ModuleProceduralFairing>()) {
                        c.DeployFairing();
                    }
                }
            }
        }
    }
}
