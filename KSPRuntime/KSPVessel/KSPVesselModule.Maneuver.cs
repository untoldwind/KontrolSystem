using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;
using System.Linq;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Maneuver")]
        public class ManeuverAdapter {
            private readonly Vessel vessel;

            public ManeuverAdapter(Vessel vessel) => this.vessel = vessel;

            [KSField]
            public bool Available {
                get {
                    float buildingLevel =
                        ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.MissionControl);

                    if (!GameVariables.Instance.UnlockedFlightPlanning(buildingLevel)) return false;
                    return PatchLimit > 0;
                }
            }

            [KSField]
            public long PatchLimit {
                get {
                    float buildingLevel =
                        ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation);
                    return GameVariables.Instance.GetPatchesAheadLimit(buildingLevel);
                }
            }

            [KSField]
            public ManeuverNodeAdapter[] Nodes =>
                // ReSharper disable once Unity.NoNullPropagation
                vessel.patchedConicSolver?.maneuverNodes.Select(n => new ManeuverNodeAdapter(vessel, n)).ToArray() ??
                new ManeuverNodeAdapter[0];

            [KSMethod]
            public Result<ManeuverNodeAdapter, string> NextNode() {
                // ReSharper disable once Unity.NoNullPropagation
                ManeuverNode node = vessel.patchedConicSolver?.maneuverNodes.FirstOrDefault();

                if (node == null) return Result.Err<ManeuverNodeAdapter, string>("No maneuver node present");
                return Result.Ok<ManeuverNodeAdapter, string>(new ManeuverNodeAdapter(vessel, node));
            }

            [KSMethod]
            public Result<ManeuverNodeAdapter, string>
                Add(double ut, double radialOut, double normal, double prograde) {
                if (vessel.patchedConicSolver == null)
                    return Result.Err<ManeuverNodeAdapter, string>("Vessel maneuvers not available");

                ManeuverNode node = vessel.patchedConicSolver.AddManeuverNode(ut);
                node.DeltaV = new Vector3d(radialOut, normal, prograde);

                return Result.Ok<ManeuverNodeAdapter, string>(new ManeuverNodeAdapter(vessel, node));
            }

            [KSMethod]
            public Result<ManeuverNodeAdapter, string> AddBurnVector(double ut, Vector3d burnVector) {
                if (vessel.patchedConicSolver == null)
                    return Result.Err<ManeuverNodeAdapter, string>("Vessel maneuvers not available");

                ManeuverNode node = vessel.patchedConicSolver.AddManeuverNode(ut);
                KSPOrbitModule.IOrbit orbit = new OrbitWrapper(vessel.orbit);
                node.DeltaV = new Vector3d(
                    Vector3d.Dot(orbit.RadialPlus(ut), burnVector),
                    Vector3d.Dot(-orbit.NormalPlus(ut), burnVector),
                    Vector3d.Dot(orbit.Prograde(ut), burnVector)
                );
                vessel.patchedConicSolver.UpdateFlightPlan();

                return Result.Ok<ManeuverNodeAdapter, string>(new ManeuverNodeAdapter(vessel, node));
            }
        }
    }
}
