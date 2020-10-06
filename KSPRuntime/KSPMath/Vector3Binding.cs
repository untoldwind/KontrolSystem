using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.KSP.Runtime.KSPMath {
    public static class Vector3Binding {
        public static readonly RecordStructType Vector3Type = new RecordStructType("ksp::math", "Vec3",
            "A 3-dimensional vector.", typeof(Vector3d),
            new RecordStructField[] {
                new RecordStructField("x", "x-coordinate", BuiltinType.Float, typeof(Vector3d).GetField("x")),
                new RecordStructField("y", "y-coordinate", BuiltinType.Float, typeof(Vector3d).GetField("y")),
                new RecordStructField("z", "z-coordinate", BuiltinType.Float, typeof(Vector3d).GetField("z")),
            },
            new OperatorCollection {
                {
                    Operator.Neg,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Unit, () => Vector3Type,
                        typeof(Vector3d).GetMethod("op_UnaryNegation", new[] {typeof(Vector3d)}))
                }, {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => Vector3Type,
                        typeof(Vector3d).GetMethod("op_Multiply", new[] {typeof(double), typeof(Vector3d)}))
                },
            },
            new OperatorCollection {
                {
                    Operator.Add,
                    new StaticMethodOperatorEmitter(() => Vector3Type, () => Vector3Type,
                        typeof(Vector3d).GetMethod("op_Addition", new[] {typeof(Vector3d), typeof(Vector3d)}))
                }, {
                    Operator.AddAssign,
                    new StaticMethodOperatorEmitter(() => Vector3Type, () => Vector3Type,
                        typeof(Vector3d).GetMethod("op_Addition", new[] {typeof(Vector3d), typeof(Vector3d)}))
                }, {
                    Operator.Sub,
                    new StaticMethodOperatorEmitter(() => Vector3Type, () => Vector3Type,
                        typeof(Vector3d).GetMethod("op_Subtraction", new[] {typeof(Vector3d), typeof(Vector3d)}))
                }, {
                    Operator.SubAssign,
                    new StaticMethodOperatorEmitter(() => Vector3Type, () => Vector3Type,
                        typeof(Vector3d).GetMethod("op_Subtraction", new[] {typeof(Vector3d), typeof(Vector3d)}))
                }, {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => Vector3Type,
                        typeof(Vector3d).GetMethod("op_Multiply", new[] {typeof(Vector3d), typeof(double)}))
                }, {
                    Operator.Mul,
                    new StaticMethodOperatorEmitter(() => Vector3Type, () => BuiltinType.Float,
                        typeof(Vector3d).GetMethod("Dot"))
                }, {
                    Operator.MulAssign,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => Vector3Type,
                        typeof(Vector3d).GetMethod("op_Multiply", new[] {typeof(Vector3d), typeof(double)}))
                }, {
                    Operator.Div,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => Vector3Type,
                        typeof(Vector3d).GetMethod("op_Division", new[] {typeof(Vector3d), typeof(double)}))
                }, {
                    Operator.DivAssign,
                    new StaticMethodOperatorEmitter(() => BuiltinType.Float, () => Vector3Type,
                        typeof(Vector3d).GetMethod("op_Division", new[] {typeof(Vector3d), typeof(double)}))
                }, {
                    Operator.Eq,
                    new StaticMethodOperatorEmitter(() => Vector3Type, () => BuiltinType.Bool,
                        typeof(Vector3d).GetMethod("op_Equality", new[] {typeof(Vector3d), typeof(Vector3d)}))
                }, {
                    Operator.NotEq,
                    new StaticMethodOperatorEmitter(() => Vector3Type, () => BuiltinType.Bool,
                        typeof(Vector3d).GetMethod("op_Equality", new[] {typeof(Vector3d), typeof(Vector3d)}),
                        OpCodes.Ldc_I4_0, OpCodes.Ceq)
                },
            },
            new Dictionary<string, IMethodInvokeFactory> {
                {
                    "cross",
                    new BoundMethodInvokeFactory("Calculate the cross/other product with `other` vector.",
                        () => Vector3Type,
                        () => new List<RealizedParameter> {new RealizedParameter("other", Vector3Type)}, false,
                        typeof(Vector3d), typeof(Vector3d).GetMethod("Cross"))
                }, {
                    "dot",
                    new BoundMethodInvokeFactory("Calculate the dot/inner product with `other` vector.",
                        () => BuiltinType.Float,
                        () => new List<RealizedParameter> {new RealizedParameter("other", Vector3Type)}, false,
                        typeof(Vector3d), typeof(Vector3d).GetMethod("Dot"))
                }, {
                    "angle_to",
                    new BoundMethodInvokeFactory("Calculate the angle in degree to `other` vector.",
                        () => BuiltinType.Float,
                        () => new List<RealizedParameter> {new RealizedParameter("other", Vector3Type)}, false,
                        typeof(Vector3d), typeof(Vector3d).GetMethod("Angle"))
                }, {
                    "lerp_to",
                    new BoundMethodInvokeFactory(
                        "Linear interpolate position between this and `other` vector, where `t = 0.0` is this and `t = 1.0` is `other`.",
                        () => Vector3Type,
                        () => new List<RealizedParameter> {
                            new RealizedParameter("other", Vector3Type), new RealizedParameter("t", BuiltinType.Float)
                        }, false, typeof(Vector3d), typeof(Vector3d).GetMethod("Lerp"))
                }, {
                    "project_to",
                    new BoundMethodInvokeFactory("Project this on `other` vector.", () => Vector3Type,
                        () => new List<RealizedParameter> {new RealizedParameter("other", Vector3Type)}, false,
                        typeof(Vector3d), typeof(Vector3d).GetMethod("Project"))
                }, {
                    "distance_to",
                    new BoundMethodInvokeFactory("Calculate the distance between this and `other` vector.",
                        () => Vector3Type,
                        () => new List<RealizedParameter> {new RealizedParameter("other", Vector3Type)}, false,
                        typeof(Vector3d), typeof(Vector3d).GetMethod("Distance"))
                }, {
                    "exclude_from",
                    new BoundMethodInvokeFactory("Exclude this from `other` vector.", () => Vector3Type,
                        () => new List<RealizedParameter> {new RealizedParameter("other", Vector3Type)}, false,
                        typeof(Vector3d), typeof(Vector3d).GetMethod("Exclude"))
                }, {
                    "to_string",
                    new BoundMethodInvokeFactory("Convert vector to string.", () => BuiltinType.String,
                        () => new List<RealizedParameter>(), false, typeof(Vector3d),
                        typeof(Vector3d).GetMethod("ToString", new Type[0]))
                }
            },
            new Dictionary<string, IFieldAccessFactory> {
                {
                    "magnitude",
                    new BoundPropertyLikeFieldAccessFactory("Magnitude/length of the vector", () => BuiltinType.Float,
                        typeof(Vector3d), typeof(Vector3d).GetProperty("magnitude")?.GetGetMethod())
                }, {
                    "sqrMagnitude",
                    new BoundPropertyLikeFieldAccessFactory("Squared magnitude of the vector", () => BuiltinType.Float,
                        typeof(Vector3d), typeof(Vector3d).GetProperty("sqrMagnitude")?.GetGetMethod())
                }, {
                    "normalized",
                    new BoundPropertyLikeFieldAccessFactory("Normalized vector (i.e. scaled to length 1)",
                        () => Vector3Type, typeof(Vector3d), typeof(Vector3d).GetProperty("normalized")?.GetGetMethod())
                }, {
                    "xzy",
                    new BoundPropertyLikeFieldAccessFactory("Swapped y- and z-coordinate", () => Vector3Type,
                        typeof(Vector3d), typeof(Vector3d).GetProperty("xzy")?.GetGetMethod())
                }
            });

        public static Vector3d vec3(double x, double y, double z) => new Vector3d(x, y, z);
    }
}
