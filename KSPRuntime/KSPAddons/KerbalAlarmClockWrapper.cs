using System;
using System.Reflection;

namespace KontrolSystem.KSP.Runtime.KSPAddons {
    public class KerbalAlarmClockWrapper {
        private static AlarmClockAPI alarmClockAPI = null;
        
        public enum AlarmTypeEnum
        {
            Raw,
            Maneuver,
            ManeuverAuto,
            Apoapsis,
            Periapsis,
            AscendingNode,
            DescendingNode,
            LaunchRendevous,
            Closest,
            SOIChange,
            SOIChangeAuto,
            Transfer,
            TransferModelled,
            Distance,
            Crew,
            EarthTime,
            Contract,
            ContractAuto,
            ScienceLab
        }

        public class AlarmClockAPI {
            private readonly Object actualKAC;
            private readonly FieldInfo apiReadyField;
            private readonly MethodInfo createAlarmMethod;
            private readonly MethodInfo deleteAlarmMethod;

            public AlarmClockAPI(Type kerbalAlarmClockType, Object kerbalAlarmClock) {
                actualKAC = kerbalAlarmClock;
                apiReadyField = kerbalAlarmClockType.GetField("APIReady", BindingFlags.Public | BindingFlags.Static);
                createAlarmMethod =
                    kerbalAlarmClockType.GetMethod("CreateAlarm", BindingFlags.Public | BindingFlags.Instance);
                deleteAlarmMethod =
                    kerbalAlarmClockType.GetMethod("DeleteAlarm", BindingFlags.Public | BindingFlags.Instance);
            }

            public bool APIReady {
                get {
                    if (apiReadyField == null)
                        return false;

                    return (bool) apiReadyField.GetValue(null);
                }
            }

            internal string CreateAlarm(AlarmTypeEnum alarmType, string name, double UT) {
                return (string) createAlarmMethod.Invoke(actualKAC, new object[] {
                    (Int32) alarmType, name, UT
                });
            }

            internal bool DeleteAlarm(string alarmID) {
                return (bool) deleteAlarmMethod.Invoke(actualKAC, new object[] {alarmID});
            }
        }
    }
}
