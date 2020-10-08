using System;
using KontrolSystem.TO2;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime {
    public delegate IAnyFuture Entrypoint();

    public static class KontrolModuleExtensions {
        private const string MainKsc = "main_ksc";
        private const string MainEditor = "main_editor";
        private const string MainTracking = "main_tracking";
        private const string MainFlight = "main_flight";
        private const string Boot = "boot";

        private static Entrypoint GetEntrypoint(IKontrolModule module, string name, IKSPContext context) {
            try {
                IKontrolFunction function = module.FindFunction(name);
                if (function == null || function.IsAsync) return null;
                return function.RuntimeMethod.CreateDelegate(typeof(Entrypoint)) as Entrypoint;
            } catch (Exception e) {
                context.Logger.Error($"GetEntrypoint {name} failed: {e}");
                return null;
            }
        }

        private static bool HasEntrypoint(IKontrolModule module, string name) {
            IKontrolFunction function = module.FindFunction(name);
            return function != null && function.IsAsync;
        }

        public static bool HasKSCEntrypoint(this IKontrolModule module) => HasEntrypoint(module, MainKsc);

        public static Entrypoint GetKSCEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MainKsc, context);

        public static bool HasEditorEntrypoint(this IKontrolModule module) => HasEntrypoint(module, MainEditor);

        public static Entrypoint GetEditorEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MainEditor, context);

        public static bool HasTrackingEntrypoint(this IKontrolModule module) => HasEntrypoint(module, MainTracking);

        public static Entrypoint GetTrackingEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MainTracking, context);

        public static bool HasFlightEntrypoint(this IKontrolModule module) => HasEntrypoint(module, MainFlight);

        public static Entrypoint GetFlightEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MainFlight, context);

        public static bool HasVesselBootEntrypoint(this IKontrolModule module) =>
            module.Name.StartsWith("boot::") && HasEntrypoint(module, Boot);

        public static Entrypoint GetVesselBootEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, Boot, context);
    }
}
