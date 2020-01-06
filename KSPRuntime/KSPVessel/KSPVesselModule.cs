using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    [KSModule("ksp::vessel",
        Description = "Collection of types and functions to get information and control in-game vessels."
    )]
    public partial class KSPVesselModule {
        [KSConstant("TYPE_DEBIRS",
            Description = "Value of `vessel.type` if vessel is some space debris."
        )]
        public static readonly string TYPE_DEBIRS = VesselType.Debris.ToString();

        [KSConstant("TYPE_SPACEOBJECT",
            Description = "Value of `vessel.type` if vessel is some unspecified space object."
        )]
        public static readonly string TYPE_SPACEOBJECT = VesselType.SpaceObject.ToString();

        [KSConstant("TYPE_UNKOWN",
            Description = "Value of `vessel.type` if the type of the vessel is unknown/undefined."
        )]
        public static readonly string TYPE_UNKOWN = VesselType.Unknown.ToString();

        [KSConstant("TYPE_PROBE",
            Description = "Value of `vessel.type` if vessel is a space probe."
        )]
        public static readonly string TYPE_PROBE = VesselType.Probe.ToString();

        [KSConstant("TYPE_RELAY",
            Description = "Value of `vessel.type` if vessel is a communication relay satelite."
        )]
        public static readonly string TYPE_RELAY = VesselType.Relay.ToString();

        [KSConstant("TYPE_LANDER",
            Description = "Value of `vessel.type` if vessel is a lander."
        )]
        public static readonly string TYPE_LANDER = VesselType.Lander.ToString();

        [KSConstant("TYPE_SHIP",
            Description = "Value of `vessel.type` if vessel is a space ship."
        )]
        public static readonly string TYPE_SHIP = VesselType.Ship.ToString();

        [KSConstant("TYPE_PLANE",
            Description = "Value of `vessel.type` if vessel is a plane."
        )]
        public static readonly string TYPE_PLANE = VesselType.Plane.ToString();

        [KSConstant("TYPE_BASE",
            Description = "Value of `vessel.type` if vessel is a planetary base."
        )]
        public static readonly string TYPE_BASE = VesselType.Base.ToString();

        [KSConstant("TYPE_EVA",
            Description = "Value of `vessel.type` if vessel is a Kerbal in EVA."
        )]
        public static readonly string TYPE_EVA = VesselType.EVA.ToString();

        [KSConstant("TYPE_FLAG",
            Description = "Value of `vessel.type` if vessel is a flag."
        )]
        public static readonly string TYPE_FLAG = VesselType.Flag.ToString();

        [KSConstant("TYPE_SCIENCE_CONTROLLER",
            Description = "Value of `vessel.type` if vessel is a deployed science controller."
        )]
        public static readonly string TYPE_SCIENCE_CONTROLLER = VesselType.DeployedScienceController.ToString();

        [KSConstant("TYPE_SCIENCE_PART",
            Description = "Value of `vessel.type` if vessel is a deployed science part."
        )]
        public static readonly string TYPE_SCIENCE_PART = VesselType.DeployedSciencePart.ToString();

        [KSConstant("SITUATION_SEALEVEL", Description = "Used for delta-v calculation at sea level of the current body.")]
        public static readonly string SITUATION_SEALEVEL = "SEALEVEL";

        [KSConstant("SITUATION_ALTITUDE", Description = "Used for delta-v calculation at the current altitude.")]
        public static readonly string SITUATION_ALTITUDE = "ALTITUDE";

        [KSConstant("SITUATION_VACUUM", Description = "Used for delta-v calculation in vacuum.")]
        public static readonly string SITUATION_VACUUM = "VACUUM";

        [KSFunction(
            Description = "Try to get the currently active vessel. Will result in an error if there is none."
        )]
        public static Result<VesselAdapter, string> activeVessel() {
            return VesselAdapter.NullSafe(KSPContext.CurrentContext, FlightGlobals.ActiveVessel).OkOr("No active vessel");
        }

        public static void DirectBindings() {
            BindingGenerator.RegisterTypeMapping(typeof(FlightCtrlState), KontrolSystem.KSP.Runtime.KSPVessel.FlightCtrlStateBinding.FlightCtrlStateType);
        }

        internal static DeltaVSituationOptions SituationFromString(string situation) {
            switch (situation.ToUpperInvariant()) {
            case "SEALEVEL": return DeltaVSituationOptions.SeaLevel;
            case "ALTITUDE": return DeltaVSituationOptions.Altitude;
            default: return DeltaVSituationOptions.Vaccum;
            }
        }
    }
}
