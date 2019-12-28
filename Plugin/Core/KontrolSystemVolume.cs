using System.Collections.Generic;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public class KontrolSystemVolume : PartModule, KSPVesselModule.IVolume {
        private Dictionary<string, bool> booleans = new Dictionary<string, bool>();
        private Dictionary<string, long> integers = new Dictionary<string, long>();
        private Dictionary<string, double> doubles = new Dictionary<string, double>();
        private Dictionary<string, string> strings = new Dictionary<string, string>();

        public bool GetBool(string key, bool defaultValue) => booleans.ContainsKey(key) ? booleans[key] : defaultValue;

        public void SetBool(string key, bool value) {
            if (!booleans.ContainsKey(key)) booleans.Add(key, value);
            else booleans[key] = value;
        }

        public long GetInt(string key, long defaultValue) => integers.ContainsKey(key) ? integers[key] : defaultValue;

        public void SetInt(string key, long value) {
            if (!integers.ContainsKey(key)) integers.Add(key, value);
            else integers[key] = value;
        }

        public double GetFloat(string key, double defaultValue) => doubles.ContainsKey(key) ? doubles[key] : defaultValue;

        public void SetFloat(string key, double value) {
            if (!doubles.ContainsKey(key)) doubles.Add(key, value);
            else doubles[key] = value;
        }

        public string GetString(string key, string defaultValue) => strings.ContainsKey(key) ? strings[key] : defaultValue;

        public void SetString(string key, string value) {
            if (!strings.ContainsKey(key)) strings.Add(key, value);
            else strings[key] = value;
        }

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
            doubles.Clear();
            foreach (ConfigNode valueNode in node.GetNodes("doubles")) {
                double value = 0;
                if (!valueNode.TryGetValue("value", ref value)) continue;
                doubles.Add(valueNode.GetValue("name"), value);
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
            foreach (var kv in doubles) {
                ConfigNode doublesNode = node.AddNode("doubles");
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
