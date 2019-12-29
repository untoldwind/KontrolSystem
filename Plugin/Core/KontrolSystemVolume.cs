using System.Collections.Generic;
using KontrolSystem.Plugin.UI;
using KontrolSystem.KSP.Runtime.KSPVessel;

namespace KontrolSystem.Plugin.Core {
    public class KontrolSystemVolume : PartModule, KSPVesselModule.IVolume {
        private SortedDictionary<string, bool> booleans = new SortedDictionary<string, bool>();
        private SortedDictionary<string, long> integers = new SortedDictionary<string, long>();
        private SortedDictionary<string, double> floats = new SortedDictionary<string, double>();
        private SortedDictionary<string, string> strings = new SortedDictionary<string, string>();

        public IEnumerable<string> BoolKeys => booleans.Keys;

        public bool GetBool(string key, bool defaultValue) => booleans.ContainsKey(key) ? booleans[key] : defaultValue;

        public void SetBool(string key, bool value) {
            if (!booleans.ContainsKey(key)) booleans.Add(key, value);
            else booleans[key] = value;
        }

        public IEnumerable<string> IntKeys => integers.Keys;

        public long GetInt(string key, long defaultValue) => integers.ContainsKey(key) ? integers[key] : defaultValue;

        public void SetInt(string key, long value) {
            if (!integers.ContainsKey(key)) integers.Add(key, value);
            else integers[key] = value;
        }

        public IEnumerable<string> FloatKeys => floats.Keys;

        public double GetFloat(string key, double defaultValue) => floats.ContainsKey(key) ? floats[key] : defaultValue;

        public void SetFloat(string key, double value) {
            if (!floats.ContainsKey(key)) floats.Add(key, value);
            else floats[key] = value;
        }

        public IEnumerable<string> StringKeys => strings.Keys;

        public string GetString(string key, string defaultValue) => strings.ContainsKey(key) ? strings[key] : defaultValue;

        public void SetString(string key, string value) {
            if (!strings.ContainsKey(key)) strings.Add(key, value);
            else strings[key] = value;
        }

        [KSPEvent(guiActive = true, guiName = "Inspect volume", category = "skip_delay;")]
        public void InspectVolume() => ToolbarButton.Instance?.VolumeInspect?.AttachTo(this);

        public override void OnLoad(ConfigNode node) {
            booleans.Clear();
            foreach (ConfigNode valueNode in node.GetNodes("booleans")) {
                bool value = false;
                if (!valueNode.TryGetValue("value", ref value)) continue;
                booleans.Add(valueNode.GetValue("name"), value);
            }
            integers.Clear();
            foreach (ConfigNode valueNode in node.GetNodes("integers")) {
                long value = 0;
                if (!valueNode.TryGetValue("value", ref value)) continue;
                integers.Add(valueNode.GetValue("name"), value);
            }
            floats.Clear();
            foreach (ConfigNode valueNode in node.GetNodes("floats")) {
                double value = 0;
                if (!valueNode.TryGetValue("value", ref value)) continue;
                floats.Add(valueNode.GetValue("name"), value);
            }
            strings.Clear();
            foreach (ConfigNode valueNode in node.GetNodes("strings")) {
                string value = null;
                if (!valueNode.TryGetValue("value", ref value)) continue;
                strings.Add(valueNode.GetValue("name"), value);
            }
        }

        public override void OnSave(ConfigNode node) {
            node.ClearNodes();
            foreach (var kv in booleans) {
                ConfigNode booleansNode = node.AddNode("booleans");
                booleansNode.SetValue("name", kv.Key);
                booleansNode.SetValue("value", kv.Value);
            }
            foreach (var kv in integers) {
                ConfigNode integersNode = node.AddNode("integers");
                integersNode.SetValue("name", kv.Key);
                integersNode.SetValue("value", kv.Value);
            }
            foreach (var kv in floats) {
                ConfigNode doublesNode = node.AddNode("floats");
                doublesNode.SetValue("name", kv.Key);
                doublesNode.SetValue("value", kv.Value);
            }
            foreach (var kv in strings) {
                ConfigNode stringsNode = node.AddNode("strings");
                stringsNode.SetValue("name", kv.Key);
                stringsNode.SetValue("value", kv.Value);
            }
        }
    }
}
