using System.Linq;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPGame {
    [KSModule("ksp::game::alarms",
        Description = "Collection of functions to control alarms."
    )]
    public class KSPAlarmClockModule {
        [KSClass("AlarmClock")]
        public class AlarmClockAPIWrapper {
            public AlarmClockAPIWrapper() {
            }

            [KSMethod]
            public bool DeleteAlarm(long alarmID) {
                return AlarmClockScenario.DeleteAlarm((uint)alarmID);
            }

            [KSMethod]
            public AlarmWrapper[] GetAlarms() {
                var list = AlarmClockScenario.Instance.alarms.Values.ToList();

                list.Sort((a, b) => a.TimeToAlarm.CompareTo(b.TimeToAlarm));

                return list.Select(a => new AlarmWrapper(a)).ToArray();
            }
        }

        [KSClass("Alarm")]
        public class AlarmWrapper {
            private readonly AlarmTypeBase instance;

            public AlarmWrapper(AlarmTypeBase instance) {
                this.instance = instance;
            }

            [KSField] public long Id => instance.Id;

            [KSField]
            public string Name {
                get { return instance.title; }
                set { instance.title = value; }
            }

            [KSField]
            public string Notes {
                get { return instance.description; }
                set { instance.description = value; }
            }

            [KSField]
            public double AlarmTime {
                get {
                    return instance.TimeToAlarm;
                }
            }
        }
    }
}
