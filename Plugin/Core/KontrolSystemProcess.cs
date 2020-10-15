using System;
using KontrolSystem.TO2;
using KontrolSystem.KSP.Runtime;
using UnityEngine;

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

        public KontrolSystemProcess MarkOutdated() {
            state = KontrolSystemProcessState.Outdated;
            return this;
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

        public Entrypoint EntrypointFor(GameScenes gameScene, IKSPContext newContext) {
            switch (gameScene) {
            case GameScenes.SPACECENTER: return module.GetKSCEntrypoint(newContext);
            case GameScenes.EDITOR:
                return module.GetEditorEntrypoint(newContext);
            case GameScenes.TRACKSTATION: return module.GetTrackingEntrypoint(newContext);
            case GameScenes.FLIGHT:
                return module.GetFlightEntrypoint(newContext);
            default:
                return null;
            }
        }

        public bool AvailableFor(GameScenes gameScene, Vessel vessel) {
            switch (gameScene) {
            case GameScenes.SPACECENTER: return module.HasKSCEntrypoint();
            case GameScenes.EDITOR: return module.HasEditorEntrypoint();
            case GameScenes.TRACKSTATION: return module.HasTrackingEntrypoint();
            case GameScenes.FLIGHT:
                return !module.Name.StartsWith("boot::") && module.HasFlightEntrypoint() ||
                       module.IsBootFlightEntrypointFor(vessel);
            default:
                return false;
            }
        }

        public bool IsBootFor(GameScenes gameScene, Vessel vessel) {
            switch (gameScene) {
            case GameScenes.FLIGHT: return module.IsBootFlightEntrypointFor(vessel);
            default: return false;
            }
        }
    }
}
