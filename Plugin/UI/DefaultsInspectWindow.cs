using System;
using System.Globalization;
using KontrolSystem.Plugin.Core;
using UnityEngine;

namespace KontrolSystem.Plugin.UI {
    public class DefaultsInspectWindow : ResizableWindow {
        private KontrolSystemDefaults defaults;

        private string steeringPitchTs;
        private string steeringYawTs;
        private string steeringRollTs;

        public void Awake() {
            Initialize("KontrolSystem: Defaults", new Rect(Screen.width / 2 - 200, Screen.height / 2 - 50, 400, 100),
                false);
        }

        protected override void DrawWindow(int windowId) {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Steering Pitch Ts", GUILayout.Width(150));
            steeringPitchTs = GUILayout.TextField(steeringPitchTs, GUILayout.ExpandWidth(true));
            try {
                defaults.SteeringPitchTs = double.Parse(steeringPitchTs, CultureInfo.InvariantCulture);
            } catch (FormatException) {
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Steering Yaw Ts", GUILayout.Width(150));
            steeringYawTs = GUILayout.TextField(steeringYawTs, GUILayout.ExpandWidth(true));
            try {
                defaults.SteeringYawTs = double.Parse(steeringYawTs, CultureInfo.InvariantCulture);
            } catch (FormatException) {
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Steering Roll Ts", GUILayout.Width(150));
            steeringRollTs = GUILayout.TextField(steeringRollTs, GUILayout.ExpandWidth(true));
            try {
                defaults.SteeringRollTs = double.Parse(steeringRollTs, CultureInfo.InvariantCulture);
            } catch (FormatException) {
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Close")) {
                defaults.CloseInspectWindow();
            }

            GUILayout.EndVertical();
        }

        protected override void OnResize(Rect newWindowRect) {
        }

        public void AttachTo(KontrolSystemDefaults newDefaults) {
            defaults = newDefaults;

            steeringPitchTs = defaults.SteeringPitchTs.ToString(CultureInfo.InvariantCulture);
            steeringYawTs = defaults.SteeringYawTs.ToString(CultureInfo.InvariantCulture);
            steeringRollTs = defaults.SteeringRollTs.ToString(CultureInfo.InvariantCulture);

            Open();
        }
    }
}
