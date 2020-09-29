using Xunit;
using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Test {
    public class FunctionTypeTests {
        [Fact]
        public void TestMakeAction() {
            Context context = new Context(KontrolRegistry.CreateCore());
            ModuleContext moduleContext = context.CreateModuleContext("Test");

            Assert.Equal(typeof(Func<object>), new FunctionType(false, new List<TO2Type> { }, BuildinType.Unit).GeneratedType(moduleContext));
            Assert.Equal(typeof(Func<long, object>), new FunctionType(false, new List<TO2Type> { BuildinType.Int }, BuildinType.Unit).GeneratedType(moduleContext));
            Assert.Equal(typeof(Func<long, string, object>), new FunctionType(false, new List<TO2Type> { BuildinType.Int, BuildinType.String }, BuildinType.Unit).GeneratedType(moduleContext));
            Assert.Equal(typeof(Func<long, string, bool, object>), new FunctionType(false, new List<TO2Type> { BuildinType.Int, BuildinType.String, BuildinType.Bool }, BuildinType.Unit).GeneratedType(moduleContext));
            Assert.Equal(typeof(Func<long, string, bool, double, object>), new FunctionType(false, new List<TO2Type> { BuildinType.Int, BuildinType.String, BuildinType.Bool, BuildinType.Float }, BuildinType.Unit).GeneratedType(moduleContext));
        }

        [Fact]
        public void TestMakeFunc() {
            Context context = new Context(KontrolRegistry.CreateCore());
            ModuleContext moduleContext = context.CreateModuleContext("Test");

            Assert.Equal(typeof(Func<long>), new FunctionType(false, new List<TO2Type> { }, BuildinType.Int).GeneratedType(moduleContext));
            Assert.Equal(typeof(Func<long, string>), new FunctionType(false, new List<TO2Type> { BuildinType.Int }, BuildinType.String).GeneratedType(moduleContext));
            Assert.Equal(typeof(Func<long, string, bool>), new FunctionType(false, new List<TO2Type> { BuildinType.Int, BuildinType.String }, BuildinType.Bool).GeneratedType(moduleContext));
            Assert.Equal(typeof(Func<long, string, bool, double>), new FunctionType(false, new List<TO2Type> { BuildinType.Int, BuildinType.String, BuildinType.Bool }, BuildinType.Float).GeneratedType(moduleContext));
            Assert.Equal(typeof(Func<long, string, bool, double, long>), new FunctionType(false, new List<TO2Type> { BuildinType.Int, BuildinType.String, BuildinType.Bool, BuildinType.Float }, BuildinType.Int).GeneratedType(moduleContext));
        }
    }
}
