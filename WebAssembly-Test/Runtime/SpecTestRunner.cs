﻿using Xunit;
using Xunit.Sdk;
using Xunit.Abstractions;
using JsonSubTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using WebAssembly.Runtime;

namespace WebAssembly.Test.Runtime {
    class SpecTestRunner {
        private readonly ITestOutputHelper _testOutputHelper;
        public SpecTestRunner(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
        }

        public void Run(string pathBase, string json) => Run<object>(pathBase, json);

        public void Run(string pathBase, string json, Func<uint, bool>? skip) => Run<object>(pathBase, json, skip);

        public void Run<TExports>(string pathBase, string json)
            where TExports : class {
            Run<TExports>(pathBase, json, null);
        }

        public void Run<TExports>(string pathBase, string json, Func<uint, bool>? skip)
            where TExports : class {
            TestInfo testInfo;
            using (var reader = new StreamReader(Path.Combine(pathBase, json))) {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(JsonSubtypesConverterBuilder
                    .Of(typeof(Command), "type")
                    .RegisterSubtype(typeof(ModuleCommand), CommandType.module)
                    .RegisterSubtype(typeof(AssertReturn), CommandType.assert_return)
                    .RegisterSubtype(typeof(AssertReturnCanonicalNan), CommandType.assert_return_canonical_nan)
                    .RegisterSubtype(typeof(AssertReturnArithmeticNan), CommandType.assert_return_arithmetic_nan)
                    .RegisterSubtype(typeof(AssertInvalid), CommandType.assert_invalid)
                    .RegisterSubtype(typeof(AssertTrap), CommandType.assert_trap)
                    .RegisterSubtype(typeof(AssertMalformed), CommandType.assert_malformed)
                    .RegisterSubtype(typeof(AssertExhaustion), CommandType.assert_exhaustion)
                    .RegisterSubtype(typeof(AssertUnlinkable), CommandType.assert_unlinkable)
                    .RegisterSubtype(typeof(Register), CommandType.register)
                    .RegisterSubtype(typeof(AssertReturn), CommandType.action)
                    .RegisterSubtype(typeof(AssertUninstantiable), CommandType.assert_uninstantiable)
                    .Build());
                settings.Converters.Add(JsonSubtypesConverterBuilder
                    .Of(typeof(TestAction), "type")
                    .RegisterSubtype(typeof(Invoke), TestActionType.invoke)
                    .RegisterSubtype(typeof(Get), TestActionType.get)
                    .Build());
                settings.Converters.Add(JsonSubtypesConverterBuilder
                    .Of(typeof(TypedValue), "type")
                    .RegisterSubtype(typeof(Int32Value), RawValueType.i32)
                    .RegisterSubtype(typeof(Int64Value), RawValueType.i64)
                    .RegisterSubtype(typeof(Float32Value), RawValueType.f32)
                    .RegisterSubtype(typeof(Float64Value), RawValueType.f64)
                    .Build());
                testInfo = (TestInfo)JsonSerializer.Create(settings).Deserialize(reader, typeof(TestInfo))!;
            }

            ObjectMethods? methodsByName = null;
            var moduleMethodsByName = new Dictionary<string, ObjectMethods>();

            // From https://github.com/WebAssembly/spec/blob/master/interpreter/host/spectest.ml
            var imports = new ImportDictionary
                {
                    { "spectest", "print_i32", new FunctionImport((Action<int>)(i => { })) },
                    { "spectest", "print_i32_f32", new FunctionImport((Action<int, float>)((i, f) => { })) },
                    { "spectest", "print_f64_f64", new FunctionImport((Action<double, double>)((d1, d2) => { })) },
                    { "spectest", "print_f32", new FunctionImport((Action<float>)(i => { })) },
                    { "spectest", "print_f64", new FunctionImport((Action<double>)(i => { })) },
                    { "spectest", "global_i32", new GlobalImport(() => 666) },
                    { "spectest", "global_i64", new GlobalImport(() => 666L) },
                    { "spectest", "global_f32", new GlobalImport(() => 666.0F) },
                    { "spectest", "global_f64", new GlobalImport(() => 666.0) },
                    { "spectest", "table", new FunctionTable(10, 20) }, // Table.alloc (TableType ({min = 10l; max = Some 20l}, FuncRefType))
                    { "spectest", "memory", new MemoryImport(() => new UnmanagedMemory(1, 2)) }, // Memory.alloc (MemoryType {min = 1l; max = Some 2l})
            };

            var registrationCandidates = new ImportDictionary();

            Action trapExpected;
            object? result;
            object obj;
            MethodInfo methodInfo;
            TExports? exports = null;
            foreach (var command in testInfo.commands) {
                if (skip != null && skip(command.line))
                    continue;

                void GetMethod(TestAction action, out MethodInfo info, out object host) {
                    var methodSource = action.module == null ? methodsByName : moduleMethodsByName[action.module];
                    Assert.True(methodSource != null, $"{command.line} has no method source.");
                    Assert.True(methodSource!.TryGetValue(NameCleaner.CleanName(action.field), out info!), $"{command.line} failed to look up method {action.field}");
                    host = methodSource.Host;
                }

                try {
                    switch (command) {
                    case ModuleCommand module:
                        var path = Path.Combine(pathBase, module.filename);
                        var parsed = Module.ReadFromBinary(path); // Ensure the module parser can read it.
                        Assert.NotNull(parsed);
                        methodsByName = new ObjectMethods(exports = Compile.FromBinary<TExports>(path)(imports).Exports);
                        if (module.name != null)
                            moduleMethodsByName[module.name] = methodsByName;
                        continue;
                    case AssertReturn assert:
                        GetMethod(assert.action, out methodInfo, out obj);
                        try {
                            result = assert.action.Call(methodInfo, obj);
                        } catch (TargetInvocationException x) when (x.InnerException != null) {
                            throw new XunitException($"{command.line}: {x.InnerException.Message}");
                        } catch (Exception x) {
                            throw new XunitException($"{command.line}: {x.Message}");
                        }
                        if (assert.expected?.Length > 0) {
                            if (assert.expected[0].BoxedValue.Equals(result))
                                continue;

                            switch (assert.expected[0].type) {
                            case RawValueType.f32: {
                                var expected = ((Float32Value)assert.expected[0]).ActualValue;
                                Assert.True(Math.Abs(expected - (float)result!) < Math.Abs(expected * 0.000001f), $"{command.line}: f32 compare");
                            }
                            continue;
                            case RawValueType.f64: {
                                var expected = ((Float64Value)assert.expected[0]).ActualValue;
                                Assert.True(Math.Abs(expected - (double)result!) < Math.Abs(expected * 0.000001), $"{command.line}: f64 compare");
                            }
                            continue;
                            }

                            throw new XunitException($"{command.line}: Not equal: {assert.expected[0].BoxedValue} and {result}");
                        }
                        continue;
                    case AssertReturnCanonicalNan assert:
                        GetMethod(assert.action, out methodInfo, out obj);
                        result = assert.action.Call(methodInfo, obj);
                        switch (assert.expected[0].type) {
                        case RawValueType.f32:
                            Assert.True(float.IsNaN((float)result!), $"{command.line}: Expected NaN, got {result}");
                            continue;
                        case RawValueType.f64:
                            Assert.True(double.IsNaN((double)result!), $"{command.line}: Expected NaN, got {result}");
                            continue;
                        default:
                            throw new XunitException($"{assert.expected[0].type} doesn't support NaN checks.");
                        }
                    case AssertReturnArithmeticNan assert:
                        GetMethod(assert.action, out methodInfo, out obj);
                        result = assert.action.Call(methodInfo, obj);
                        switch (assert.expected[0].type) {
                        case RawValueType.f32:
                            Assert.True(float.IsNaN((float)result!), $"{command.line}: Expected NaN, got {result}");
                            continue;
                        case RawValueType.f64:
                            Assert.True(double.IsNaN((double)result!), $"{command.line}: Expected NaN, got {result}");
                            continue;
                        default:
                            throw new XunitException($"{assert.expected[0].type} doesn't support NaN checks.");
                        }
                    case AssertInvalid assert:
                        trapExpected = () => {
                            try {
                                Compile.FromBinary<TExports>(Path.Combine(pathBase, assert.filename));
                            } catch (TargetInvocationException x) when (x.InnerException != null) {
                                ExceptionDispatchInfo.Capture(x.InnerException).Throw();
                            }
                        };
                        switch (assert.text) {
                        case "type mismatch":
                            try {
                                trapExpected();
                            } catch (StackTypeInvalidException) {
                                continue;
                            } catch (StackTooSmallException) {
                                continue;
                            } catch (ModuleLoadException) {
                                continue;
                            } catch (StackSizeIncorrectException) {
                                continue;
                            } catch (LabelTypeMismatchException) {
                                continue;
                            } catch (Exception x) {
                                throw new XunitException($"{command.line} threw an unexpected exception of type {x.GetType().Name}.");
                            }
                            throw new XunitException($"{command.line} should have thrown an exception but did not.");
                        case "alignment must not be larger than natural":
                        case "global is immutable":
                            Assert.Throws<CompilerException>(trapExpected);
                            continue;
                        case "unknown memory 0":
                        case "constant expression required":
                        case "duplicate export name":
                        case "unknown table":
                        case "unknown local":
                        case "multiple memories":
                        case "size minimum must not be greater than maximum":
                        case "memory size must be at most 65536 pages (4GiB)":
                        case "unknown label":
                        case "invalid result arity":
                        case "unknown type":
                            Assert.Throws<ModuleLoadException>(trapExpected);
                            continue;
                        case "unknown global":
                        case "unknown memory":
                        case "unknown function":
                        case "unknown table 0":
                            try {
                                trapExpected();
                            } catch (CompilerException) {
                                continue;
                            } catch (ModuleLoadException) {
                                continue;
                            } catch (Exception x) {
                                throw new XunitException($"{command.line} threw an unexpected exception of type {x.GetType().Name}.");
                            }
                            throw new XunitException($"{command.line} should have thrown an exception but did not.");
                        case "multiple tables":
                            Assert.Throws<ModuleLoadException>(trapExpected);
                            continue;
                        default:
                            throw new XunitException($"{command.line}: {assert.text} doesn't have a test procedure set up.");
                        }
                    case AssertTrap assert:
                        trapExpected = () => {
                            GetMethod(assert.action, out methodInfo, out obj);
                            try {
                                assert.action.Call(methodInfo, obj);
                            } catch (TargetInvocationException x) when (x.InnerException != null) {
                                ExceptionDispatchInfo.Capture(x.InnerException).Throw();
                            }
                        };

                        switch (assert.text) {
                        case "integer divide by zero":
                            Assert.Throws<DivideByZeroException>(trapExpected);
                            continue;
                        case "integer overflow":
                            Assert.Throws<OverflowException>(trapExpected);
                            continue;
                        case "out of bounds memory access":
                            try {
                                trapExpected();
                            } catch (MemoryAccessOutOfRangeException) {
                                continue;
                            } catch (OverflowException) {
                                continue;
                            } catch (Exception x) {
                                throw new XunitException($"{command.line} threw an unexpected exception of type {x.GetType().Name}.");
                            }
                            throw new XunitException($"{command.line} should have thrown an exception but did not.");
                        case "invalid conversion to integer":
                            Assert.Throws<OverflowException>(trapExpected);
                            continue;
                        case "undefined element":
                        case "uninitialized element 7":
                            try {
                                trapExpected();
                                throw new XunitException($"{command.line}: Expected ModuleLoadException or IndexOutOfRangeException, but no exception was thrown.");
                            } catch (ModuleLoadException) {
                            } catch (IndexOutOfRangeException) {
                            } catch (Exception x) {
                                throw new XunitException($"{command.line}: Expected ModuleLoadException or IndexOutOfRangeException, but received {x.GetType().Name}.");
                            }
                            continue;
                        case "indirect call type mismatch":
                            Assert.Throws<InvalidCastException>(trapExpected);
                            continue;
                        case "unreachable":
                            Assert.Throws<UnreachableException>(trapExpected);
                            continue;
                        case "uninitialized element":
                        case "uninitialized":
                            try {
                                trapExpected();
                                throw new XunitException($"{command.line}: Expected KeyNotFoundException or NullReferenceException, but no exception was thrown.");
                            } catch (KeyNotFoundException) {
                            } catch (NullReferenceException) {
                            } catch (Exception x) {
                                throw new XunitException($"{command.line}: Expected KeyNotFoundException or NullReferenceException, but received {x.GetType().Name}.");
                            }
                            continue;
                        case "undefined":
                            try {
                                trapExpected();
                                throw new XunitException($"{command.line}: Expected KeyNotFoundException or IndexOutOfRangeException, but no exception was thrown.");
                            } catch (KeyNotFoundException) {
                            } catch (IndexOutOfRangeException) {
                            } catch (Exception x) {
                                throw new XunitException($"{command.line}: Expected KeyNotFoundException or IndexOutOfRangeException, but received {x.GetType().Name}.");
                            }
                            continue;
                        case "indirect call":
                            Assert.Throws<InvalidCastException>(trapExpected);
                            continue;
                        default:
                            throw new XunitException($"{command.line}: {assert.text} doesn't have a test procedure set up.");
                        }
                    case AssertMalformed assert:
                        continue; // Not writing a WAT parser.
                    case AssertExhaustion assert:
                        trapExpected = () => {
                            GetMethod(assert.action, out methodInfo, out obj);
                            try {
                                assert.action.Call(methodInfo, obj);
                            } catch (TargetInvocationException x) when (x.InnerException != null) {
                                ExceptionDispatchInfo.Capture(x.InnerException).Throw();
                            }
                        };

                        switch (assert.text) {
                        case "call stack exhausted":
                            Assert.Throws<StackOverflowException>(trapExpected);
                            continue;
                        default:
                            throw new XunitException($"{command.line}: {assert.text} doesn't have a test procedure set up.");
                        }
                    case AssertUnlinkable assert:
                        trapExpected = () => {
                            try {
                                Compile.FromBinary<TExports>(Path.Combine(pathBase, assert.filename))(imports);
                            } catch (TargetInvocationException x) when (x.InnerException != null
#if DEBUG
                                && !System.Diagnostics.Debugger.IsAttached
#endif
                                ) {
                                ExceptionDispatchInfo.Capture(x.InnerException).Throw();
                            }
                        };
                        switch (assert.text) {
                        case "data segment does not fit":
                        case "elements segment does not fit":
                            try {
                                trapExpected();
                                throw new XunitException($"{command.line}: Expected ModuleLoadException, MemoryAccessOutOfRangeException or OverflowException, but no exception was thrown.");
                            } catch (Exception x) when (x is ModuleLoadException || x is MemoryAccessOutOfRangeException || x is OverflowException) {
                            }
                            continue;
                        case "unknown import":
                        case "incompatible import type":
                            Assert.Throws<ImportException>(trapExpected);
                            continue;
                        default:
                            throw new XunitException($"{command.line}: {assert.text} doesn't have a test procedure set up.");
                        }
                    case Register register:
                        Assert.NotNull(
                            moduleMethodsByName[register.@as] = register.name != null ? moduleMethodsByName[register.name] : methodsByName!);
                        Assert.NotNull(exports);
                        imports.AddFromExports(register.@as, exports!);
                        continue;
                    case AssertUninstantiable assert:
                        trapExpected = () => {
                            try {
                                Compile.FromBinary<TExports>(Path.Combine(pathBase, assert.filename));
                            } catch (TargetInvocationException x) when (x.InnerException != null) {
                                ExceptionDispatchInfo.Capture(x.InnerException).Throw();
                            }
                        };
                        switch (assert.text) {
                        case "unreachable":
                            Assert.Throws<ModuleLoadException>(trapExpected);
                            continue;
                        default:
                            throw new XunitException($"{command.line}: {assert.text} doesn't have a test procedure set up.");
                        }
                    default:
                        throw new XunitException($"{command.line}: {command} doesn't have a test procedure set up.");
                    }
                } catch (Exception x) when (!System.Diagnostics.Debugger.IsAttached && !(x is XunitException)) {
                    throw new XunitException($"{command.line}: {x}");
                }
            }

            if (skip != null)
                _testOutputHelper.WriteLine("Some scenarios were skipped.");
        }

