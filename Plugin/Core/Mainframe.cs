using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using KontrolSystem.Plugin.Config;
using KontrolSystem.Parsing;
using KontrolSystem.TO2;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.Plugin.Utils;
using UnityEngine;

namespace KontrolSystem.Plugin.Core {
    public readonly struct MainframeError {
        public readonly Position position;
        public readonly string errorType;
        public readonly string message;

        public MainframeError(Position position, string errorType, string message) {
            this.position = position;
            this.errorType = errorType;
            this.message = message;
        }
    }

    public class Mainframe : Singleton<Mainframe> {
        static readonly char[] PathSeparator = {'\\', '/'};

        volatile State state;
        volatile bool rebooting;

        List<KontrolSystemProcess> processes;

        readonly KSPConsoleBuffer consoleBuffer = new KSPConsoleBuffer(40, 50);

        public bool Initialized => state != null;

        public KSPConsoleBuffer ConsoleBuffer => consoleBuffer;

        public bool Rebooting => rebooting;
        public TimeSpan LastRebootTime => state?.bootTime ?? TimeSpan.Zero;
        public IEnumerable<MainframeError> LastErrors => state?.errors ?? Enumerable.Empty<MainframeError>();
        private readonly Dictionary<Guid, Coroutine> coroutines = new Dictionary<Guid, Coroutine>();

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
                    if (process.context?.markers != null)
                        foreach (IMarker marker in process.context?.markers)
                            marker.Update();
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
                }
            }
        }

        public void OnGUI() {
            if (processes == null) return;
            foreach (KontrolSystemProcess process in processes) {
                switch (process.State) {
                case KontrolSystemProcessState.Outdated:
                case KontrolSystemProcessState.Running:
                    if (process.context?.markers != null)
                        foreach (IMarker marker in process.context?.markers)
                            marker.OnRender();
                    break;
                }
            }
        }

        public void Reboot(KontrolSystemConfig config) {
            if (rebooting) return;
            DoReboot(config);
        }

        private async void DoReboot(KontrolSystemConfig config) {
            rebooting = true;
            State nextState = await Task.Run(() => {
                Stopwatch stopwatch = new Stopwatch();
                try {
                    stopwatch.Start();
                    string registryPath = Path.GetFullPath(config.TO2BaseDir).TrimEnd(PathSeparator);

                    PluginLogger.Instance.Info($"Rebooting Mainframe on path {registryPath}");

                    Directory.CreateDirectory(registryPath);

                    KontrolRegistry nextRegistry = KontrolSystemKSPRegistry.CreateKSP();

                    if (KontrolSystemConfig.Instance.IncludeStdLib) {
                        PluginLogger.Instance.Debug($"Add Directory: {KontrolSystemConfig.Instance.StdLibDir}");
                        nextRegistry.AddDirectory(KontrolSystemConfig.Instance.StdLibDir);
                    }

                    PluginLogger.Instance.Debug($"Add Directory: {registryPath}");
                    nextRegistry.AddDirectory(registryPath);
                    stopwatch.Stop();

                    return new State(nextRegistry, stopwatch.Elapsed, new List<MainframeError>());
                } catch (CompilationErrorException e) {
                    foreach (StructuralError error in e.errors) {
                        PluginLogger.Instance.Info(error.ToString());
                    }

                    return new State(state?.registry, stopwatch.Elapsed, e.errors.Select(error => new MainframeError(
                        error.start,
                        error.errorType.ToString(),
                        error.message
                    )).ToList());
                } catch (ParseException e) {
                    PluginLogger.Instance.Info(e.Message);

                    return new State(state?.registry, stopwatch.Elapsed, new List<MainframeError> {
                        new MainframeError(e.position, "Parsing", e.Message)
                    });
                } catch (Exception e) {
                    PluginLogger.Instance.Error("Mainframe initialization error: " + e);

                    return new State(state?.registry, stopwatch.Elapsed, new List<MainframeError> {
                        new MainframeError(new Position(), "Unknown error", e.Message)
                    });
                }
            });
            if (nextState.errors.Count == 0) {
                processes = (processes ?? Enumerable.Empty<KontrolSystemProcess>())
                    .Where(p => p.State == KontrolSystemProcessState.Running ||
                                p.State == KontrolSystemProcessState.Outdated).Select(p => p.MarkOutdated()).Concat(
                        nextState.registry.modules.Values
                            .Where(module =>
                                module.HasKSCEntrypoint() || module.HasEditorEntrypoint() ||
                                module.HasTrackingEntrypoint() ||
                                module.HasFlightEntrypoint())
                            .Select(module => new KontrolSystemProcess(module))).ToList();
            }

            state = nextState;
            rebooting = false;
        }

        public IEnumerable<KontrolSystemProcess> ListProcesses() {
            GameScenes current = HighLogic.LoadedScene;

            return processes != null
                ? processes.Where(p => p.AvailableFor(current, FlightGlobals.ActiveVessel))
                : Enumerable.Empty<KontrolSystemProcess>();
        }

        public bool StartProcess(KontrolSystemProcess process, Vessel vessel) {
            switch (process.State) {
            case KontrolSystemProcessState.Available:
                KSPContext context = new KSPContext(consoleBuffer);
                Entrypoint entrypoint = process.EntrypointFor(HighLogic.LoadedScene, context);
                if (entrypoint == null) return false;
                CorouttineAdapter adapter = new CorouttineAdapter(entrypoint(vessel), context,
                    message => OnProcessDone(process, message));
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

        public void TriggerBoot(Vessel vessel) {
            GameScenes current = HighLogic.LoadedScene;

            KontrolSystemProcess bootProcess = processes?.FirstOrDefault(p => p.IsBootFor(current, vessel));

            if (bootProcess?.State != KontrolSystemProcessState.Available) return;

            StartProcess(bootProcess, vessel);
        }

        public bool StopProcess(KontrolSystemProcess process) {
            switch (process.State) {
            case KontrolSystemProcessState.Running:
            case KontrolSystemProcessState.Outdated:
                if (coroutines.ContainsKey(process.id)) {
                    StopCoroutine(coroutines[process.id]);
                    OnProcessDone(process, "Aborted by pilot");
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
                    OnProcessDone(process, "Aborted by pilot");
                }
            }
        }

        private void OnProcessDone(KontrolSystemProcess process, string message) {
            if (process.State == KontrolSystemProcessState.Outdated) {
                processes.Remove(process);
            }

            process.MarkDone(message);
            coroutines.Remove(process.id);
        }

        private void OnSceneChange(GameEvents.FromToAction<GameScenes, GameScenes> action) {
        }

        private void OnGameStatePostLoad(ConfigNode config) {
            StopAll();
        }
    }
}
