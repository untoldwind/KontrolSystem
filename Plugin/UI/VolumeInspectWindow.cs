using System;
using System.Globalization;
using KontrolSystem.Plugin.Core;
using UnityEngine;

namespace KontrolSystem.Plugin.UI {
    public class VolumeInspectWindow : ResizableWindow {
        private Vector2 valuesScrollPos;
        private int selectedGroup;
        private KontrolSystemVolume volume;

        public void Awake() {
            Initialize("KontrolSystem: Volume", new Rect(50, 50, 400, 400), false);
        }

        protected override void DrawWindow(int windowId) {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            if (GUILayout.Button("Close")) Close();
            selectedGroup = GUILayout.SelectionGrid(selectedGroup,
                new[] {"Boolean", "Integers", "Floats", "Strings"}, 1);
            GUILayout.EndVertical();

            valuesScrollPos = GUILayout.BeginScrollView(valuesScrollPos, GUILayout.MinWidth(300),
                GUILayout.MaxWidth(700), GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical();

            DrawValues();

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        protected override void OnResize(Rect windowRect) {
        }

        private void DrawValues() {
            if (volume == null) {
                GUILayout.Label("No volume");
                return;
            }

            switch (selectedGroup) {
            case 0:
                DrawBooleans();
                break;
            case 1:
                DrawIntegers();
                break;
            case 2:
                DrawFloats();
                break;
            case 3:
                DrawStrings();
                break;
            }
        }

        private void DrawBooleans() {
            foreach (string key in volume.BoolKeys) {
                bool value = GUILayout.Toggle(volume.GetBool(key, false), key);
                volume.SetBool(key, value);
            }
        }

        private void DrawIntegers() {
            foreach (string key in volume.IntKeys) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(key);
                string value = GUILayout.TextField(volume.GetInt(key, 0).ToString());
                try {
                    volume.SetInt(key, Convert.ToInt64(value, CultureInfo.InvariantCulture));
                } catch (Exception) {
                    volume.SetInt(key, 0);
                }

                GUILayout.EndHorizontal();
            }
        }

        private void DrawFloats() {
            foreach (string key in volume.FloatKeys) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(key);
                string value = GUILayout.TextField(volume.GetFloat(key, 0).ToString());
                try {
                    volume.SetFloat(key, Convert.ToDouble(value, CultureInfo.InvariantCulture));
                } catch (Exception) {
                    volume.SetFloat(key, 0);
                }

                GUILayout.EndHorizontal();
            }
        }

        private void DrawStrings() {
            foreach (string key in volume.FloatKeys) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(key);
                string value = GUILayout.TextField(volume.GetString(key, ""));
                volume.SetString(key, value);
                GUILayout.EndHorizontal();
            }
        }

        public void AttachTo(KontrolSystemVolume volume) {
            this.volume = volume;
            Open();
        }
    }
}
