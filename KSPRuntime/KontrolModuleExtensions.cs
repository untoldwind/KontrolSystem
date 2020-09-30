using System;
using KontrolSystem.TO2;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime {
    public delegate IAnyFuture Entrypoint();

    public static class KontrolModuleExtensions {
        public const string MAIN_KSC = "main_ksc";
        public const string MAIN_EDITOR = "main_editor";
        public const string MAIN_TRACKING = "main_tracking";
        public const string MAIN_FLIGHT = "main_flight";

        private static Entrypoint GetEntrypoint(IKontrolModule module, string name, IKSPContext context) {
            try {
                IKontrolFunction function = module.FindFunction(name);
                if (function == null && function.IsAsync) return null;
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

        public static bool HasKSCEntrypoint(this IKontrolModule module) => HasEntrypoint(module, MAIN_KSC);

        public static Entrypoint GetKSCEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MAIN_KSC, context);

        public static bool HasEditorEntrypoint(this IKontrolModule module) => HasEntrypoint(module, MAIN_EDITOR);

        public static Entrypoint GetEditorEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MAIN_EDITOR, context);

        public static bool HasTrackingEntrypoint(this IKontrolModule module) => HasEntrypoint(module, MAIN_TRACKING);

        public static Entrypoint GetTrackingEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MAIN_TRACKING, context);

        public static bool HasFlightEntrypoint(this IKontrolModule module) => HasEntrypoint(module, MAIN_FLIGHT);

        public static Entrypoint GetFlightEntrypoint(this IKontrolModule module, IKSPContext context) =>
            GetEntrypoint(module, MAIN_FLIGHT, context);
    }
}
