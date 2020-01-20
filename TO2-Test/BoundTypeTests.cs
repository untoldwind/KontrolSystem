using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Test {
    public class NonGeneric {
    }

    public class SimpleGeneric<T> {
    }

    [TestFixture]
    public class BoundTypeTests {
        [Test]
        public void TestNonGeneric() {
            BoundType type = new BoundType("module", "NonGeneric", "", typeof(NonGeneric),
                BuildinType.NO_OPERATORS,
                BuildinType.NO_OPERATORS,
                Enumerable.Empty<(string name, IMethodInvokeFactory invoker)>(),
                Enumerable.Empty<(string name, IFieldAccessFactory access)>()
            );
            Context context = new Context(KontrolRegistry.CreateCore());
            ModuleContext moduleContext = context.CreateModuleContext("Test");

            Assert.AreEqual("NonGeneric", type.LocalName);
            Assert.AreEqual("module::NonGeneric", type.Name);
            Assert.IsTrue(type.IsValid(moduleContext));
            Assert.IsEmpty(type.GenericParameters);

            Assert.AreEqual(typeof(NonGeneric), type.GeneratedType(moduleContext));
        }

        [Test]
        public void TestSimpleGeneric() {
            BoundType type = new BoundType("module", "SimpleGeneric", "", typeof(SimpleGeneric<>),
                BuildinType.NO_OPERATORS,
                BuildinType.NO_OPERATORS,
                Enumerable.Empty<(string name, IMethodInvokeFactory invoker)>(),
                Enumerable.Empty<(string name, IFieldAccessFactory access)>()
            );
            Context context = new Context(KontrolRegistry.CreateCore());
            ModuleContext moduleContext = context.CreateModuleContext("Test");

            Assert.AreEqual("SimpleGeneric", type.LocalName);
            Assert.AreEqual("module::SimpleGeneric<T>", type.Name);
            Assert.AreEqual(new string[] { "T" }, type.GenericParameters);
            Assert.IsFalse(type.IsValid(moduleContext));
            Assert.AreEqual(typeof(SimpleGeneric<>), type.GeneratedType(moduleContext));

            RealizedType filledType = type.FillGenerics(moduleContext, new Dictionary<string, RealizedType> {
                { "T", BuildinType.Int }
            });

            Assert.AreEqual("SimpleGeneric", filledType.LocalName);
            Assert.AreEqual("module::SimpleGeneric<int>", filledType.Name);
            Assert.IsTrue(filledType.IsValid(moduleContext));
            Assert.IsEmpty(filledType.GenericParameters);
            Assert.AreEqual(typeof(SimpleGeneric<long>), filledType.GeneratedType(moduleContext));

            RealizedType aliased = type.FillGenerics(moduleContext, new Dictionary<string, RealizedType> {
                { "T", new GenericParameter("U") }
            });

            Assert.AreEqual("SimpleGeneric", aliased.LocalName);
            Assert.AreEqual("module::SimpleGeneric<U>", aliased.Name);
            Assert.IsFalse(aliased.IsValid(moduleContext));
            Assert.AreEqual(new string[] { "U" }, aliased.GenericParameters);
            Assert.AreEqual(typeof(SimpleGeneric<>), aliased.GeneratedType(moduleContext));

            RealizedType filledType2 = aliased.FillGenerics(moduleContext, new Dictionary<string, RealizedType> {
                { "U", BuildinType.String }
            });

            Assert.AreEqual("SimpleGeneric", filledType2.LocalName);
            Assert.AreEqual("module::SimpleGeneric<string>", filledType2.Name);
            Assert.IsTrue(filledType2.IsValid(moduleContext));
            Assert.IsEmpty(filledType2.GenericParameters);
            Assert.AreEqual(typeof(SimpleGeneric<string>), filledType2.GeneratedType(moduleContext));

            Assert.AreEqual(new Dictionary<string, RealizedType> {
                { "T", BuildinType.Int }
            }, type.InferGenericArgument(moduleContext, filledType).ToDictionary(t => t.name, t => t.type));
            Assert.AreEqual(new Dictionary<string, RealizedType> {
                { "U", BuildinType.Int }
            }, aliased.InferGenericArgument(moduleContext, filledType).ToDictionary(t => t.name, t => t.type));
            Assert.AreEqual(new Dictionary<string, RealizedType> {
                { "T", BuildinType.String }
            }, type.InferGenericArgument(moduleContext, filledType2).ToDictionary(t => t.name, t => t.type));
            Assert.AreEqual(new Dictionary<string, RealizedType> {
                { "U", BuildinType.String }
            }, aliased.InferGenericArgument(moduleContext, filledType2).ToDictionary(t => t.name, t => t.type));

        }
    }
}
