using System;
using KontrolSystem.TO2;
using KontrolSystem.KSP.Runtime;

namespace KontrolSystem.Plugin.Core {
    public enum KontrolSystemProcessState {
        Available,
        Running,
        Outdated,
        Error,
    }

    public class KontrolSystemProcess {
        private readonly IKontrolModule module;
        private KontrolSystemProcessState state;
        internal KSPContext context;
        public readonly Guid id;

        public KontrolSystemProcess(IKontrolModule module) {
            this.module = module;
            state = KontrolSystemProcessState.Available;
            id = Guid.NewGuid();
        }

        public string Name => module.Name;

        public KontrolSystemProcessState State => state;


        public void MarkRunning(KSPContext newContext) {
            state = KontrolSystemProcessState.Running;
            context?.Cleanup();
            context = newContext;
        }

        public void MarkDone(string message) {
            if (!string.IsNullOrEmpty(message)) {
                PluginLogger.Instance.Info($"Process {id} for module {module.Name} terminated with: {message}");
                ScreenMessages.PostScreenMessage(
                    $"<color=red><size=20>Module {module.Name} failed: {message}</size></color>", 5,
                    ScreenMessageStyle.UPPER_CENTER);
            }

            state = KontrolSystemProcessState.Available;
            context?.Cleanup();
            context = null;
        }

        public Entrypoint EntrypointFor(GameScenes gameScene, IKSPContext context) {
            switch (gameScene) {
            case GameScenes.SPACECENTER: return module.GetKSCEntrypoint(context);
            case GameScenes.EDITOR: return module.GetEditorEntrypoint(context);
            case GameScenes.TRACKSTATION: return module.GetKSCEntrypoint(context);
            case GameScenes.FLIGHT: return module.GetFlightEntrypoint(context);
            default:
                return null;
            }
        }

        public bool AvailableFor(GameScenes gameScene) {
            switch (gameScene) {
            case GameScenes.SPACECENTER: return module.HasKSCEntrypoint();
            case GameScenes.EDITOR: return module.HasEditorEntrypoint();
            case GameScenes.TRACKSTATION: return module.HasTrackingEntrypoint();
            case GameScenes.FLIGHT: return module.HasFlightEntrypoint();
            default:
                return false;
            }
        }
    }
}
