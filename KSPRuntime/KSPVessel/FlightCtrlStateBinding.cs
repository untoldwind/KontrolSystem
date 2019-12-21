using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public class FlightCtrlStateBinding {
        public static RecordStructType FlightCtrlStateType = new RecordStructType("ksp::vessel", "FlightCtrlState", "Current state of the (pilots) flight controls.", typeof(FlightCtrlState),
            new RecordStructField[] {
                new RecordStructField("main_throttle", "Setting for the main throttle (0 - 1)",  BuildinType.Float, typeof(FlightCtrlState).GetField("mainThrottle")),
                new RecordStructField("x", "Setting for x-translation (-1 - 1)",  BuildinType.Float, typeof(FlightCtrlState).GetField("X")),
                new RecordStructField("y", "Setting for y-translation (-1 - 1)", BuildinType.Float, typeof(FlightCtrlState).GetField("Y")),
                new RecordStructField("z", "Setting for z-translation (-1 - 1)", BuildinType.Float, typeof(FlightCtrlState).GetField("Z")),
                new RecordStructField("pitch", "Setting for pitch rotation (-1 - 1)",  BuildinType.Float, typeof(FlightCtrlState).GetField("pitch")),
                new RecordStructField("yaw", "Setting for yaw rotation (-1 - 1)",  BuildinType.Float, typeof(FlightCtrlState).GetField("yaw")),
                new RecordStructField("roll", "Setting for roll rotation (-1 - 1)",  BuildinType.Float, typeof(FlightCtrlState).GetField("roll")),
                new RecordStructField("pitch_trim", "Current trim value for pitch",  BuildinType.Float, typeof(FlightCtrlState).GetField("pitchTrim")),
                new RecordStructField("yaw_trim", "Current trim value for yaw",  BuildinType.Float, typeof(FlightCtrlState).GetField("yawTrim")),
                new RecordStructField("roll_trim", "Current trim value for roll",  BuildinType.Float, typeof(FlightCtrlState).GetField("rollTrim")),
                new RecordStructField("wheel_throttle", "Setting for wheel throttle (0 - 1, applied to Rovers)",  BuildinType.Float, typeof(FlightCtrlState).GetField("wheelThrottle")),
                new RecordStructField("wheel_steer", "Setting for wheel sterring (-1 - 1, applied to Rovers)",  BuildinType.Float, typeof(FlightCtrlState).GetField("wheelSteer")),
                new RecordStructField("wheel_throttle_trim", "Current trim value for wheel throttle",  BuildinType.Float, typeof(FlightCtrlState).GetField("wheelThrottleTrim")),
                new RecordStructField("wheel_steer_trim", "Current trim value for wheel steering",  BuildinType.Float, typeof(FlightCtrlState).GetField("wheelSteerTrim")),
            },
            BuildinType.NO_OPERATORS,
            BuildinType.NO_OPERATORS,
            new Dictionary<string, IMethodInvokeFactory> {

            },
            new Dictionary<string, IFieldAccessFactory> {
            });
    }
}
