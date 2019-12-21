using NUnit.Framework;
using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Test {
    [TestFixture]
    public class TupleTypeTests {
        [Test]
        public void MakeShortTuple() {
            Context context = new Context(KontrolRegistry.CreateCore());
            ModuleContext moduleContext = context.CreateModuleContext("Test");

            Assert.AreEqual(typeof(ValueTuple<string>), new TupleType(new List<TO2Type> { BuildinType.String }).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(ValueTuple<long, string>), new TupleType(new List<TO2Type> { BuildinType.Int, BuildinType.String }).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(ValueTuple<long, string, double>), new TupleType(new List<TO2Type> { BuildinType.Int, BuildinType.String, BuildinType.Float }).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(ValueTuple<long, string, double, bool>), new TupleType(new List<TO2Type> { BuildinType.Int, BuildinType.String, BuildinType.Float, BuildinType.Bool }).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(ValueTuple<double, long, string, double, bool>), new TupleType(new List<TO2Type> { BuildinType.Float, BuildinType.Int, BuildinType.String, BuildinType.Float, BuildinType.Bool }).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(ValueTuple<double, long, string, double, bool, string, long>), new TupleType(new List<TO2Type> { BuildinType.Float, BuildinType.Int, BuildinType.String, BuildinType.Float, BuildinType.Bool, BuildinType.String, BuildinType.Int }).GeneratedType(moduleContext));
        }

        public void MakeLongTuple() {
            Context context = new Context(KontrolRegistry.CreateCore());
            ModuleContext moduleContext = context.CreateModuleContext("Test");

            Assert.AreEqual(typeof(ValueTuple<double, long, string, double, bool, string, long, ValueTuple<string>>),
                            new TupleType(new List<TO2Type> { BuildinType.Float, BuildinType.Int, BuildinType.String, BuildinType.Float, BuildinType.Bool, BuildinType.String, BuildinType.Int, BuildinType.String }).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(ValueTuple<double, long, string, double, bool, string, long, ValueTuple<string, bool, long, double, bool, long, double>>),
                            new TupleType(new List<TO2Type> { BuildinType.Float, BuildinType.Int, BuildinType.String, BuildinType.Float, BuildinType.Bool, BuildinType.String, BuildinType.Int,
                                          BuildinType.String, BuildinType.Bool, BuildinType.Int, BuildinType.Float, BuildinType.Bool, BuildinType.Int, BuildinType.Float
                                                            }).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(ValueTuple<double, long, string, double, bool, string, long, ValueTuple<string, bool, long, double, bool, long, double, ValueTuple<string, bool>>>),
                            new TupleType(new List<TO2Type> { BuildinType.Float, BuildinType.Int, BuildinType.String, BuildinType.Float, BuildinType.Bool, BuildinType.String, BuildinType.Int,
                                          BuildinType.String, BuildinType.Bool, BuildinType.Int, BuildinType.Float, BuildinType.Bool, BuildinType.Int, BuildinType.Float,
                                          BuildinType.String, BuildinType.Bool
                                                            }).GeneratedType(moduleContext));
        }
    }
}
