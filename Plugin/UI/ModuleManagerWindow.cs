using System.Linq;
using KontrolSystem.Plugin.Config;
using KontrolSystem.Plugin.Core;
using UnityEngine;

namespace KontrolSystem.Plugin.UI {
    public class ModuleManagerWindow : ResizableWindow {
        private Vector2 errorScrollPos;

        public void Toggle() {
            if (!isOpen) Open();
            else Close();
        }

        public void Awake() {
            Initialize("KontrolSystem: ModuleManager", new Rect(50, 50, 400, 400), false);
        }

        protected override void DrawWindow(int windowId) {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            if (GUILayout.Button(Mainframe.Instance.Rebooting ? "Rebooting..." : "Reboot")) OnReboot();
            if (GUILayout.Button("Close")) Close();
            GUILayout.EndVertical();

            errorScrollPos = GUILayout.BeginScrollView(errorScrollPos, GUILayout.MinWidth(300), GUILayout.MaxWidth(700),
                GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical();

            DrawErrors();

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void DrawErrors() {
            GUIStyle style = new GUIStyle();
            style.richText = true;

            if (Mainframe.Instance.Rebooting)
                GUILayout.Label("Rebooteding...");
            else
                GUILayout.Label($"Rebooted in {Mainframe.Instance.LastRebootTime}");
            if (!Mainframe.Instance.LastErrors.Any()) {
                GUILayout.Label($"<color=green>No errors</color>");
                return;
            }

            foreach (MainframeError error in Mainframe.Instance.LastErrors) {
                GUILayout.Label($"<color=red>ERROR: </color> [{error.position}] {error.errorType}");
                GUILayout.Label("    <color=white>" + error.message + "</color>");
            }
        }

        protected override void OnResize(Rect windowRect) {
        }

        void OnReboot() {
            Mainframe.Instance.Reboot(KontrolSystemConfig.Instance);
        }
    }
}
