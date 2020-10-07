using System;
using KontrolSystem.KSP.Runtime.KSPUI;
using UnityEngine;

namespace KontrolSystem.Plugin.UI.Adapter {
    public class UIWindowHandle<T> : KSPUIModule.IWindowHandle<T> {
        private readonly object handleLock;
        private T state;
        private readonly Func<T, bool> isEndState;
        private readonly Action<KSPUIModule.IWindow<T>, T> render;
        private UIWindow<T> window;
        private readonly UIWindowAdapter adapter;

        public UIWindowHandle(UIWindowAdapter adapter, T initialState, Func<T, bool> initialIsEndState,
            Action<KSPUIModule.IWindow<T>, T> initialRender) {
            this.adapter = adapter;
            state = initialState;
            isEndState = initialIsEndState;
            render = initialRender;
            handleLock = new object();
            window = new UIWindow<T>();
            render(window, state);

            adapter.AttachTo(window.Title, DrawWindow, isEndState(state));
        }


        public T State {
            get {
                lock (handleLock) {
                    return state;
                }
            }
            set {
                lock (handleLock) {
                    state = value;
                }
            }
        }

        public bool Closed {
            get {
                lock (handleLock) {
                    return !adapter.IsOpen;
                }
            }
        }

        public void Close() {
            lock (handleLock) {
                adapter.Close();
            }
        }

        protected void DrawWindow() {
            var (nextState, changed) = window.Draw(state);

            if (changed) {
                lock (handleLock) {
                    state = nextState;
                    if (isEndState(state)) {
                        adapter.Close();
                    }

                    window = new UIWindow<T>();
                    render(window, state);
                    adapter.Title = window.Title;
                }
            }
        }
    }
}
