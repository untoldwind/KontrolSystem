using System;
using System.IO;
using KSP.IO;
using KontrolSystem.KSP.Runtime;

namespace KontrolSystem.Plugin.Config {
    public class KontrolSystemConfig {
        private static KontrolSystemConfig instance;

        public static KontrolSystemConfig Instance => instance ?? (instance = new KontrolSystemConfig());

        private string to2BaseDir;
        private bool includeStdLib;
        private string stdLibDir;

        private KontrolSystemConfig() {
            LoadConfig();
        }

        public string TO2BaseDir {
            get => to2BaseDir;
            set {
                to2BaseDir = value;
                SaveConfig();
            }
        }

        public bool IncludeStdLib {
            get => includeStdLib;
            set {
                includeStdLib = value;
                SaveConfig();
            }
        }

        public string StdLibDir {
            get => stdLibDir;
            set {
                stdLibDir = value;
                SaveConfig();
            }
        }

        private void LoadConfig() {
            try {
                PluginConfiguration config = PluginConfiguration.CreateForType<KontrolSystemKSPRegistry>();
                config.load();

                to2BaseDir = config.GetValue<string>("TO2BaseDir", DefaultBaseDir);
                includeStdLib = config.GetValue<bool>("IncludeStdLib", true);
                stdLibDir = config.GetValue<string>("StdLibDir", DefaultStdLibDir);
            } catch (Exception e) {
                PluginLogger.Instance.Error("Load config failed (using fallback)");
                PluginLogger.Instance.Error(e.ToString());

                to2BaseDir = DefaultBaseDir;
                stdLibDir = DefaultStdLibDir;
                includeStdLib = true;
            }
        }

        private string DefaultBaseDir => Path.Combine(GameDatabase.Instance.PluginDataFolder, "Ships", "to2");

        private string DefaultStdLibDir =>
            Path.Combine(GameDatabase.Instance.PluginDataFolder, "GameData", "KontrolSystem", "to2");

        private void SaveConfig() {
            PluginConfiguration config = PluginConfiguration.CreateForType<KontrolSystemKSPRegistry>();

            config.SetValue("TO2BaseDir", to2BaseDir);
            config.SetValue("IncludeStdLib", includeStdLib);
            config.SetValue("StdLibDir", stdLibDir);
            config.save();
        }
    }
}
