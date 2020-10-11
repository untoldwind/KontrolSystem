using System;
using System.Collections.Generic;
using System.Globalization;
using KontrolSystem.Plugin.Core;
using UnityEngine;
using System.Linq;

namespace KontrolSystem.Plugin.UI {
    public class VolumeInspectWindow : ResizableWindow {
        private Vector2 valuesScrollPos;
        private KontrolSystemVolume volume;
        private Texture2D addButtonTexture;
        private Texture2D deleteButtonTexture;
        private int addType;
        private string addName;
        private readonly Dictionary<string, string> invalids = new Dictionary<string, string>();

        public void Awake() {
            Initialize("KontrolSystem: Volume", new Rect(Screen.width / 2 - 200, Screen.height / 2 - 100, 400, 400), false);

            addButtonTexture = GameDatabase.Instance.GetTexture("KontrolSystem/GFX/add", false);
            deleteButtonTexture = GameDatabase.Instance.GetTexture("KontrolSystem/GFX/delete", false);
        }

        protected override void DrawWindow(int windowId) {
            GUILayout.BeginVertical();

            valuesScrollPos = GUILayout.BeginScrollView(valuesScrollPos, GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true),
                GUILayout.MinWidth(300), GUILayout.MaxWidth(3000), GUILayout.MinHeight(150), GUILayout.MaxHeight(3000));

            DrawValues();

            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            addType = GUILayout.SelectionGrid(addType, new[] {"Bool", "Int", "Float", "String"}, 4,
                GUILayout.ExpandWidth(false));
            addName = GUILayout.TextField(addName, GUILayout.ExpandWidth(true));
            if (GUILayout.Button(addButtonTexture, GUILayout.Width(30)) && !string.IsNullOrEmpty(addName)) {
                switch (addType) {
                case 0:
                    volume.SetBool(addName, false);
                    break;
                case 1:
                    volume.SetInt(addName, 0);
                    break;
                case 2:
                    volume.SetFloat(addName, 0.0);
                    break;
                case 3:
                    volume.SetString(addName, "");
                    break;
                }
            }

            GUILayout.EndHorizontal();


            if (GUILayout.Button("Close")) {
                volume.CloseInspectWindow();
            }

            GUILayout.EndVertical();
        }

        protected override void OnResize(Rect newWindowRect) {
        }

        private void DrawValues() {
            GUILayout.BeginVertical();

            foreach (var entry in volume.AllEntries.ToList()) {
                GUILayout.BeginHorizontal();

                GUILayout.Label(entry.key, GUILayout.Width(windowRect.width / 2 - 80));

                switch (entry) {
                case KontrolSystemVolume.BoolEntry e:
                    GUILayout.Label("Bool", GUILayout.Width(50));
                    var newBool = GUILayout.Toggle(e.value, "", GUILayout.ExpandWidth(true));
                    if (newBool != e.value) {
                        volume.SetBool(e.key, newBool);
                    }

                    break;
                case KontrolSystemVolume.IntEntry e:
                    GUILayout.Label("Int", GUILayout.Width(50));
                    var currentInt = invalids.ContainsKey(e.key)
                        ? invalids[e.key]
                        : e.value.ToString(CultureInfo.InvariantCulture);
                    var newInt = GUILayout.TextField(currentInt, GUILayout.ExpandWidth(true));
                    if (newInt != currentInt) {
                        if (invalids.ContainsKey(e.key)) invalids.Remove(e.key);
                        try {
                            volume.SetInt(e.key, long.Parse(newInt, CultureInfo.InvariantCulture));
                        } catch (FormatException) {
                            invalids.Add(e.key, newInt);
                        }
                    }

                    break;
                case KontrolSystemVolume.FloatEntry e:
                    GUILayout.Label("Float", GUILayout.Width(50));
                    var currentFloat = invalids.ContainsKey(e.key)
                        ? invalids[e.key]
                        : e.value.ToString(CultureInfo.InvariantCulture);
                    var newFloat = GUILayout.TextField(currentFloat, GUILayout.ExpandWidth(true));
                    if (newFloat != currentFloat) {
                        if (invalids.ContainsKey(e.key)) invalids.Remove(e.key);
                        try {
                            volume.SetFloat(e.key, double.Parse(newFloat, CultureInfo.InvariantCulture));
                        } catch (FormatException) {
                            invalids.Add(e.key, newFloat);
                        }
                    }

                    break;
                case KontrolSystemVolume.StringEntry e:
                    GUILayout.Label("String", GUILayout.Width(50));
                    var newString = GUILayout.TextField(e.value, GUILayout.ExpandWidth(true));
                    if (newString != e.value) {
                        volume.SetString(e.key, newString);
                    }

                    break;
                }

                if (GUILayout.Button(deleteButtonTexture, GUILayout.Width(30))) {
                    volume.Remove(entry.key);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        public void AttachTo(KontrolSystemVolume newVolume) {
            volume = newVolume;
            invalids.Clear();
            Open();
        }
    }
}
