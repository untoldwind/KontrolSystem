using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using KontrolSystem.Plugin.Config;
using KontrolSystem.Plugin.Core;
using KontrolSystem.TO2;

namespace KontrolSystem.Plugin.UI {
    /// <summary>
    /// The window that popups on hover or click of the toolbar button.
    /// Usually this would be a MonoBehaviour, in this case though we just forward the OnGUI
    /// from the ToolbarButton itself.
    /// I.e. this class is just encapsulates all the window related code so that ToolbarButton does
    /// not become too bloated.
    /// </summary>
    public class ToolbarWindow {
        private readonly int objectId;
        private readonly string windowTitle;
        private Rect rectToFit;
        private Rect windowRect;
        private readonly CommonStyles commonStyles;
        private Vector2 scrollPos = new Vector2(200, 350);
        private readonly ConsoleWindow consoleWindow;
        private readonly ModuleManagerWindow moduleManagerWindow;

        public ToolbarWindow(int objectId, CommonStyles commonStyles, ConsoleWindow consoleWindow,
            ModuleManagerWindow moduleManagerWindow) {
            this.objectId = objectId;
            this.commonStyles = commonStyles;
            this.consoleWindow = consoleWindow;
            this.moduleManagerWindow = moduleManagerWindow;

            Assembly assembly = Assembly.GetExecutingAssembly();
            windowTitle = $"KontrolSystem {assembly.GetName().Version}";
        }

        public void SetPosition(bool isTop) {
            float offset = 64f;

            if (isTop) {
                rectToFit = new Rect(0, 0, Screen.width - offset, Screen.height);
                windowRect = new Rect(Screen.width, 0, 0, 0);
            } else {
                rectToFit = new Rect(0, 0, Screen.width - offset, Screen.height - offset);
                windowRect = new Rect(Screen.width, Screen.height, 0, 0);
            }
        }

        public void SetPosition(bool isTop, Vector3 anchorPos) {
            float offset = 64f;
            float launcherScreenX = anchorPos.x + Screen.width / 2f;
            float launcherScreenY = anchorPos.y + Screen.height / 2f;

            float fitWidth = (isTop ? launcherScreenX : Screen.width - offset);
            float fitHeight = (isTop ? Screen.height : Screen.height - launcherScreenY);

            rectToFit = new Rect(0, 0f, fitWidth, fitHeight);

            float leftEdge = Screen.width;
            float topEdge = isTop ? 0f : Screen.height;

            windowRect = new Rect(leftEdge, topEdge, 0, 0);
        }

        public void DrawUI() {
            GUI.skin = HighLogic.Skin;

            windowRect = GUILayout.Window(objectId, windowRect, DrawWindow, windowTitle);
            windowRect = RectExtensions.ClampToRectAngle(windowRect, rectToFit);
        }

        void DrawWindow(int windowId) {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            DrawAvailableModules();

            GUILayout.BeginVertical(GUILayout.MinWidth(150));
            GUILayout.Label("Control", commonStyles.headingLabelStyle);
            if (GUILayout.Button("Manage")) moduleManagerWindow?.Toggle();
            if (GUILayout.Button(Mainframe.Instance.Rebooting ? "Rebooting..." : "Reboot")) OnReboot();
            GUILayout.Label("Global VALUES", commonStyles.headingLabelStyle);
            if (GUILayout.Button("Console")) {
                consoleWindow?.AttachTo(Mainframe.Instance.ConsoleBuffer);
                consoleWindow?.Toggle();
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            DrawStatus();
            GUILayout.EndVertical();
        }

        void DrawAvailableModules() {
            scrollPos = GUILayout.BeginScrollView(scrollPos, commonStyles.panelSkin.scrollView,
                GUILayout.MinWidth(260));

            GUILayout.BeginVertical();
            List<KontrolSystemProcess> availableProcesses = Mainframe.Instance.ListProcesses().ToList();
            if (!availableProcesses.Any()) {
                GUILayout.Label("No runable Kontrol module found.\n" +
                                "-------------------------\n" +
                                "Add one by implementing main_ksc(),\n" +
                                "main_editor(), main_tracking or\n" +
                                "main_flight().", commonStyles.panelSkin.label);
            } else {
                foreach (KontrolSystemProcess process in availableProcesses) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"{process.Name} ({process.State})");
                    switch (process.State) {
                    case KontrolSystemProcessState.Available:
                        if (GUILayout.Button("Run"))
                            Mainframe.Instance.StartProcess(process);
                        break;
                    case KontrolSystemProcessState.Running:
                    case KontrolSystemProcessState.Outdated:
                        if (GUILayout.Button("Abort"))
                            Mainframe.Instance.StopProcess(process);
                        break;
                    }

                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }

        void DrawStatus() {
            string status = "Unavailable";

            if (Mainframe.Instance.Initialized) {
                if (Mainframe.Instance.LastErrors.Any()) status = "Critical (Reboot failed)";
                else status = "OK";
            }

            GUILayout.Label($"Status: {status}");
        }

        void OnReboot() {
            Mainframe.Instance.Reboot(KontrolSystemConfig.Instance);
        }
    }
}
