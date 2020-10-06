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
        public static readonly string TypeDebirs = VesselType.Debris.ToString();

        [KSConstant("TYPE_SPACEOBJECT",
            Description = "Value of `vessel.type` if vessel is some unspecified space object."
        )]
        public static readonly string TypeSpaceobject = VesselType.SpaceObject.ToString();

        [KSConstant("TYPE_UNKOWN",
            Description = "Value of `vessel.type` if the type of the vessel is unknown/undefined."
        )]
        public static readonly string TypeUnkown = VesselType.Unknown.ToString();

        [KSConstant("TYPE_PROBE",
            Description = "Value of `vessel.type` if vessel is a space probe."
        )]
        public static readonly string TypeProbe = VesselType.Probe.ToString();

        [KSConstant("TYPE_RELAY",
            Description = "Value of `vessel.type` if vessel is a communication relay satelite."
        )]
        public static readonly string TypeRelay = VesselType.Relay.ToString();

        [KSConstant("TYPE_LANDER",
            Description = "Value of `vessel.type` if vessel is a lander."
        )]
        public static readonly string TypeLander = VesselType.Lander.ToString();

        [KSConstant("TYPE_SHIP",
            Description = "Value of `vessel.type` if vessel is a space ship."
        )]
        public static readonly string TypeShip = VesselType.Ship.ToString();

        [KSConstant("TYPE_PLANE",
            Description = "Value of `vessel.type` if vessel is a plane."
        )]
        public static readonly string TypePlane = VesselType.Plane.ToString();

        [KSConstant("TYPE_BASE",
            Description = "Value of `vessel.type` if vessel is a planetary base."
        )]
        public static readonly string TypeBase = VesselType.Base.ToString();

        [KSConstant("TYPE_EVA",
            Description = "Value of `vessel.type` if vessel is a Kerbal in EVA."
        )]
        public static readonly string TypeEva = VesselType.EVA.ToString();

        [KSConstant("TYPE_FLAG",
            Description = "Value of `vessel.type` if vessel is a flag."
        )]
        public static readonly string TypeFlag = VesselType.Flag.ToString();

        [KSConstant("TYPE_SCIENCE_CONTROLLER",
            Description = "Value of `vessel.type` if vessel is a deployed science controller."
        )]
        public static readonly string TypeScienceController = VesselType.DeployedScienceController.ToString();

        [KSConstant("TYPE_SCIENCE_PART",
            Description = "Value of `vessel.type` if vessel is a deployed science part."
        )]
        public static readonly string TypeSciencePart = VesselType.DeployedSciencePart.ToString();

        [KSConstant("SITUATION_SEALEVEL",
            Description = "Used for delta-v calculation at sea level of the current body.")]
        public static readonly string SituationSealevel = "SEALEVEL";

        [KSConstant("SITUATION_ALTITUDE", Description = "Used for delta-v calculation at the current altitude.")]
        public static readonly string SituationAltitude = "ALTITUDE";

        [KSConstant("SITUATION_VACUUM", Description = "Used for delta-v calculation in vacuum.")]
        public static readonly string SituationVacuum = "VACUUM";

        [KSFunction(
            Description = "Try to get the currently active vessel. Will result in an error if there is none."
        )]
        public static Result<VesselAdapter, string> ActiveVessel() {
            return VesselAdapter.NullSafe(KSPContext.CurrentContext, FlightGlobals.ActiveVessel)
                .OkOr("No active vessel");
        }

        public static void DirectBindings() {
            BindingGenerator.RegisterTypeMapping(typeof(FlightCtrlState),
                FlightCtrlStateBinding.FlightCtrlStateType);
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
