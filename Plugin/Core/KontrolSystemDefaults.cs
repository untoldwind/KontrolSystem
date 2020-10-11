using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.Plugin.UI;

namespace KontrolSystem.Plugin.Core {
    public class KontrolSystemDefaults : PartModule, KSPVesselModule.IDefaults {
        public double SteeringPitchTs { get; set; } = 2;

        public double SteeringYawTs { get; set; } = 2;

        public double SteeringRollTs { get; set; } = 2;

        private DefaultsInspectWindow inspectWindow;

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "KontrolSystem defaults",
            groupName = "KontrolSystem", groupDisplayName = "KontrolSystem", category = "skip_delay;")]
        public void InspectVolume() {
            inspectWindow = gameObject.AddComponent<DefaultsInspectWindow>();
            inspectWindow.AttachTo(this);
        }

        public void CloseInspectWindow() {
            inspectWindow.Close();
            Destroy(inspectWindow);
            inspectWindow = null;
        }

        public override void OnLoad(ConfigNode node) {
            PluginLogger.Instance.Info("Defaults OnLoad");

            if (!node.HasNode("defaultsData")) return;

            ConfigNode defaultsData = node.GetNode("defaultsData");

            double steeringPitchTs = 0;
            if (defaultsData.TryGetValue("steeringPithTs", ref steeringPitchTs)) {
                SteeringPitchTs = steeringPitchTs;
            }

            double steeringYawTs = 0;
            if (defaultsData.TryGetValue("steeringYawTs", ref steeringYawTs)) {
                SteeringYawTs = steeringYawTs;
            }

            double steeringRollTs = 0;
            if (defaultsData.TryGetValue("steeringRollTs", ref steeringRollTs)) {
                SteeringRollTs = steeringRollTs;
            }

            base.OnLoad(node);
        }

        public override void OnSave(ConfigNode node) {
            PluginLogger.Instance.Info("Defaults OnSave");

            ConfigNode defaultsData = new ConfigNode("defaultsData");

            defaultsData.AddValue("steeringPithTs", SteeringPitchTs);
            defaultsData.AddValue("steeringYawTs", SteeringYawTs);
            defaultsData.AddValue("steeringRollTs", SteeringRollTs);

            node.AddNode(defaultsData);

            base.OnSave(node);
        }
    }
}
