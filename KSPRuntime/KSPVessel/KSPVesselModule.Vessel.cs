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
            Description = "Represents an in-game vessel, which might be a rocket, plane, rover ... or actually just a Kerbal in a spacesuite."
        )]
        public class VesselAdapter : IFixedUpdateObserver {
            internal readonly IKSPContext context;
            internal readonly Vessel vessel;
            internal readonly VesselStageAdapter stage;
            internal readonly ActionGroupsAdapter actions;
            internal readonly ManeuverAdapter maneuver;
            private double sampleTime;

            public static Option<VesselAdapter> NullSafe(IKSPContext context, Vessel vessel) => vessel != null ? new Option<VesselAdapter>(new VesselAdapter(context, vessel)) : new Option<VesselAdapter>();

            internal VesselAdapter(IKSPContext _context, Vessel _vessel) {
                context = _context;
                vessel = _vessel;
                stage = new VesselStageAdapter(vessel);
                actions = new ActionGroupsAdapter(vessel);
                maneuver = new ManeuverAdapter(vessel);
                sampleTime = 0.0;
                context.AddFixedUpdateObserver(new WeakReference<IFixedUpdateObserver>(this));
            }

            [KSField(Description = "The name of the vessel.")]
            public string Name => vessel.vesselName;

            [KSField]
            public string VesselType => vessel.vesselType.ToString();

            [KSField]
            public string Status => vessel.situation.ToString();

            [KSField]
            public VesselStageAdapter Stage => stage;

            [KSField]
            public ActionGroupsAdapter Actions => actions;

            [KSField]
            public ManeuverAdapter Maneuver => maneuver;

            [KSField]
            public PartAdapter[] Parts => vessel.parts.Select(part => new PartAdapter(this, part)).ToArray();

            [KSField]
            public ModuleEngineAdapter[] Engines => vessel.parts.SelectMany(part => part.Modules.GetModules<ModuleEngines>()).Select(module => new ModuleEngineAdapter(this, module)).ToArray();

            [KSField]
            public IVolume[] Volumes => vessel.parts.SelectMany(part => part.Modules.GetModules<IVolume>()).ToArray();

            [KSField]
            public bool IsActive => vessel.isActiveVessel;

            [KSField]
            public bool IsCommandable => vessel.isCommandable;

            [KSField]
            public bool IsEVA => vessel.isEVA;

            [KSField]
            public bool CanSeparate => vessel.isActiveVessel && StageManager.CanSeparate;

            [KSField]
            public KSPOrbitModule.IBody MainBody => new BodyWrapper(vessel.mainBody);

            [KSField]
            public KSPOrbitModule.IOrbit Orbit => new OrbitWrapper(vessel.orbit);

            [KSField]
            public double Mass => vessel.GetTotalMass();

            [KSField]
            public Vector3d Position => vessel.CoMD - (FlightGlobals.ActiveVessel?.CoMD ?? Vector3d.zero);

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


            [KSField]
            public Vector3d CoM => vessel.CoMD;

            [KSField]
            public Vector3d VesselUp => (vessel.CoMD - vessel.mainBody.position).normalized;

            [KSField]
            public Vector3d NorthVector => Vector3d.Exclude(vessel.upAxis, vessel.mainBody.transform.up);

            [KSField]
            public double Heading {
                get {
                    Quaternion headingQ = Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(vessel.GetTransform().rotation) *
                                                             Quaternion.LookRotation(NorthVector, vessel.upAxis));
                    return headingQ.eulerAngles.y;
                }
            }

            [KSMethod]
            public Direction HeadingDirection(double degreesFromNorth, double pitchAboveHorizon, double roll) {
                Quaternion q = Quaternion.LookRotation(NorthVector, vessel.upAxis);
                q *= Quaternion.Euler((float)-pitchAboveHorizon, (float)degreesFromNorth, 0);
                q *= Quaternion.Euler(0, 0, (float)roll);
                return new Direction(q);
            }

            [KSField]
            public Direction Prograde => new Direction(Quaternion.LookRotation(vessel.obt_velocity.normalized, vessel.upAxis));

            [KSField]
            public Direction Retrograde => new Direction(Quaternion.LookRotation(-vessel.obt_velocity.normalized, vessel.upAxis));

            [KSField]
            public Vector3d OrbitalVelocity => vessel.obt_velocity;

            [KSField]
            public Vector3d SurfaceVelocity => vessel.srf_velocity;

            [KSField]
            public double SampleTime => sampleTime;

            [KSField]
            public double VerticalSpeed => vessel.verticalSpeed;

            [KSField]
            public double AirSpeed => (vessel.orbit.GetVel() - vessel.mainBody.getRFrmVel(vessel.CoMD)).magnitude;

            [KSField]
            public double GroundSpeed {
                get {
                    double squared2DSpeed = vessel.srfSpeed * vessel.srfSpeed - vessel.verticalSpeed * vessel.verticalSpeed;
                    if (squared2DSpeed < 0) squared2DSpeed = 0;
                    return Math.Sqrt(squared2DSpeed);
                }
            }

            [KSField]
            public Direction Facing {
                get {
                    Quaternion vesselRotation = vessel.ReferenceTransform.rotation;
                    Quaternion vesselFacing = Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(vesselRotation) * Quaternion.identity);
                    return new Direction(vesselFacing);
                }
            }

            [KSField]
            public double Altitude => vessel.altitude;

            [KSField]
            public Vector3d AngularMomentum => vessel.angularMomentum;

            [KSField]
            public Vector3d AngularVelocity => Facing.Rotation * vessel.angularVelocity;

            [KSMethod("stage_deltav", Description = "Get delta-v information for a specific `stage` of the vessel, if existent.")]
            public Option<DeltaVStageInfoAdapter> StageDeltaV(long stage) {
                DeltaVStageInfo stageInfo = vessel.VesselDeltaV.GetStage((int)stage);

                return stageInfo != null ? new Option<DeltaVStageInfoAdapter>(new DeltaVStageInfoAdapter(this, stageInfo)) : new Option<DeltaVStageInfoAdapter>();
            }

            [KSField]
            public KSPOrbitModule.GeoCoordinates GeoCoordinates {
                get {
                    double latitude = MainBody.GetLatitude(vessel.CoMD);
                    double longitude = MainBody.GetLongitude(vessel.CoMD);

                    return new KSPOrbitModule.GeoCoordinates(MainBody, latitude, longitude);
                }
            }

            [KSMethod]
            public KSPControlModule.SteeringManager ManageSteering(Func<Direction> directionProvider) => new KSPControlModule.SteeringManager(context, this, directionProvider);

            [KSMethod]
            public KSPControlModule.ThrottleManager ManageThrottle(Func<double> throttleProvider) => new KSPControlModule.ThrottleManager(context, vessel, throttleProvider);

            public void OnFixedUpdate() {
                if (!vessel.HoldPhysics) {
                    sampleTime += TimeWarp.fixedDeltaTime;
                }
            }
        }
    }
}
