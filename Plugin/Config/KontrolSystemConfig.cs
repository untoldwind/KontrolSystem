using System;
using System.IO;
using KSP.IO;

namespace KontrolSystem.Plugin.Config {
    public class KontrolSystemConfig {
        private static KontrolSystemConfig _instance;

        public static KontrolSystemConfig Instance => _instance ??= new KontrolSystemConfig();

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
                PluginConfiguration config = PluginConfiguration.CreateForType<KontrolSystemConfig>();
                config.load();

                to2BaseDir = config.GetValue("TO2BaseDir", DefaultBaseDir);
                includeStdLib = config.GetValue("IncludeStdLib", true);
                stdLibDir = config.GetValue("StdLibDir", DefaultStdLibDir);
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
            PluginConfiguration config = PluginConfiguration.CreateForType<KontrolSystemConfig>();

            config.SetValue("TO2BaseDir", to2BaseDir);
            config.SetValue("IncludeStdLib", includeStdLib);
            config.SetValue("StdLibDir", stdLibDir);
            config.save();
        }
    }
}
