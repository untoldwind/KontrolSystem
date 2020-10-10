using System.Collections.Generic;
using KontrolSystem.Plugin.UI;
using KontrolSystem.KSP.Runtime.KSPVessel;

namespace KontrolSystem.Plugin.Core {
    public class KontrolSystemVolume : PartModule, KSPVesselModule.IVolume {
        private bool firstUpdate = true;

        public abstract class Entry {
            public readonly string key;

            protected Entry(string key) => this.key = key;
        }

        public class BoolEntry : Entry {
            public readonly bool value;

            internal BoolEntry(string key, bool value) : base(key) => this.value = value;
        }

        public class IntEntry : Entry {
            public readonly long value;

            internal IntEntry(string key, long value) : base(key) => this.value = value;
        }

        public class FloatEntry : Entry {
            public readonly double value;

            internal FloatEntry(string key, double value) : base(key) => this.value = value;
        }

        public class StringEntry : Entry {
            public readonly string value;

            internal StringEntry(string name, string value) : base(name) => this.value = value;
        }

        private readonly SortedDictionary<string, Entry> entries = new SortedDictionary<string, Entry>();

        public IEnumerable<string> BoolKeys => entries.Keys;

        public void Remove(string key) => entries.Remove(key);

        public bool GetBool(string key, bool defaultValue) => entries.ContainsKey(key)
            ? ((entries[key] as BoolEntry)?.value ?? defaultValue)
            : defaultValue;

        public void SetBool(string key, bool value) {
            if (!entries.ContainsKey(key)) entries.Add(key, new BoolEntry(key, value));
            else entries[key] = new BoolEntry(key, value);
        }

        public IEnumerable<string> IntKeys => entries.Keys;

        public long GetInt(string key, long defaultValue) => entries.ContainsKey(key)
            ? ((entries[key] as IntEntry)?.value ?? defaultValue)
            : defaultValue;

        public void SetInt(string key, long value) {
            if (!entries.ContainsKey(key)) entries.Add(key, new IntEntry(key, value));
            else entries[key] = new IntEntry(key, value);
        }

        public IEnumerable<string> FloatKeys => entries.Keys;

        public double GetFloat(string key, double defaultValue) => entries.ContainsKey(key)
            ? ((entries[key] as FloatEntry)?.value ?? defaultValue)
            : defaultValue;

        public void SetFloat(string key, double value) {
            if (!entries.ContainsKey(key)) entries.Add(key, new FloatEntry(key, value));
            else entries[key] = new FloatEntry(key, value);
        }

        public IEnumerable<string> StringKeys => entries.Keys;

        public string GetString(string key, string defaultValue) => entries.ContainsKey(key)
            ? ((entries[key] as StringEntry)?.value ?? defaultValue)
            : defaultValue;

        public void SetString(string key, string value) {
            if (!entries.ContainsKey(key)) entries.Add(key, new StringEntry(key, value));
            else entries[key] = new StringEntry(key, value);
        }

        public IEnumerable<Entry> AllEntries => entries.Values;

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Inspect volume", category = "skip_delay;")]
        public void InspectVolume() => ToolbarButton.Instance?.VolumeInspect?.AttachTo(this);

        public override void OnFixedUpdate() {
            if (vessel.HoldPhysics) return;

            if (firstUpdate) {
                Mainframe mainframe = Mainframe.Instance;

                if (mainframe == null || mainframe.Rebooting) return;

                firstUpdate = false;
            }
        }

        public override void OnLoad(ConfigNode node) {
            PluginLogger.Instance.Info("OnLoad: " + node);
            
            entries.Clear();

            if (!node.HasNode("volumnData")) return;

            ConfigNode volumeData = node.GetNode("volumnData");
            ConfigNode booleans = volumeData.GetNode("booleans");
            ConfigNode integers = volumeData.GetNode("integers");
            ConfigNode floats = volumeData.GetNode("floats");
            ConfigNode strings = volumeData.GetNode("strings");

            foreach(var key in booleans.values.DistinctNames()) {
                bool value = false;
                if (!booleans.TryGetValue(key, ref value)) continue;
                entries.Add(key, new BoolEntry(key, value));
            }
            foreach(var key in integers.values.DistinctNames()) {
                long value = 0;
                if (!integers.TryGetValue(key, ref value)) continue;
                entries.Add(key, new IntEntry(key, value));
            }
            foreach(var key in floats.values.DistinctNames()) {
                double value = 0;
                if (!floats.TryGetValue(key, ref value)) continue;
                entries.Add(key, new FloatEntry(key, value));
            }
            foreach(var key in strings.values.DistinctNames()) {
                string value = "";
                if (!strings.TryGetValue(key, ref value)) continue;
                entries.Add(key, new StringEntry(key, value));
            }

            base.OnLoad(node);
        }

        public override void OnSave(ConfigNode node) {
            PluginLogger.Instance.Info("OnSave: " + node);

            ConfigNode booleans = new ConfigNode("booleans");
            ConfigNode integers = new ConfigNode("integers");
            ConfigNode floats = new ConfigNode("floats");
            ConfigNode strings = new ConfigNode("strings");

            foreach (var entry in entries.Values) {
                switch (entry) {
                case BoolEntry e:
                    booleans.AddValue(e.key, e.value);
                    break;
                case IntEntry e:
                    integers.AddValue(e.key, e.value);
                    break;
                case FloatEntry e:
                    floats.AddValue(e.key, e.value);
                    break;
                case StringEntry e:
                    strings.AddValue(e.key, e.value);
                    break;
                }
            }

            ConfigNode volumeData = new ConfigNode("volumnData");
            volumeData.AddNode(booleans);
            volumeData.AddNode(integers);
            volumeData.AddNode(floats);
            volumeData.AddNode(strings);

            node.AddNode(volumeData);
            
            base.OnSave(node);
        }
    }
}
