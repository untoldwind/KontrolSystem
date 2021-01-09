using System;
using System.Linq;
using System.Reflection;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPAddons {
    public enum AlarmTypeEnum {
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

    public partial class KSPAddonsModule {
        [KSClass("AlarmClock")]
        public class AlarmClockAPIWrapper {
            private readonly object actualKAC;
            private readonly Type alarmType;
            private readonly FieldInfo apiReadyField;
            private readonly MethodInfo createAlarmMethod;
            private readonly MethodInfo deleteAlarmMethod;
            private readonly FieldInfo alarmsField;

            public AlarmClockAPIWrapper() {
                Type kerbalAlarmClockType = AssemblyLoader.loadedAssemblies
                    .SelectMany(a => a.assembly.GetExportedTypes())
                    .FirstOrDefault(t => t.FullName == "KerbalAlarmClock.KerbalAlarmClock");
                Type alarmType = AssemblyLoader.loadedAssemblies.SelectMany(a => a.assembly.GetExportedTypes())
                    .FirstOrDefault(t => t.FullName == "KerbalAlarmClock.KACAlarm");

                if (kerbalAlarmClockType == null || alarmType == null) return;

                this.alarmType = alarmType;
                try {
                    actualKAC = kerbalAlarmClockType
                        .GetField("APIInstance", BindingFlags.Public | BindingFlags.Static).GetValue(null);

                    apiReadyField =
                        kerbalAlarmClockType.GetField("APIReady", BindingFlags.Public | BindingFlags.Static);
                    alarmsField = kerbalAlarmClockType.GetField("alarms", BindingFlags.Public | BindingFlags.Static);
                    createAlarmMethod =
                        kerbalAlarmClockType.GetMethod("CreateAlarm", BindingFlags.Public | BindingFlags.Instance);
                    deleteAlarmMethod =
                        kerbalAlarmClockType.GetMethod("DeleteAlarm", BindingFlags.Public | BindingFlags.Instance);
                } catch (Exception) {
                    actualKAC = null;
                }
            }

            [KSField]
            public bool Ready {
                get {
                    if (apiReadyField == null)
                        return false;

                    return (bool) apiReadyField.GetValue(null);
                }
            }

            [KSMethod]
            public Result<string, string> CreateAlarm(string alarmType, string name, double UT) {
                if (createAlarmMethod == null)
                    return Result.Err<string, string>("KerbalAlarmClock addon not available");
                try {
                    AlarmTypeEnum newAlarmType = (AlarmTypeEnum) Enum.Parse(typeof(AlarmTypeEnum), alarmType);
                    return Result.Ok<string, string>((string) createAlarmMethod.Invoke(actualKAC, new object[] {
                        (Int32) newAlarmType, name, UT
                    }));
                } catch (ArgumentException) {
                    return Result.Err<string, string>($"Invalid alarm type {alarmType}");
                }
            }

            [KSMethod]
            public Result<bool, string> DeleteAlarm(string alarmID) {
                if (deleteAlarmMethod == null)
                    return Result.Err<bool, string>("KerbalAlarmClock addon not available");
                return Result.Ok<bool, string>((bool) deleteAlarmMethod.Invoke(actualKAC, new object[] {alarmID}));
            }

            [KSMethod]
            public Result<AlarmWrapper[], string> GetAlarms() {
                if (alarmsField == null)
                    return Result.Err<AlarmWrapper[], string>("KerbalAlarmClock addon not available");

                System.Collections.IList list = (System.Collections.IList) alarmsField.GetValue(actualKAC);
                AlarmWrapper[] result = new AlarmWrapper[list.Count];

                for (int i = 0; i < result.Length; i++) {
                    result[i] = new AlarmWrapper(alarmType, list[i]);
                }
                return Result.Ok<AlarmWrapper[], string>(result);
            }
        }

        [KSClass("Alarm")]
        public class AlarmWrapper {
            private readonly object instance;
            private readonly FieldInfo idField;
            private readonly FieldInfo nameField;
            private readonly FieldInfo notesField;
            private readonly FieldInfo alarmTypeField;
            private readonly PropertyInfo alarmTimeProperty;

            public AlarmWrapper(Type alarmType, object instance) {
                this.instance = instance;

                idField = alarmType.GetField("ID");
                nameField = alarmType.GetField("Name");
                notesField = alarmType.GetField("Notes");
                alarmTypeField = alarmType.GetField("TypeOfAlarm");
                alarmTimeProperty = alarmType.GetProperty("AlarmTimeUT");
            }

            [KSField] public string Id => (string) idField.GetValue(instance);

            [KSField]
            public string Name {
                get { return (string) nameField.GetValue(instance); }
                set { nameField.SetValue(instance, value); }
            }

            [KSField]
            public string Notes {
                get { return (string) notesField.GetValue(instance); }
                set { notesField.SetValue(instance, value); }
            }

            [KSField]
            public double AlarmTime {
                get {
                    return (double) alarmTimeProperty.GetValue(instance, null);
                }
                set {
                    alarmTimeProperty.SetValue(instance, value);
                }
            }

            public string AlarmType => ((AlarmTypeEnum) alarmTypeField.GetValue(instance)).ToString();
        }
    }
}
