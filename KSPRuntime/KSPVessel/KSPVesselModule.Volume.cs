using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Volume")]
        public interface IVolume {
            [KSMethod]
            bool GetBool(string key, bool defaultValue);

            [KSMethod]
            void SetBool(string key, bool value);

            [KSMethod]
            long GetInt(string key, long defaultValue);

            [KSMethod]
            void SetInt(string key, long value);

            [KSMethod]
            double GetFloat(string key, double defaultValue);

            [KSMethod]
            void SetFloat(string key, double value);

            [KSMethod]
            string GetString(string key, string defaultValue);

            [KSMethod]
            void SetString(string key, string value);
        }
    }
}