        class ObjectMethods : Dictionary<string, MethodInfo> {
            public readonly object Host;

            public ObjectMethods(object host) {
                Assert.NotNull(this.Host = host);

                foreach (var method in host
                    .GetType()
                    .GetMethods()
                    .Select(m => new { m.Name, MethodInfo = m })
                    .Concat(host
                    .GetType()
                    .GetProperties()
                    .Where(p => p.GetGetMethod() != null)
                    .Select(p => new { p.Name, MethodInfo = p.GetGetMethod()! }))) {
                    Assert.True(TryAdd(method.Name, method.MethodInfo));
                }
            }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        enum CommandType {
            module,
            assert_return,
            assert_return_canonical_nan,
            assert_return_arithmetic_nan,
            assert_invalid,
            assert_trap,
            assert_malformed,
            assert_exhaustion,
            assert_unlinkable,
            register,
            action,
            assert_uninstantiable
        }

#pragma warning disable 649
        class TestInfo {
            public string source_filename;
            public Command[] commands;
        }

        abstract class Command {
            public CommandType type;
            public uint line;

            public override string ToString() => type.ToString();
        }

        class ModuleCommand : Command {
            public string name;
            public string filename;

            public override string ToString() => $"{base.ToString()}: {filename}";
        }

        [JsonConverter(typeof(StringEnumConverter))]
        enum RawValueType {
            i32 = WebAssemblyValueType.Int32,
            i64 = WebAssemblyValueType.Int64,
            f32 = WebAssemblyValueType.Float32,
            f64 = WebAssemblyValueType.Float64,
        }

