using System;
using UnityEngine;

namespace KontrolSystem.Plugin.UI.Adapter {
    public class UIWindowAdapter : ResizableWindow {
        private Action render = () => { };

        internal void AttachTo(string initialTitle, Action newRender, bool initialClosed) {
            Initialize(initialTitle, new Rect((float) Screen.width / 2, (float) Screen.height / 2, 50, 50), false);
            render = newRender;
            isOpen = !initialClosed;
            if (!isOpen) {
                Destroy(gameObject);
            }
        }

        public override void Close() {
            if (isOpen) {
                isOpen = false;
                Destroy(gameObject);
            }
        }

        protected override void DrawWindow(int windowId) => render();

        protected override void OnResize(Rect newWindowRect) {
        }
    }
}
