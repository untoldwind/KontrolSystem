using System;
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
        [KSClass("Maneuver")]
        public class ManeuverAdapter {

            private readonly Vessel vessel;

            public ManeuverAdapter(Vessel _vessel) => vessel = _vessel;

            [KSField]
            public bool Available {
                get {
                    float buildingLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.MissionControl);

                    if (!GameVariables.Instance.UnlockedFlightPlanning(buildingLevel)) return false;
                    return PatchLimit > 0;
                }
            }

            [KSField]
            public long PatchLimit {
                get {
                    float buildingLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation);
                    return GameVariables.Instance.GetPatchesAheadLimit(buildingLevel);
                }
            }

            [KSField]
            public ManeuverNodeAdapter[] Nodes => vessel.patchedConicSolver?.maneuverNodes.Select(n => new ManeuverNodeAdapter(vessel, n)).ToArray() ?? new ManeuverNodeAdapter[0];

            [KSMethod]
            public Result<ManeuverNodeAdapter, string> NextNode() {
                ManeuverNode node = vessel.patchedConicSolver?.maneuverNodes.FirstOrDefault();

                if (node == null) return Result<ManeuverNodeAdapter, string>.failure("No maneuver node present");
                return Result<ManeuverNodeAdapter, string>.successful(new ManeuverNodeAdapter(vessel, node));
            }

            [KSMethod]
            public Result<ManeuverNodeAdapter, string> Add(double UT, double radialOut, double normal, double prograde) {
                if (vessel.patchedConicSolver == null) return Result<ManeuverNodeAdapter, string>.failure("Vessel maneuvers not availble");

                ManeuverNode node = vessel.patchedConicSolver.AddManeuverNode(UT);
                node.DeltaV = new Vector3d(radialOut, normal, prograde);

                return Result<ManeuverNodeAdapter, string>.successful(new ManeuverNodeAdapter(vessel, node));
            }

            [KSMethod]
            public Result<ManeuverNodeAdapter, string> AddBurnVector(double UT, Vector3d burnVector) {
                if (vessel.patchedConicSolver == null) return Result<ManeuverNodeAdapter, string>.failure("Vessel maneuvers not availble");

                ManeuverNode node = vessel.patchedConicSolver.AddManeuverNode(UT);
                KSPOrbitModule.IOrbit orbit = new OrbitWrapper(vessel.orbit);
                node.DeltaV = new Vector3d(
                    Vector3d.Dot(orbit.RadialPlus(UT), burnVector),
                    Vector3d.Dot(-orbit.NormalPlus(UT), burnVector),
                    Vector3d.Dot(orbit.Prograde(UT), burnVector)
                );
                vessel.patchedConicSolver.UpdateFlightPlan();

                return Result<ManeuverNodeAdapter, string>.successful(new ManeuverNodeAdapter(vessel, node));
            }
        }
    }
}
