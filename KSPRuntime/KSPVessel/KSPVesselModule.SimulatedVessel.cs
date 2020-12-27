using System.Collections.Generic;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass]
        public class SimulatedVessel {
            public List<SimulatedPart> parts = new List<SimulatedPart>();
            private int count;
            public double totalMass = 0;
            private SimCurves simCurves;

            public SimulatedVessel(Vessel v, SimCurves simCurves, double startTime, int limitChutesStage) {
                totalMass = 0;

                var oParts = v.Parts;
                count = oParts.Count;

                this.simCurves = simCurves;

                if (parts.Capacity < count)
                    parts.Capacity = count;

                for (int i = 0; i < count; i++) {
                    SimulatedPart simulatedPart = null;
                    bool special = false;
                    for (int j = 0; j < oParts[i].Modules.Count; j++) {
                        ModuleParachute mp = oParts[i].Modules[j] as ModuleParachute;
                        if (mp != null && v.mainBody.atmosphere) {
                            special = true;
                            simulatedPart = new SimulatedParachute(mp, simCurves, startTime, limitChutesStage);
                        }
                    }
                    if (!special) {
                        simulatedPart = new SimulatedPart(oParts[i], simCurves);
                    }

                    parts.Add(simulatedPart);
                    totalMass += simulatedPart.totalMass;
                }
            }
            
            [KSMethod]
            public Vector3d Drag(Vector3d localVelocity, double dynamicPressurekPa, double mach) {
                Vector3d drag = Vector3d.zero;

                double dragFactor = dynamicPressurekPa * PhysicsGlobals.DragCubeMultiplier * PhysicsGlobals.DragMultiplier;

                for (int i = 0; i < count; i++) {
                    drag += parts[i].Drag(localVelocity, dragFactor, (float)mach);
                }

                return -localVelocity.normalized * drag.magnitude;
            }

            [KSMethod]
            public Vector3d Lift(Vector3d localVelocity, double dynamicPressurekPa, double mach) {
                Vector3d lift = Vector3d.zero;

                double liftFactor = dynamicPressurekPa * simCurves.LiftMachCurve.Evaluate((float)mach);

                for (int i = 0; i < count; i++) {
                    lift += parts[i].Lift(localVelocity, liftFactor);
                }
                return lift;
            }
            
            [KSMethod]
            public bool WillChutesDeploy(double altAGL, double altASL, double probableLandingSiteASL, double pressure, double shockTemp, double t, double parachuteSemiDeployMultiplier) {
                for (int i = 0; i < count; i++) {
                    if (parts[i].SimulateAndRollback(altAGL, altASL, probableLandingSiteASL, pressure, shockTemp, t, parachuteSemiDeployMultiplier)) {
                        return true;
                    }
                }
                return false;
            }

            [KSMethod]
            public bool Simulate(double altATGL, double altASL, double endASL, double pressure, double shockTemp, double time, double semiDeployMultiplier) {
                bool deploying = false;
                for (int i = 0; i < count; i++) {
                    deploying |= parts[i].Simulate(altATGL, altASL, endASL, pressure, shockTemp, time, semiDeployMultiplier);
                }
                return deploying;
            }            
        }
    }
}
