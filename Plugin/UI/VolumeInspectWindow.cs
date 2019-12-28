using UnityEngine;

namespace KontrolSystem.Plugin.UI {
    public class VolumeInspectWindow : ResizableWindow {
        private Vector2 valuesScrollPos;
        private int selectedGroup;

        public void Awake() {
            Initialize(new Rect(50, 50, 400, 400));
        }
        protected override void DrawWindow(int windowId) {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            if (GUILayout.Button("Close")) Close();
            GUILayout.EndVertical();

            valuesScrollPos = GUILayout.BeginScrollView(valuesScrollPos, GUILayout.MinWidth(300), GUILayout.MaxWidth(700), GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical();

            selectedGroup = GUILayout.SelectionGrid(selectedGroup, new string[] { "Boolean", "Integers", "Floats", "Strings"}, 1);
            
            GUILayout.Toggle(true, "Demo");

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        protected override void OnResize(Rect windowRect) { }
    }
}