using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPControl;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;
using System.Linq;
using KSP.UI.Screens;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ActionGroups")]
        public class ActionGroupsAdapter {
            private readonly Vessel vessel;

            public ActionGroupsAdapter(Vessel _vessel) => vessel = _vessel;

            [KSField(IncludeSetter = true)]
            public bool Sas {
                get => vessel.ActionGroups[KSPActionGroup.SAS];
                set => vessel.ActionGroups[KSPActionGroup.SAS] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Rcs {
                get => vessel.ActionGroups[KSPActionGroup.RCS];
                set => vessel.ActionGroups[KSPActionGroup.RCS] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Breaks {
                get => vessel.ActionGroups[KSPActionGroup.Brakes];
                set => vessel.ActionGroups[KSPActionGroup.Brakes] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Gear {
                get => vessel.ActionGroups[KSPActionGroup.Gear];
                set => vessel.ActionGroups[KSPActionGroup.Gear] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Light {
                get => vessel.ActionGroups[KSPActionGroup.Light];
                set => vessel.ActionGroups[KSPActionGroup.Light] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Abort {
                get => vessel.ActionGroups[KSPActionGroup.Abort];
                set => vessel.ActionGroups[KSPActionGroup.Abort] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Custom1 {
                get => vessel.ActionGroups[KSPActionGroup.Custom01];
                set => vessel.ActionGroups[KSPActionGroup.Custom01] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Custom2 {
                get => vessel.ActionGroups[KSPActionGroup.Custom02];
                set => vessel.ActionGroups[KSPActionGroup.Custom02] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Custom3 {
                get => vessel.ActionGroups[KSPActionGroup.Custom03];
                set => vessel.ActionGroups[KSPActionGroup.Custom03] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Custom4 {
                get => vessel.ActionGroups[KSPActionGroup.Custom04];
                set => vessel.ActionGroups[KSPActionGroup.Custom04] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Custom5 {
                get => vessel.ActionGroups[KSPActionGroup.Custom05];
                set => vessel.ActionGroups[KSPActionGroup.Custom05] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Custom6 {
                get => vessel.ActionGroups[KSPActionGroup.Custom06];
                set => vessel.ActionGroups[KSPActionGroup.Custom06] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Custom7 {
                get => vessel.ActionGroups[KSPActionGroup.Custom07];
                set => vessel.ActionGroups[KSPActionGroup.Custom07] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Custom8 {
                get => vessel.ActionGroups[KSPActionGroup.Custom08];
                set => vessel.ActionGroups[KSPActionGroup.Custom08] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Custom9 {
                get => vessel.ActionGroups[KSPActionGroup.Custom09];
                set => vessel.ActionGroups[KSPActionGroup.Custom09] = value;
            }

            [KSField(IncludeSetter = true)]
            public bool Custom10 {
                get => vessel.ActionGroups[KSPActionGroup.Custom10];
                set => vessel.ActionGroups[KSPActionGroup.Custom10] = value;
            }

            [KSField(IncludeSetter = true)]
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

            [KSField(IncludeSetter = true)]
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

            [KSField(IncludeSetter = true)]
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