        class TypeOnly {
            public RawValueType type;

            public override string ToString() => type.ToString();
        }

        abstract class TypedValue : TypeOnly {
            public abstract object BoxedValue { get; }
        }

        class Int32Value : TypedValue {
            public uint value;

            public override object BoxedValue => (int)value;

            public override string ToString() => $"{type}: {value}";
        }

        class Int64Value : TypedValue {
            public ulong value;

            public override object BoxedValue => (long)value;

            public override string ToString() => $"{type}: {value}";
        }

        class Float32Value : Int32Value {
            public float ActualValue => Int32BitsToSingle(unchecked((int)value));

            public override object BoxedValue => ActualValue;

            public override string ToString() => $"{type}: {BoxedValue}";

            static unsafe float Int32BitsToSingle(int value) => *(float*)&value;
        }

        class Float64Value : Int64Value {
            public double ActualValue => BitConverter.Int64BitsToDouble(unchecked((long)value));

            public override object BoxedValue => ActualValue;

            public override string ToString() => $"{type}: {BoxedValue}";
        }

        [JsonConverter(typeof(StringEnumConverter))]
        enum TestActionType {
            invoke,
            get,
        }

        abstract class TestAction {
            public TestActionType type;
            public string module;
            public string field;

            public abstract object? Call(MethodInfo methodInfo, object obj);
        }

