using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.Plugin.Core {
    internal class AutopilotHooks {
        internal readonly IKSPContext context;
        internal readonly List<FlightInputCallback> autopilots = new List<FlightInputCallback>();

        internal AutopilotHooks(IKSPContext _context) => context = _context;

        internal void Add(FlightInputCallback autopilot) {
            if (!autopilots.Contains(autopilot)) autopilots.Add(autopilot);
        }

        internal void Remove(FlightInputCallback autopilot) => autopilots.Remove(autopilot);

        internal bool IsEmpty => autopilots.Count == 0;

        internal void RunAutopilots(FlightCtrlState state) {
            try {
                ContextHolder.CurrentContext.Value = context;
                foreach (FlightInputCallback autopilot in autopilots)
                    autopilot(state);
            } finally {
                ContextHolder.CurrentContext.Value = null;
            }
        }
    }

    public class KSPContext : IKSPContext {
        private readonly KSPConsoleBuffer consoleBuffer;
        private object nextYield;
        private long loopCounter;
        private Stopwatch timeStopwatch;
        private long timeoutMillis;
        internal readonly List<IMarker> markers;
        internal readonly List<WeakReference<IFixedUpdateObserver>> fixedUpdateObservers;
        internal readonly Dictionary<Vessel, AutopilotHooks> autopilotHooks;

        public KSPContext(KSPConsoleBuffer _consoleBuffer) {
            consoleBuffer = _consoleBuffer;
            markers = new List<IMarker>();
            fixedUpdateObservers = new List<WeakReference<IFixedUpdateObserver>>();
            nextYield = new WaitForFixedUpdate();
            autopilotHooks = new Dictionary<Vessel, AutopilotHooks>();
            loopCounter = 0;
            timeStopwatch = Stopwatch.StartNew();
            timeoutMillis = 100;
        }

        public ITO2Logger Logger => PluginLogger.Instance;

        public void CheckTimeout() {
            loopCounter++;
            if (loopCounter > 10000) {
                loopCounter = 0;
                long elapsed = timeStopwatch.ElapsedMilliseconds;
                if (elapsed >= timeoutMillis)
                    throw new YieldTimeoutException(elapsed);
            }
        }

        public GameScenes CurrentScene => HighLogic.LoadedScene;

        public KSPConsoleBuffer ConsoleBuffer => consoleBuffer;

        public IEnumerable<KSPOrbitModule.IBody> Bodies => FlightGlobals.Bodies.Select(body => new BodyWrapper(body));

        public object NextYield {
            get {
                object result = nextYield;
                nextYield = new WaitForFixedUpdate();
                return result;
            }
            set {
                nextYield = value;
            }
        }

        public void ResetTimeout() {
            loopCounter = 0;
            timeStopwatch.Reset();
            timeStopwatch.Start();
        }

        public void AddMarker(IMarker marker) => markers.Add(marker);

        public void ClearMarkers() {
            foreach (IMarker marker in markers) marker.Visible = false;
            markers.Clear();
        }

        public void AddFixedUpdateObserver(WeakReference<IFixedUpdateObserver> observer) => fixedUpdateObservers.Add(observer);

        internal void TriggerFixedUpdateObservers() {
            try {
                ContextHolder.CurrentContext.Value = this;
                for (int i = fixedUpdateObservers.Count - 1; i >= 0; i--) {
                    IFixedUpdateObserver observer;
                    if (fixedUpdateObservers[i].TryGetTarget(out observer))
                        observer.OnFixedUpdate();
                    else
                        fixedUpdateObservers.RemoveAt(i);
                }
            } finally {
                ContextHolder.CurrentContext.Value = null;
            }
        }

        public void HookAutopilot(Vessel vessel, FlightInputCallback autopilot) {
            PluginLogger.Instance.Debug($"Hook autopilot {autopilot} to {vessel.name}");
            if (autopilotHooks.ContainsKey(vessel)) {
                autopilotHooks[vessel].Add(autopilot);
            } else {
                AutopilotHooks autopilots = new AutopilotHooks(this);
                autopilots.Add(autopilot);
                autopilotHooks.Add(vessel, autopilots);

                PluginLogger.Instance.Debug($"Hooking up for vessel: {vessel.name}");
                // Ensure that duplicates do no trigger an exception
                vessel.OnPreAutopilotUpdate -= autopilots.RunAutopilots;
                vessel.OnPreAutopilotUpdate += autopilots.RunAutopilots;
            }
        }

        public void UnhookAutopilot(Vessel vessel, FlightInputCallback autopilot) {
            PluginLogger.Instance.Debug($"Unhook autopilot {autopilot} to {vessel.name}");
            if (!autopilotHooks.ContainsKey(vessel)) return;

            AutopilotHooks autopilots = autopilotHooks[vessel];

            autopilots.Remove(autopilot);
            if (autopilots.IsEmpty) {
                PluginLogger.Instance.Debug($"Unhooking from vessel: {vessel.name}");
                autopilotHooks.Remove(vessel);
                vessel.OnPreAutopilotUpdate -= autopilots.RunAutopilots;
            }
        }

        public void UnhookAllAutopilots(Vessel vessel) {
            if (!autopilotHooks.ContainsKey(vessel)) return;

            AutopilotHooks autopilots = autopilotHooks[vessel];

            autopilotHooks.Remove(vessel);
            PluginLogger.Instance.Debug($"Unhooking from vessel: {vessel.name}");
            vessel.OnPreAutopilotUpdate -= autopilots.RunAutopilots;
        }

        public void Cleanup() {
            ClearMarkers();
            foreach (var kv in autopilotHooks) {
                PluginLogger.Instance.Debug($"Unhooking from vessel: {kv.Key.name}");
                kv.Key.OnPreAutopilotUpdate -= kv.Value.RunAutopilots;
            }
            autopilotHooks.Clear();
        }
    }
}
