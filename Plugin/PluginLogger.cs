using KontrolSystem.TO2.Runtime;
using System;

namespace KontrolSystem.Plugin {
    public class PluginLogger : ITO2Logger {
        private static string LOG_PREFIX = "[KontrolSystem] ";

        private static PluginLogger _instance;

        public static PluginLogger Instance => _instance ??= new PluginLogger();

        public static bool debugEnabled = true;

        public void Debug(string message) {
            if (debugEnabled) UnityEngine.Debug.Log(LOG_PREFIX + "DEBUG: " + message);
        }

        public void Info(string message) => UnityEngine.Debug.Log(LOG_PREFIX + "INFO: " + message);

        public void Warning(string message) => UnityEngine.Debug.LogWarning(LOG_PREFIX + message);

        public void Error(string message) => UnityEngine.Debug.LogError(LOG_PREFIX + message);

        public void LogException(Exception exception) => UnityEngine.Debug.LogException(exception);
    }
}
