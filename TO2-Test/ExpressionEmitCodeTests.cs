using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Parser;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.Test {
    [TestFixture]
    public class ExpressionEmitCodeTests {
        static (object instance, MethodInfo method) GenerateMethod(Expression expression, TO2Type returnType) {
            Context context = new Context(KontrolRegistry.CreateCore());
            ModuleContext moduleContext = context.CreateModuleContext("DynamicExpression");
            IBlockContext methodContext = moduleContext.CreateMethodContext(FunctionModifier.Public, false, "Exec", returnType, Enumerable.Empty<FunctionParameter>());

            expression.EmitCode(methodContext, false);
            Assert.False(methodContext.HasErrors);
            methodContext.IL.EmitReturn(methodContext.MethodBuilder.ReturnType);

            Type dynamicType = moduleContext.CreateType();

            return (Activator.CreateInstance(dynamicType, new object[] { new EmptyContext() }), dynamicType.GetMethod("Exec"));
        }

        [Test]
        public void TestLiteral() {
            (object instance, MethodInfo method) = GenerateMethod(new LiteralInt(1234), BuildinType.Int);
            var result = method.Invoke(instance, new object[0]);

            Assert.AreEqual(1234L, result);

            (instance, method) = GenerateMethod(new LiteralFloat(1234.56), BuildinType.Float);
            result = method.Invoke(instance, new object[0]);

            Assert.AreEqual(1234.56, result);

            (instance, method) = GenerateMethod(new LiteralBool(true), BuildinType.Bool);
            result = method.Invoke(instance, new object[0]);

            Assert.AreEqual(true, result);

            (instance, method) = GenerateMethod(new LiteralString("abcded"), BuildinType.String);
            result = method.Invoke(instance, new object[0]);

            Assert.AreEqual("abcded", result);
        }

        [Test]
        public void TestSimpleCalc() {
            (object instance, MethodInfo method) = GenerateMethod(TO2ParserExpressions.expression.Parse("1234 + 4321"), BuildinType.Int);
            var result = method.Invoke(instance, new object[0]);

            Assert.AreEqual(5555L, result);
        }
    }
}
