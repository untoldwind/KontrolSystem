using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public static class Vector2Binding {
        public static RecordStructType Vector2Type = new RecordStructType("ksp::math", "Vec2", "A 2-dimensional vector.", typeof(Vector2d),
            new RecordStructField[] {
                new RecordStructField("x", "x-coordinate", BuildinType.Float, typeof(Vector2d).GetField("x")),
                new RecordStructField("y", "y-coordinate", BuildinType.Float, typeof(Vector2d).GetField("y")),
            },
            new OperatorCollection {
                {Operator.Neg, new StaticMethodOperatorEmitter(() => BuildinType.Unit, () => Vector2Type, typeof(Vector2d).GetMethod("op_UnaryNegation", new Type[] { typeof(Vector2d) }))},
                {Operator.Mul, new StaticMethodOperatorEmitter(() => BuildinType.Float, () => Vector2Type, typeof(Vector2d).GetMethod("op_Multiply", new Type[] { typeof(double), typeof(Vector2d) }))},
            },
            new OperatorCollection {
                {Operator.Add, new StaticMethodOperatorEmitter(() => Vector2Type, () => Vector2Type, typeof(Vector2d).GetMethod("op_Addition", new Type[] { typeof(Vector2d), typeof(Vector2d) }))},
                {Operator.AddAssign, new StaticMethodOperatorEmitter(() => Vector2Type, () => Vector2Type, typeof(Vector2d).GetMethod("op_Addition", new Type[] { typeof(Vector2d), typeof(Vector2d) }))},
                {Operator.Sub, new StaticMethodOperatorEmitter(() => Vector2Type, () => Vector2Type, typeof(Vector2d).GetMethod("op_Subtraction", new Type[] { typeof(Vector2d), typeof(Vector2d) }))},
                {Operator.SubAssign, new StaticMethodOperatorEmitter(() => Vector2Type, () => Vector2Type, typeof(Vector2d).GetMethod("op_Subtraction", new Type[] { typeof(Vector2d), typeof(Vector2d) }))},
                {Operator.Mul, new StaticMethodOperatorEmitter(() => BuildinType.Float, () => Vector2Type, typeof(Vector2d).GetMethod("op_Multiply", new Type[] { typeof(Vector2d), typeof(double) }))},
                {Operator.MulAssign, new StaticMethodOperatorEmitter(() => BuildinType.Float, () => Vector2Type, typeof(Vector2d).GetMethod("op_Multiply", new Type[] { typeof(Vector2d), typeof(double) }))},
                {Operator.Div, new StaticMethodOperatorEmitter(() => BuildinType.Float, () => Vector2Type, typeof(Vector2d).GetMethod("op_Division", new Type[] { typeof(Vector2d), typeof(double) }))},
                {Operator.DivAssign, new StaticMethodOperatorEmitter(() => BuildinType.Float, () => Vector2Type, typeof(Vector2d).GetMethod("op_Division", new Type[] { typeof(Vector2d), typeof(double) }))},
                {Operator.Eq, new StaticMethodOperatorEmitter(() => Vector2Type, () => BuildinType.Bool, typeof(Vector2d).GetMethod("op_Equality", new Type[] { typeof(Vector2d), typeof(Vector2d) }))},
                {Operator.NotEq, new StaticMethodOperatorEmitter(() => Vector2Type, () => BuildinType.Bool, typeof(Vector2d).GetMethod("op_Equality", new Type[] { typeof(Vector2d), typeof(Vector2d) }), OpCodes.Ldc_I4_0, OpCodes.Ceq)},
            },
            new Dictionary<string, IMethodInvokeFactory> {
                {"angle_to", new BoundMethodInvokeFactory("Calculate the angle in degree to `other` vector.", () => BuildinType.Float, () => new List<RealizedParameter> { new RealizedParameter("other", Vector2Type) }, false, typeof(Vector2d), typeof(Vector2d).GetMethod("Angle") )},
                {"to_string", new BoundMethodInvokeFactory("Convert the vector to string", () => BuildinType.String, () => new List<RealizedParameter> { }, false, typeof(Vector2d), typeof(Vector2d).GetMethod("ToString", new Type[0]) )}
            },
            new Dictionary<string, IFieldAccessFactory> {
                {"magnitude", new BoundPropertyLikeFieldAccessFactory("Magnitude/length of the vector", () => BuildinType.Float, typeof(Vector2d), typeof(Vector2d).GetProperty("magnitude").GetGetMethod())},
                {"sqrMagnitude", new BoundPropertyLikeFieldAccessFactory("Squared magnitude of the vector", () => BuildinType.Float, typeof(Vector2d), typeof(Vector2d).GetProperty("sqrMagnitude").GetGetMethod())},
                {"normalized", new BoundPropertyLikeFieldAccessFactory("Normalized vector (i.e. scaled to length 1)", () => Vector2Type, typeof(Vector2d), typeof(Vector2d).GetProperty("normalized").GetGetMethod())}
            });

        public static Vector2d vec2(double x, double y) => new Vector2d(x, y);
    }
}
