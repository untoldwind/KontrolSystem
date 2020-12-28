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
        [KSClass("Vessel",
            Description =
                "Represents an in-game vessel, which might be a rocket, plane, rover ... or actually just a Kerbal in a spacesuite."
        )]
        public class VesselAdapter : IKSPTargetable, IFixedUpdateObserver {
            public readonly IKSPContext context;
            public readonly Vessel vessel;
            private readonly VesselStageAdapter stage;
            private readonly ActionGroupsAdapter actions;
            private readonly ManeuverAdapter maneuver;
            private double sampleTime;

            public static Option<VesselAdapter> NullSafe(IKSPContext context, Vessel vessel) => vessel != null
                ? new Option<VesselAdapter>(new VesselAdapter(context, vessel))
                : new Option<VesselAdapter>();

            internal VesselAdapter(IKSPContext context, Vessel vessel) {
                this.context = context;
                this.vessel = vessel;
                stage = new VesselStageAdapter(this.vessel);
                actions = new ActionGroupsAdapter(this.vessel);
                maneuver = new ManeuverAdapter(this.vessel);
                sampleTime = 0.0;
                this.context.AddFixedUpdateObserver(this);
            }

            [KSField(Description = "The name of the vessel.")]
            public string Name => vessel.vesselName;

            [KSField] public string VesselType => vessel.vesselType.ToString();

            [KSField] public string Status => vessel.situation.ToString();

            [KSField] public string ControlLevel => vessel.CurrentControlLevel.ToString();

            [KSField] public VesselStageAdapter Stage => stage;

            [KSField] public ActionGroupsAdapter Actions => actions;

            [KSField] public ManeuverAdapter Maneuver => maneuver;

            [KSField] public PartAdapter[] Parts => vessel.parts.Select(part => new PartAdapter(this, part)).ToArray();

            [KSField]
            public CrewMemberAdapter[] Crew => vessel.GetVesselCrew().Select(crew => new CrewMemberAdapter(this, crew)).ToArray();

            [KSField] public long CrewCapacity => vessel.GetCrewCapacity();
            
            [KSField]
            public ModuleEngineAdapter[] Engines => vessel.parts
                .SelectMany(part => part.Modules.GetModules<ModuleEngines>())
                .Select(module => new ModuleEngineAdapter(this, module)).ToArray();

            [KSField]
            public ModuleDockingNodeAdapter[] DockingPorts => vessel.parts
                .Where(part => part.Modules.Contains<ModuleDockingNode>()).Select(part =>
                    new ModuleDockingNodeAdapter(this, part.Modules.GetModule<ModuleDockingNode>())).ToArray();

            [KSField]
            public IVolume[] Volumes => vessel.parts.SelectMany(part => part.Modules.GetModules<IVolume>()).ToArray();

            [KSField]
            public IDefaults Defaults =>
                new DefaultsWrapper(vessel.parts.SelectMany(part => part.Modules.GetModules<IDefaults>()).ToList());

            [KSField] public bool IsActive => vessel.isActiveVessel;

            [KSField] public bool IsCommandable => vessel.isCommandable;

            [KSField] public bool IsEva => vessel.isEVA;

            [KSField] public bool CanSeparate => vessel.isActiveVessel && StageManager.CanSeparate;

            [KSField] public KSPOrbitModule.IBody MainBody => new BodyWrapper(vessel.mainBody);

            [KSField] public KSPOrbitModule.IOrbit Orbit => new OrbitWrapper(vessel.orbit);

            [KSField] public double Mass => vessel.GetTotalMass();

            // ReSharper disable once Unity.NoNullPropagation
            [KSField] public Vector3d Position => vessel.CoMD - (FlightGlobals.ActiveVessel?.CoMD ?? Vector3d.zero);

            [KSField]
            public double AvailableThrust {
                get {
                    double thrust = 0.0;

                    foreach (Part part in vessel.parts)
                    foreach (PartModule module in part.Modules) {
                        ModuleEngines engine = module as ModuleEngines;
                        if (!module.isEnabled || engine == null) continue;
                        thrust += engine.thrustPercentage * engine.GetMaxThrust() / 100.0;
                    }

                    return thrust;
                }
            }


            [KSField] public Vector3d CoM => vessel.CoMD;

            [KSField] public Vector3d Up => vessel.up;

            [KSField] public Vector3d North => vessel.north;

            [KSField] public Vector3d East => vessel.east;

            [KSField]
            public double Heading {
                get {
                    Quaternion headingQ = Quaternion.Inverse(Quaternion.Euler(90, 0, 0) *
                                                             Quaternion.Inverse(vessel.GetTransform().rotation) *
                                                             Quaternion.LookRotation(North, vessel.upAxis));
                    return headingQ.eulerAngles.y;
                }
            }

            [KSMethod]
            public Direction HeadingDirection(double degreesFromNorth, double pitchAboveHorizon, double roll) {
                Quaternion q = Quaternion.LookRotation(North, vessel.upAxis);
                q *= Quaternion.Euler((float) -pitchAboveHorizon, (float) degreesFromNorth, 0);
                q *= Quaternion.Euler(0, 0, (float) roll);
                return new Direction(q);
            }

            [KSField]
            public double VelocityHeading {
                get {
                    Quaternion headingQ = Quaternion.Inverse(
                        Quaternion.Inverse(Quaternion.LookRotation(vessel.srf_velocity, vessel.upAxis)) *
                        Quaternion.LookRotation(North, vessel.upAxis));

                    return headingQ.eulerAngles.y;
                }
            }

            [KSField]
            public Direction Prograde =>
                new Direction(Quaternion.LookRotation(vessel.obt_velocity.normalized, vessel.upAxis));

            [KSField]
            public Direction Retrograde =>
                new Direction(Quaternion.LookRotation(-vessel.obt_velocity.normalized, vessel.upAxis));

            [KSField] public Vector3d OrbitalVelocity => vessel.obt_velocity;

            [KSField] public Vector3d SurfaceVelocity => vessel.srf_velocity;

            [KSField] public double HorizontalSurfaceSpeed => vessel.horizontalSrfSpeed;

            [KSField] public double SampleTime => sampleTime;

            [KSField] public double VerticalSpeed => vessel.verticalSpeed;

            [KSField]
            public double AirSpeed => (vessel.orbit.GetVel() - vessel.mainBody.getRFrmVel(vessel.CoMD)).magnitude;

            [KSField]
            public double GroundSpeed {
                get {
                    double squared2DSpeed =
                        vessel.srfSpeed * vessel.srfSpeed - vessel.verticalSpeed * vessel.verticalSpeed;
                    if (squared2DSpeed < 0) squared2DSpeed = 0;
                    return Math.Sqrt(squared2DSpeed);
                }
            }

            [KSField]
            public Direction Facing {
                get {
                    Quaternion vesselRotation = vessel.ReferenceTransform.rotation;
                    Quaternion vesselFacing = Quaternion.Inverse(Quaternion.Euler(90, 0, 0) *
                                                                 Quaternion.Inverse(vesselRotation) *
                                                                 Quaternion.identity);
                    return new Direction(vesselFacing);
                }
            }

            [KSField] public double Altitude => vessel.altitude;

            [KSField] public Vector3d AngularMomentum => vessel.angularMomentum;

            [KSField] public Vector3d AngularVelocity => Facing.Rotation * vessel.angularVelocity;

            [KSMethod("stage_deltav",
                Description = "Get delta-v information for a specific `stage` of the vessel, if existent.")]
            public Option<DeltaVStageInfoAdapter> StageDeltaV(long stage) {
                DeltaVStageInfo stageInfo = vessel.VesselDeltaV.GetStage((int) stage);

                return stageInfo != null
                    ? new Option<DeltaVStageInfoAdapter>(new DeltaVStageInfoAdapter(this, stageInfo))
                    : new Option<DeltaVStageInfoAdapter>();
            }

            [KSField]
            public KSPOrbitModule.GeoCoordinates GeoCoordinates {
                get {
                    double latitude = MainBody.Latitude(vessel.CoMD);
                    double longitude = MainBody.Longitude(vessel.CoMD);

                    return new KSPOrbitModule.GeoCoordinates(MainBody, latitude, longitude);
                }
            }

            [KSMethod]
            public double HeadingTo(KSPOrbitModule.GeoCoordinates geoCoordinates) {
                var up = Up;
                var north = North;

                var targetWorldCoords = MainBody.SurfacePosition(geoCoordinates.Latitude, geoCoordinates.Longitude,
                    geoCoordinates.TerrainHeight);

                var vector = Vector3d.Exclude(Up, targetWorldCoords - CoM).normalized;
                var headingQ =
                    Quaternion.Inverse(Quaternion.Euler(90, 0, 0) *
                                       Quaternion.Inverse(Quaternion.LookRotation(vector, up)) *
                                       Quaternion.LookRotation(north, up));

                return headingQ.eulerAngles.y;
            }

            [KSField]
            public Option<IKSPTargetable> Target {
                get {
                    var target = (FlightGlobals.ActiveVessel == vessel)
                        ? FlightGlobals.fetch.VesselTarget
                        : vessel.targetObject;

                    if (target == null) return new Option<IKSPTargetable>();
                    switch (target) {
                    case Vessel vesselTarget:
                        return new Option<IKSPTargetable>(new VesselAdapter(context, vesselTarget));
                    case CelestialBody bodyTarget:
                        return new Option<IKSPTargetable>(new BodyWrapper(bodyTarget));
                    case ModuleDockingNode dockingNodeTarget:
                        return new Option<IKSPTargetable>(
                            new ModuleDockingNodeAdapter(this, dockingNodeTarget));
                    default: return new Option<IKSPTargetable>();
                    }
                }
                set {
                    if (value.defined) {
                        FlightGlobals.fetch.SetVesselTarget(value.value.Underlying, true);
                    } else {
                        FlightGlobals.fetch.SetVesselTarget(null, true);
                    }
                }
            }
            
            [KSMethod]
            public KSPControlModule.SteeringManager SetSteering(Direction direction) =>
                new KSPControlModule.SteeringManager(context, this, () => direction);

            [KSMethod]
            public KSPControlModule.SteeringManager ManageSteering(Func<Direction> directionProvider) =>
                new KSPControlModule.SteeringManager(context, this, directionProvider);

            [KSMethod]
            public KSPControlModule.WheelSteeringManager SetWheelSteering(double bearing) =>
                new KSPControlModule.WheelSteeringManager(context, this, () => bearing);

            [KSMethod]
            public KSPControlModule.WheelSteeringManager ManageWheelSteering(Func<double> bearingProvider) =>
                new KSPControlModule.WheelSteeringManager(context, this, bearingProvider);

            [KSMethod]
            public KSPControlModule.ThrottleManager SetThrottle(double throttle) =>
                new KSPControlModule.ThrottleManager(context, vessel, () => throttle);

            [KSMethod]
            public KSPControlModule.ThrottleManager ManageThrottle(Func<double> throttleProvider) =>
                new KSPControlModule.ThrottleManager(context, vessel, throttleProvider);

            [KSMethod]
            public KSPControlModule.RCSTranslateManager SetRcsTranslate(Vector3d translate) =>
                new KSPControlModule.RCSTranslateManager(context, vessel, () => translate);

            [KSMethod]
            public KSPControlModule.RCSTranslateManager ManageRcsTranslate(Func<Vector3d> translateProvider) =>
                new KSPControlModule.RCSTranslateManager(context, vessel, translateProvider);

            [KSMethod]
            public KSPControlModule.WheelThrottleManager SetWheelThrottle(double throttle) =>
                new KSPControlModule.WheelThrottleManager(context, vessel, () => throttle);

            [KSMethod]
            public KSPControlModule.WheelThrottleManager ManageWheelThrottle(Func<double> throttleProvider) =>
                new KSPControlModule.WheelThrottleManager(context, vessel, throttleProvider);

            [KSMethod]
            public void ReleaseControl() => context.UnhookAllAutopilots(vessel);

            [KSMethod]
            public SimulatedVessel Simulated(long limitStage) => new SimulatedVessel(vessel,
                new SimCurves(vessel.orbit.referenceBody), vessel.orbit.StartUT, (int)limitStage);
            
            public Option<KSPOrbitModule.IBody> AsBody => new Option<KSPOrbitModule.IBody>();

            public Option<VesselAdapter> AsVessel => new Option<VesselAdapter>(this);

            public Option<ModuleDockingNodeAdapter> AsDockingPort => new Option<ModuleDockingNodeAdapter>();

            public ITargetable Underlying => vessel;

            public void OnFixedUpdate(double deltaTime) {
                if (!vessel.HoldPhysics) {
                    sampleTime += deltaTime;
                }
            }
        }
    }
}