        class Invoke : TestAction {
            public TypedValue[] args;

            public override object? Call(MethodInfo methodInfo, object obj) {
                return methodInfo.Invoke(obj, args.Select(arg => arg.BoxedValue).ToArray());
            }

            public override string ToString() => $"{field}({module})[{string.Join(",", (IEnumerable<TypedValue>)args)}]";
        }

        class Get : TestAction {
            public override object? Call(MethodInfo methodInfo, object obj) {
                return methodInfo.Invoke(obj, Array.Empty<object>());
            }
        }

        abstract class AssertCommand : Command {
            public TestAction action;

            public override string ToString() => $"{base.ToString()}: {action}";
        }

        class AssertReturn : AssertCommand {
            public TypedValue[] expected;

            public override string ToString() => $"{base.ToString()} = [{string.Join(",", (IEnumerable<TypedValue>)expected)}]";
        }

        class AssertReturnCanonicalNan : AssertCommand {
            public TypeOnly[] expected;

            public override string ToString() => $"{base.ToString()} = [{string.Join(",", (IEnumerable<TypeOnly>)expected)}]";
        }

        class AssertReturnArithmeticNan : AssertCommand {
            public TypeOnly[] expected;

            public override string ToString() => $"{base.ToString()} = [{string.Join(",", (IEnumerable<TypeOnly>)expected)}]";
        }

        abstract class InvalidCommand : Command {
            public string filename;
            public string text;
            public string module_type;

            public override string ToString() => $"{base.ToString()}: {filename} \"{text}\" {module_type}";
        }

        class AssertInvalid : InvalidCommand {
        }

        class AssertTrap : AssertCommand {
            public TypeOnly[] expected;
            public string text;
        }

        class AssertMalformed : InvalidCommand {
        }

        class AssertExhaustion : AssertCommand {
            public string text;
        }

        class AssertUnlinkable : InvalidCommand {
        }

        class AssertUninstantiable : InvalidCommand {
        }

        class Register : Command {
            public string name;
            public string @as;
        }
#pragma warning restore
    }

}
