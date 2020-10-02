using System;
using System.Collections.Generic;
using KontrolSystem.KSP.Runtime.KSPConsole;
using KramaxReloadExtensions;
using KSP.UI.Dialogs;
using UnityEngine;

namespace KontrolSystem.Plugin.UI {
    public abstract class ResizableWindow : ReloadableMonoBehaviour {
        protected Texture2D resizeButtonImage;
        protected int objectId;
        protected bool isOpen = false;
        protected Rect windowRect;
        protected bool mouseDown;

        public void Open() {
            isOpen = true;
        }

        public void Close() {
            isOpen = false;
        }

        protected void Initialize(Rect windowRect) {
            objectId = GetInstanceID();

            resizeButtonImage = GameDatabase.Instance.GetTexture("KontrolSystem/GFX/dds_resize-button", false);
            this.windowRect = windowRect;
        }

        public void OnGUI() {
            if (!isOpen) return;

            GUI.skin = HighLogic.Skin;

            windowRect = GUI.Window(objectId, windowRect, DrawWindowOuter, "KontrolSystem: Console");
        }

        private void DrawWindowOuter(int windowId) {
            Rect resizeButtonCoords = new Rect(windowRect.width - resizeButtonImage.width + 2,
                windowRect.height - resizeButtonImage.height,
                resizeButtonImage.width,
                resizeButtonImage.height);
            GUI.Label(resizeButtonCoords, resizeButtonImage);

            DrawWindow(windowId);

            HandleResizeEvents(resizeButtonCoords);

            GUI.DragWindow();
        }

        protected abstract void DrawWindow(int windowId);

        protected void HandleResizeEvents(Rect resizeRect) {
            Event theEvent = Event.current;
            if (theEvent == null) return;

            if (!mouseDown) {
                if (theEvent.type == EventType.MouseDown && theEvent.button == 0 &&
                    resizeRect.Contains(theEvent.mousePosition)) {
                    mouseDown = true;
                    theEvent.Use();
                }
            } else if (theEvent.type != EventType.Layout) {
                if (Input.GetMouseButton(0)) {
                    // Flip the mouse Y so that 0 is at the top
                    float mouseY = Screen.height - Input.mousePosition.y;

                    windowRect.width = Mathf.Clamp(Input.mousePosition.x - windowRect.x + (resizeRect.width / 2), 50,
                        Screen.width - windowRect.x);
                    windowRect.height = Mathf.Clamp(mouseY - windowRect.y + (resizeRect.height / 2), 50,
                        Screen.height - windowRect.y);
                } else {
                    mouseDown = false;
                }

                OnResize(windowRect);
            }
        }

        protected abstract void OnResize(Rect windowRect);
    }
}
