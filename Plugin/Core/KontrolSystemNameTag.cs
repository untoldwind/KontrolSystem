using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.Plugin.UI;

namespace KontrolSystem.Plugin.Core {
    public class KontrolSystemNameTag : PartModule, KSPVesselModule.INameTag {
        private NameTagInspectWindow inspectWindow;

        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "KontrolSystem tag")]
        public string nameTag = "";

        public string Tag {
            get => nameTag;
            set => nameTag = value;
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Change KontrolSystem Tag",
            groupName = "KontrolSystem", groupDisplayName = "KontrolSystem", category = "skip_delay;")]
        public void InspectNameTag() {
            if (inspectWindow != null) {
                inspectWindow.Close();
                Destroy(inspectWindow);
            }
            inspectWindow = gameObject.AddComponent<NameTagInspectWindow>();
            inspectWindow.AttachTo(this);
        }

        public void CloseInspectWindow() {
            inspectWindow.Close();
            Destroy(inspectWindow);
            inspectWindow = null;
        }
    }
}
