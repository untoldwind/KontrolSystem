using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.KSP.Runtime.KSPUI;

namespace KontrolSystem.KSP.Runtime {
    public interface IMarker {
        bool Visible { get; set; }
        void Update();
        void OnRender();
    }

    public interface IFixedUpdateObserver {
        void OnFixedUpdate();
    }

    public interface IKSPContext : IContext {
        GameScenes CurrentScene { get; }

        KSPConsoleBuffer ConsoleBuffer { get; }

        IEnumerable<KSPOrbitModule.IBody> Bodies { get; }

        object NextYield { get; set; }

        void AddMarker(IMarker marker);

        void ClearMarkers();

        void AddFixedUpdateObserver(WeakReference<IFixedUpdateObserver> observer);

        void HookAutopilot(Vessel vessel, FlightInputCallback autopilot);

        void UnhookAutopilot(Vessel vessel, FlightInputCallback autopilot);

        void UnhookAllAutopilots(Vessel vessel);

        KSPUIModule.IWindow<T> ShowWindow<T>(T initialState, Func<T, bool> isEndState,
            Action<KSPUIModule.IContainer<T>, T> render);
    }

    public class KSPContext {
        public static IKSPContext CurrentContext {
            get {
                IKSPContext context = ContextHolder.CurrentContext.Value as IKSPContext;
                if (context == null) throw new ArgumentException($"No current IKSPContext");
                return context;
            }
        }
    }
}
