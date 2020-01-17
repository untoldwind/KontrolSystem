using NUnit.Framework;
using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Test {
    [TestFixture]
    public class FunctionTypeTests {
        [Test]
        public void TestMakeAction() {
            Context context = new Context(KontrolRegistry.CreateCore());
            ModuleContext moduleContext = context.CreateModuleContext("Test");

            Assert.AreEqual(typeof(Func<object>), new FunctionType(false, new List<TO2Type> { }, BuildinType.Unit).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(Func<long, object>), new FunctionType(false, new List<TO2Type> { BuildinType.Int }, BuildinType.Unit).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(Func<long, string, object>), new FunctionType(false, new List<TO2Type> { BuildinType.Int, BuildinType.String }, BuildinType.Unit).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(Func<long, string, bool, object>), new FunctionType(false, new List<TO2Type> { BuildinType.Int, BuildinType.String, BuildinType.Bool }, BuildinType.Unit).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(Func<long, string, bool, double, object>), new FunctionType(false, new List<TO2Type> { BuildinType.Int, BuildinType.String, BuildinType.Bool, BuildinType.Float }, BuildinType.Unit).GeneratedType(moduleContext));
        }

        [Test]
        public void TestMakeFunc() {
            Context context = new Context(KontrolRegistry.CreateCore());
            ModuleContext moduleContext = context.CreateModuleContext("Test");

            Assert.AreEqual(typeof(Func<long>), new FunctionType(false, new List<TO2Type> { }, BuildinType.Int).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(Func<long, string>), new FunctionType(false, new List<TO2Type> { BuildinType.Int }, BuildinType.String).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(Func<long, string, bool>), new FunctionType(false, new List<TO2Type> { BuildinType.Int, BuildinType.String }, BuildinType.Bool).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(Func<long, string, bool, double>), new FunctionType(false, new List<TO2Type> { BuildinType.Int, BuildinType.String, BuildinType.Bool }, BuildinType.Float).GeneratedType(moduleContext));
            Assert.AreEqual(typeof(Func<long, string, bool, double, long>), new FunctionType(false, new List<TO2Type> { BuildinType.Int, BuildinType.String, BuildinType.Bool, BuildinType.Float }, BuildinType.Int).GeneratedType(moduleContext));
        }
    }
}
