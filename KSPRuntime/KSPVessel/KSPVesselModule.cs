using System.Collections.Generic;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    [KSModule("ksp::vessel")]
    public partial class KSPVesselModule {
        IKSPContext context;

        public KSPVesselModule(IContext _context, Dictionary<string, object> modules) => context = _context as IKSPContext;

        [KSFunction(
            Description = "Try to get the currently active vessel. Will result in an error if there is none."
        )]
        public Result<VesselAdapter, string> activeVessel() {
            if (context == null) return Result<VesselAdapter, string>.failure("No runtime context");
            return VesselAdapter.NullSafe(context, FlightGlobals.ActiveVessel).OkOr("No active vessel");
        }

        public static void DirectBindings() {
            BindingGenerator.RegisterTypeMapping(typeof(FlightCtrlState), KontrolSystem.KSP.Runtime.KSPVessel.FlightCtrlStateBinding.FlightCtrlStateType);
        }
    }
}
