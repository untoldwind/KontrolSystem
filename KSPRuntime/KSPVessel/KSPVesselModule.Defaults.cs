using System.Collections.Generic;
using KontrolSystem.TO2.Binding;
using System.Linq;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Defaults")]
        public interface IDefaults {
            [KSField] double SteeringPitchTs { get; set; }

            [KSField] double SteeringYawTs { get; set; }

            [KSField] double SteeringRollTs { get; set; }
        }

        public class DefaultsWrapper : IDefaults {
            private readonly List<IDefaults> defaults;

            public DefaultsWrapper(List<IDefaults> defaults) => this.defaults = defaults;

            public double SteeringPitchTs {
                get => defaults.FirstOrDefault()?.SteeringPitchTs ?? 2;
                set {
                    UnityEngine.Debug.Log($">>>>> Pitch {defaults.Count}: {value}");
                    foreach (var item in defaults) item.SteeringPitchTs = value;
                }
            }

            public double SteeringYawTs {
                get => defaults.FirstOrDefault()?.SteeringYawTs ?? 2;
                set {
                    UnityEngine.Debug.Log($">>>>> Yaw {defaults.Count}: {value}");
                    foreach (var item in defaults) item.SteeringYawTs = value;
                }
            }

            public double SteeringRollTs {
                get => defaults.FirstOrDefault()?.SteeringRollTs ?? 2;
                set {
                    foreach (var item in defaults) item.SteeringRollTs = value;
                }
            }
        }
    }
}
