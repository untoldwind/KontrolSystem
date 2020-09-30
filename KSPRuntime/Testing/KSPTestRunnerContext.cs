using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.Testing {
    public class KSPTestRunnerContext : TestRunnerContext, IKSPContext {
        private KSPConsoleBuffer consoleBuffer = new KSPConsoleBuffer(50, 80);
        private object nextYield;

        public GameScenes CurrentScene => GameScenes.FLIGHT;

        public KSPConsoleBuffer ConsoleBuffer => consoleBuffer;

        public IEnumerable<KSPOrbitModule.IBody> Bodies => new List<KSPOrbitModule.IBody> {
            MockBody.Kerbol,
            MockBody.Eve,
            MockBody.Gilly,
            MockBody.Kerbin,
            MockBody.Mun,
            MockBody.Minmus,
            MockBody.Duna,
            MockBody.Ike,
            MockBody.Jool,
            MockBody.Tylo,
            MockBody.Vall,
            MockBody.Pol
        };

        public object NextYield {
            get {
                object result = nextYield;
                nextYield = new WaitForFixedUpdate();
                return result;
            }
            set { nextYield = value; }
        }

        public void AddMarker(IMarker marker) {
        }

        public void ClearMarkers() {
        }

        public void AddFixedUpdateObserver(WeakReference<IFixedUpdateObserver> observer) {
        }

        public void HookAutopilot(Vessel vessel, FlightInputCallback autopilot) {
        }

        public void UnhookAutopilot(Vessel vessel, FlightInputCallback autopilot) {
        }

        public void UnhookAllAutopilots(Vessel vessel) {
        }
    }
}
