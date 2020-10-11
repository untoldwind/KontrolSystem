using KontrolSystem.Plugin.Core;
using UnityEngine;

namespace KontrolSystem.Plugin.UI {
    public class NameTagInspectWindow : ResizableWindow {
        private KontrolSystemNameTag nameTag;
        
        public void Awake() {
            Initialize("KontrolSystem: Name Tag", new Rect(Screen.width / 2 - 75, Screen.height / 2 - 20, 150, 40), false);
        }
        protected override void DrawWindow(int windowId) {
            GUILayout.BeginVertical();
            nameTag.nameTag = GUILayout.TextField(nameTag.nameTag, GUILayout.ExpandWidth(true));

            if (GUILayout.Button("Close")) {
                nameTag.CloseInspectWindow();
            }
            
            GUILayout.EndVertical();
        }

        protected override void OnResize(Rect newWindowRect) {
        }
        
        public void AttachTo(KontrolSystemNameTag newNameTag) {
            nameTag = newNameTag;
            Open();
        }        
    }
}
