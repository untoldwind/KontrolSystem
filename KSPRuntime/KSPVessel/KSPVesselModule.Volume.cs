using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Volume")]
        public interface IVolume {
            [KSMethod]
            bool HasBool(string key);

            [KSMethod]
            bool GetBool(string key, bool defaultValue);

            [KSMethod]
            void SetBool(string key, bool value);

            [KSMethod]
            bool HasInt(string key);

            [KSMethod]
            long GetInt(string key, long defaultValue);

            [KSMethod]
            void SetInt(string key, long value);

            [KSMethod]
            bool HasFloat(string key);

            [KSMethod]
            double GetFloat(string key, double defaultValue);

            [KSMethod]
            void SetFloat(string key, double value);

            [KSMethod]
            bool HasString(string key);

            [KSMethod]
            string GetString(string key, string defaultValue);

            [KSMethod]
            void SetString(string key, string value);
        }

        public class VolumeWrapper : IVolume {
            private readonly List<IVolume> volumes;

            public VolumeWrapper(List<IVolume> volumes) => this.volumes = volumes;

            public bool HasBool(string key) {
                foreach (var item in volumes) {
                    if (item.HasBool(key)) return true;
                }

                return false;
            }

            public bool GetBool(string key, bool defaultValue) {
                foreach (var item in volumes) {
                    if (item.HasBool(key)) return item.GetBool(key, defaultValue);
                }

                return defaultValue;
            }

            public void SetBool(string key, bool value) {
                foreach (var item in volumes) item.SetBool(key, value);
            }

            public bool HasInt(string key) {
                foreach (var item in volumes) {
                    if (item.HasInt(key)) return true;
                }

                return false;
            }

            public long GetInt(string key, long defaultValue) {
                foreach (var item in volumes) {
                    if (item.HasInt(key)) return item.GetInt(key, defaultValue);
                }

                return defaultValue;
            }

            public void SetInt(string key, long value) {
                foreach (var item in volumes) item.SetInt(key, value);
            }

            public bool HasFloat(string key) {
                foreach (var item in volumes) {
                    if (item.HasFloat(key)) return true;
                }

                return false;
            }

            public double GetFloat(string key, double defaultValue) {
                foreach (var item in volumes) {
                    if (item.HasFloat(key)) return item.GetFloat(key, defaultValue);
                }

                return defaultValue;
            }

            public void SetFloat(string key, double value) {
                foreach (var item in volumes) item.SetFloat(key, value);
            }

            public bool HasString(string key) {
                foreach (var item in volumes) {
                    if (item.HasString(key)) return true;
                }

                return false;
            }

            public string GetString(string key, string defaultValue) {
                foreach (var item in volumes) {
                    if (item.HasString(key)) return item.GetString(key, defaultValue);
                }

                return defaultValue;
            }

            public void SetString(string key, string value) {
                foreach (var item in volumes) item.SetString(key, value);
            }
        }
    }
}
