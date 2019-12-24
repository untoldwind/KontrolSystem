using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Plugin.Config;
using KontrolSystem.Parsing;
using KontrolSystem.TO2;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.Plugin.Utils;
using UnityEngine;

namespace KontrolSystem.Plugin.Core {
    public struct MainframeError {
        public readonly Position position;
        public readonly string errorType;
        public readonly string message;

        public MainframeError(Position _position, string _errorType, string _message) {
            position = _position;
            errorType = _errorType;
            message = _message;
        }
    }

    public class Mainframe : Singleton<Mainframe> {
        static char[] PathSeparator = new char[] { '\\', '/' };

        KontrolRegistry registry = null;

        List<MainframeError> lastErrors = null;

        List<KontrolSystemProcess> processes;

        KSPConsoleBuffer consoleBuffer = new KSPConsoleBuffer(40, 50);

        public bool Initialized => registry != null;

        public KSPConsoleBuffer ConsoleBuffer => consoleBuffer;

        public IEnumerable<MainframeError> LastErrors => lastErrors ?? Enumerable.Empty<MainframeError>();
        public Dictionary<Guid, Coroutine> coroutines = new Dictionary<Guid, Coroutine>();

        public void Awake() {
            GameEvents.onGameSceneSwitchRequested.Add(OnSceneChange);
            GameEvents.onGameStatePostLoad.Add(OnGameStatePostLoad);
        }

        public void Destroy() {
            GameEvents.onGameSceneSwitchRequested.Remove(OnSceneChange);
            GameEvents.onGameStatePostLoad.Remove(OnGameStatePostLoad);
        }

        public void Update() {
            if (processes == null) return;
            foreach (KontrolSystemProcess process in processes) {
                switch (process.State) {
                case KontrolSystemProcessState.Outdated:
                case KontrolSystemProcessState.Running:
                    foreach (IMarker marker in process.context?.markers) marker.Update();
                    break;
                default:
                    break;
                }
            }
        }

        public void FixedUpdate() {
            if (processes == null) return;
            foreach (KontrolSystemProcess process in processes) {
                switch (process.State) {
                case KontrolSystemProcessState.Outdated:
                case KontrolSystemProcessState.Running:
                    process.context?.TriggerFixedUpdateObservers();
                    break;
                default:
                    break;
                }
            }
        }

        public void OnPostRender() {
            if (processes == null) return;
            foreach (KontrolSystemProcess process in processes) {
                switch (process.State) {
                case KontrolSystemProcessState.Outdated:
                case KontrolSystemProcessState.Running:
                    foreach (IMarker marker in process.context?.markers) marker.OnRender();
                    break;
                default:
                    break;
                }
            }
        }

        public void Reboot(KontrolSystemConfig config) {
            try {
                string registryPath = Path.GetFullPath(config.TO2BaseDir).TrimEnd(PathSeparator);

                PluginLogger.Instance.Info($"Rebooting Mainframe on path {registryPath}");

                Directory.CreateDirectory(registryPath);

                KontrolRegistry nextRegistry = KontrolSystemKSPRegistry.CreateKSP();

                if (KontrolSystemConfig.Instance.IncludeStdLib) {
                    PluginLogger.Instance.Debug($"Add Directory: " + KontrolSystemConfig.Instance.StdLibDir);
                    nextRegistry.AddDirectory(KontrolSystemConfig.Instance.StdLibDir);
                }
                PluginLogger.Instance.Debug($"Add Directory: " + registryPath);
                nextRegistry.AddDirectory(registryPath);

                lastErrors = null;
                registry = nextRegistry;
                processes = registry.modules.Values.Where(module => module.HasKSCEntrypoint() || module.HasEditorEntrypoint() || module.HasTrackingEntrypoint() || module.HasFlightEntrypoint()).Select(module => new KontrolSystemProcess(module)).ToList();
            } catch (CompilationErrorException e) {
                foreach (StructuralError error in e.errors) {
                    PluginLogger.Instance.Info(error.ToString());
                }

                lastErrors = e.errors.Select(error => new MainframeError(
                    error.start,
                    error.errorType.ToString(),
                    error.message
                )).ToList();
            } catch (ParseException e) {
                PluginLogger.Instance.Info(e.Message);

                lastErrors = new List<MainframeError> {
                    new MainframeError(e.position, "Parsing", e.Message)
                };
            } catch (Exception e) {
                PluginLogger.Instance.Error("Mainframe initialization error: " + e);
            }
        }

        public IEnumerable<KontrolSystemProcess> ListProcesses() {
            GameScenes current = HighLogic.LoadedScene;

            return processes != null ? processes.Where(p => p.AvailableFor(current)) : Enumerable.Empty<KontrolSystemProcess>();
        }

        public bool StartProcess(KontrolSystemProcess process) {
            switch (process.State) {
            case KontrolSystemProcessState.Available:
                KSPContext context = new KSPContext(consoleBuffer);
                Entrypoint entrypoint = process.EntrypointFor(HighLogic.LoadedScene, context);
                if (entrypoint == null) return false;
                CorouttineAdapter adapter = new CorouttineAdapter(entrypoint(), context, process.MarkDone);
                process.MarkRunning(context);

                Coroutine coroutine = StartCoroutine(adapter);

                if (coroutines.ContainsKey(process.id)) {
                    StopCoroutine(coroutines[process.id]);
                    coroutines[process.id] = coroutine;
                } else {
                    coroutines.Add(process.id, coroutine);
                }

                return true;
            default:
                return false;
            }
        }

        public bool StopProcess(KontrolSystemProcess process) {
            switch (process.State) {
            case KontrolSystemProcessState.Running:
            case KontrolSystemProcessState.Outdated:
                if (coroutines.ContainsKey(process.id)) {
                    StopCoroutine(coroutines[process.id]);
                    process.MarkDone("Aborted by pilot");
                    coroutines.Remove(process.id);
                }
                return true;
            default:
                return false;
            }
        }

        public void StopAll() {
            if (processes == null) return;
            foreach (KontrolSystemProcess process in processes) {
                if (coroutines.ContainsKey(process.id)) {
                    StopCoroutine(coroutines[process.id]);
                    process.MarkDone("Aborted by pilot");
                    coroutines.Remove(process.id);
                }
            }
        }

        public void RunModule(IKontrolModule module) {
            KSPContext context = new KSPContext(consoleBuffer);
            Entrypoint entrypoint = FindEntrypoint(module, context);

            if (entrypoint == null) {
                PluginLogger.Instance.Error($"Failed to get entrypoint for {module.Name} in game scene {HighLogic.LoadedScene}");
                return;
            }
            entrypoint();
        }

        Entrypoint FindEntrypoint(IKontrolModule module, IKSPContext context) {
            switch (HighLogic.LoadedScene) {
            case GameScenes.SPACECENTER:
                return module.GetKSCEntrypoint(context);
            case GameScenes.EDITOR:
                return module.GetEditorEntrypoint(context);
            case GameScenes.TRACKSTATION:
                return module.GetTrackingEntrypoint(context);
            case GameScenes.FLIGHT:
                return module.GetFlightEntrypoint(context);
            default:
                return null;
            }
        }

        private void OnSceneChange(GameEvents.FromToAction<GameScenes, GameScenes> action) {

        }

        private void OnGameStatePostLoad(ConfigNode config) {
            StopAll();
        }
    }
}
