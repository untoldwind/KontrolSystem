using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2 {
    public interface IDefaultValue {
        void EmitCode(IBlockContext context);
    }

    public static class DefaultValue {
        public static IDefaultValue ForParameter(IBlockContext context, FunctionParameter parameter) {
            if (parameter.defaultValue == null) return null;
            switch (parameter.defaultValue) {
            case LiteralBool b when parameter.type == BuildinType.Bool: return new BoolDefaultValue(b.value);
            case LiteralInt i when parameter.type == BuildinType.Int: return new IntDefaultValue(i.value);
            case LiteralInt i when parameter.type == BuildinType.Float: return new FloatDefaultValue(i.value);
            case LiteralFloat f when parameter.type == BuildinType.Float: return new FloatDefaultValue(f.value);
            case LiteralString s when parameter.type == BuildinType.String: return new StringDefaultValue(s.value);
            default:
                IBlockContext defaultContext = new SyncBlockContext(context.ModuleContext, context.ModuleField, FunctionModifier.Public, false, $"default_{context.MethodBuilder.Name}_{parameter.name}", parameter.type, Enumerable.Empty<FunctionParameter>());
                TO2Type resultType = parameter.defaultValue.ResultType(defaultContext);

                if (!parameter.type.IsAssignableFrom(context.ModuleContext, resultType)) {
                    context.AddError(new StructuralError(
                                        StructuralError.ErrorType.IncompatibleTypes,
                                        $"Default value of parameter {parameter.name} has to be of type {parameter.type}, found {resultType}",
                                        parameter.Start,
                                        parameter.End
                                    ));
                    return null;
                }

                parameter.defaultValue.EmitCode(defaultContext, false);
                parameter.type.AssignFrom(context.ModuleContext, resultType).EmitConvert(context);
                defaultContext.IL.EmitReturn(parameter.type.GeneratedType(context.ModuleContext));

                foreach (StructuralError error in defaultContext.AllErrors) context.AddError(error);

                return new DefaultValueFactoryFunction(context.ModuleContext.moduleName, defaultContext.MethodBuilder);
            }
        }
    }

    public class BoolDefaultValue : IDefaultValue {
        private readonly bool value;

        public BoolDefaultValue(bool _value) => value = _value;

        public void EmitCode(IBlockContext context) => context.IL.Emit(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
    }

    public class IntDefaultValue : IDefaultValue {
        private readonly long value;

        public IntDefaultValue(long _value) => value = _value;

        public void EmitCode(IBlockContext context) => context.IL.Emit(OpCodes.Ldc_I8, value);
    }

    public class FloatDefaultValue : IDefaultValue {
        private readonly double value;

        public FloatDefaultValue(double _value) => value = _value;

        public void EmitCode(IBlockContext context) => context.IL.Emit(OpCodes.Ldc_R8, value);
    }

    public class StringDefaultValue : IDefaultValue {
        private readonly string value;

        public StringDefaultValue(string _value) => value = _value;

        public void EmitCode(IBlockContext context) => context.IL.Emit(OpCodes.Ldstr, value);
    }

    public class DefaultValueFactoryFunction : IDefaultValue {
        private readonly string moduleName;
        private readonly MethodInfo method;

        public DefaultValueFactoryFunction(string _moduleName, MethodInfo _method) {
            moduleName = _moduleName;
            method = _method;
        }

        public void EmitCode(IBlockContext context) {
            IKontrolModule module = context.ModuleContext.FindModule(moduleName);

            context.IL.Emit(OpCodes.Ldarg_0);

            if (module.Name != context.ModuleContext.moduleName) {
                FieldInfo moduleField = context.ModuleContext.RegisterImportedModule(module);

                context.IL.Emit(OpCodes.Ldfld, moduleField);
            } else if (context.ModuleField != null) {
                context.IL.Emit(OpCodes.Ldfld, context.ModuleField);
            }
            context.IL.EmitCall(OpCodes.Call, method, 1);
        }
    }
}
