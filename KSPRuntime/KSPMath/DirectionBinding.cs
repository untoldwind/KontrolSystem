using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public static class DirectionBinding {
        public static BoundType DirectionType = Direct.BindType("ksp::math", "Direction", "Represents the rotation from an initial coordinate system when looking down the z-axis and \"up\" being the y-axis", typeof(Direction),
            new OperatorCollection {
                {Operator.Neg, new StaticMethodOperatorEmitter(() => BuildinType.Unit, () => DirectionType, typeof(Direction).GetMethod("op_UnaryNegation", new Type[] { typeof(Vector2d) }))},
                {Operator.Mul, new StaticMethodOperatorEmitter(() => Vector3Binding.Vector3Type, () => Vector3Binding.Vector3Type, typeof(Direction).GetMethod("op_Multiply", new Type[] { typeof(Vector3d), typeof(Direction) }))},
            },
            new OperatorCollection {
                {Operator.Add, new StaticMethodOperatorEmitter(() => DirectionType, () => DirectionType, typeof(Direction).GetMethod("op_Addition", new Type[] { typeof(Direction), typeof(Direction) }))},
                {Operator.AddAssign, new StaticMethodOperatorEmitter(() => DirectionType, () => DirectionType, typeof(Direction).GetMethod("op_Addition", new Type[] { typeof(Direction), typeof(Direction) }))},
                {Operator.Sub, new StaticMethodOperatorEmitter(() => DirectionType, () => DirectionType, typeof(Direction).GetMethod("op_Subtraction", new Type[] { typeof(Direction), typeof(Direction) }))},
                {Operator.SubAssign, new StaticMethodOperatorEmitter(() => DirectionType, () => DirectionType, typeof(Direction).GetMethod("op_Subtraction", new Type[] { typeof(Direction), typeof(Direction) }))},
                {Operator.Mul, new StaticMethodOperatorEmitter(() => DirectionType, () => DirectionType, typeof(Direction).GetMethod("op_Multiply", new Type[] { typeof(Direction), typeof(Direction) }))},
                {Operator.MulAssign, new StaticMethodOperatorEmitter(() => DirectionType, () => DirectionType, typeof(Direction).GetMethod("op_Multiply", new Type[] { typeof(Direction), typeof(Direction) }))},
                {Operator.Mul, new StaticMethodOperatorEmitter(() => Vector3Binding.Vector3Type, () => Vector3Binding.Vector3Type, typeof(Direction).GetMethod("op_Multiply", new Type[] { typeof(Direction), typeof(Vector3d) }))},
                {Operator.Eq, new StaticMethodOperatorEmitter(() => DirectionType, () => BuildinType.Bool, typeof(Direction).GetMethod("op_Equality", new Type[] { typeof(Direction), typeof(Direction) }))},
                {Operator.NotEq, new StaticMethodOperatorEmitter(() => DirectionType, () => BuildinType.Bool, typeof(Direction).GetMethod("op_Equality", new Type[] { typeof(Direction), typeof(Direction) }), OpCodes.Ldc_I4_0, OpCodes.Ceq)},
            },
            new Dictionary<string, IMethodInvokeFactory> {
                {"to_string", new BoundMethodInvokeFactory("Convert the direction to string", () => BuildinType.String, () => new List<RealizedParameter> { }, false, typeof(Direction), typeof(Direction).GetMethod("ToString") )}
            },
            new Dictionary<string, IFieldAccessFactory> {
                {"euler", new BoundPropertyLikeFieldAccessFactory("Euler angles in degree of the rotation", () => Vector3Binding.Vector3Type,  typeof(Direction), typeof(Direction).GetProperty("Euler").GetGetMethod())},
                {"vector", new BoundPropertyLikeFieldAccessFactory("Fore vector of the rotation (i.e. looking/facing direction", () => Vector3Binding.Vector3Type,  typeof(Direction), typeof(Direction).GetProperty("Vector").GetGetMethod())},
                {"up_vector", new BoundPropertyLikeFieldAccessFactory("Up vector of the rotation", () => Vector3Binding.Vector3Type,  typeof(Direction), typeof(Direction).GetProperty("UpVector").GetGetMethod())},
                {"right_vector", new BoundPropertyLikeFieldAccessFactory("Right vector of the rotation", () => Vector3Binding.Vector3Type,  typeof(Direction), typeof(Direction).GetProperty("RightVector").GetGetMethod())},
                {"pitch", new BoundPropertyLikeFieldAccessFactory("Pitch in degree", () => BuildinType.Float,  typeof(Direction), typeof(Direction).GetProperty("Pitch").GetGetMethod())},
                {"yaw", new BoundPropertyLikeFieldAccessFactory("Yaw in degree", () => BuildinType.Float, typeof(Direction), typeof(Direction).GetProperty("Yaw").GetGetMethod())},
                {"roll", new BoundPropertyLikeFieldAccessFactory("Roll in degree", () => BuildinType.Float, typeof(Direction), typeof(Direction).GetProperty("Roll").GetGetMethod())},
            });

        public static Direction look_dir_up(Vector3d lookDirection, Vector3d upDirection) => Direction.LookRotation(lookDirection, upDirection);

        public static Direction euler(double x, double y, double z) => new Direction(new Vector3d(x, y, z), true);

        public static Direction angle_axis(double angle, Vector3d axis) => Direction.AngleAxis(angle, axis);
    }
}
