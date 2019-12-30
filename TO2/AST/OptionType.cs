using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class OptionType : RealizedType {
        public readonly TO2Type elementType;

        private Type generatedType;
        private OperatorCollection allowedSuffixOperators;
        private Dictionary<string, IMethodInvokeFactory> allowedMethods;
        private Dictionary<string, IFieldAccessFactory> allowedFields;

        public OptionType(TO2Type _elementType) {
            elementType = _elementType;
            allowedSuffixOperators = new OperatorCollection {
                {Operator.Unwrap, new OptionUnwrapOperator(this) }
            };
            allowedMethods = new Dictionary<string, IMethodInvokeFactory> {
                {"map", new OptionMapFactory(this) },
                {"ok_or", new OptionOkOrFactory(this) }
            };
            if (elementType == BuildinType.Unit) {
                allowedFields = new Dictionary<string, IFieldAccessFactory> {
                    {"defined", new InlineFieldAccessFactory("`true` if the option is defined, i.e. contains a value", () => BuildinType.Bool) }
                };
            } else {
                allowedFields = new Dictionary<string, IFieldAccessFactory> {
                    {"defined", new OptionFieldAccess(this, OptionField.Defined) },
                    {"value", new OptionFieldAccess(this, OptionField.Value) }
                };
            }
        }

        public override string Name => $"Option<{elementType}>";

        public override bool IsValid(ModuleContext context) => elementType.IsValid(context);

        public override RealizedType UnderlyingType(ModuleContext context) => new OptionType(elementType.UnderlyingType(context));

        public override Type GeneratedType(ModuleContext context) => generatedType ?? (generatedType = DeriveType(context));

        public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => BuildinType.NO_OPERATORS;

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => allowedSuffixOperators;

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

        public override IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) => null;

        public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) {
            Type generatedOther = otherType.GeneratedType(context);

            return GeneratedType(context).IsAssignableFrom(generatedOther) || elementType.GeneratedType(context).IsAssignableFrom(generatedOther);
        }

        public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) {
            Type generatedOther = otherType.GeneratedType(context);

            return elementType != BuildinType.Unit && elementType.GeneratedType(context).IsAssignableFrom(generatedOther) ? new AssignSome(this, otherType) : DefaultAssignEmitter.Instance;
        }

        private Type DeriveType(ModuleContext context) => (elementType == BuildinType.Unit) ? typeof(bool) : typeof(Option<>).MakeGenericType(elementType.GeneratedType(context));
    }

    internal enum OptionField {
        Defined,
        Value,
    }

    internal class OptionFieldAccess : IFieldAccessFactory {
        private OptionType optionType;
        private OptionField field;

        internal OptionFieldAccess(OptionType _optionType, OptionField _field) {
            optionType = _optionType;
            field = _field;
        }

        public TO2Type DeclaredType {
            get {
                switch (field) {
                case OptionField.Defined: return BuildinType.Bool;
                case OptionField.Value: return optionType.elementType;
                default: throw new InvalidOperationException($"Unkown option field: {field}");
                }
            }
        }

        public string Description {
            get {
                switch (field) {
                case OptionField.Defined: return "`true` if the option is defined, i.e. contains a value";
                case OptionField.Value: return "Value of the option if definied";
                default: throw new InvalidOperationException($"Unkown option field: {field}");
                }
            }
        }

        public IFieldAccessEmitter Create(ModuleContext context) {
            Type generateType = optionType.GeneratedType(context);
            switch (field) {
            case OptionField.Defined: return new BoundFieldAccessEmitter(BuildinType.Bool, generateType, new List<FieldInfo> { generateType.GetField("defined") });
            case OptionField.Value: return new BoundFieldAccessEmitter(optionType.elementType.UnderlyingType(context), generateType, new List<FieldInfo> { generateType.GetField("value") });
            default: throw new InvalidOperationException($"Unkown option field: {field}");
            }
        }
    }

    internal class AssignSome : IAssignEmitter {
        private readonly OptionType optionType;
        private readonly TO2Type otherType;

        internal AssignSome(OptionType _optionType, TO2Type _otherType) {
            optionType = _optionType;
            otherType = _otherType;
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression, bool dropResult) {
            Type generatedType = optionType.GeneratedType(context.ModuleContext);
            IBlockVariable valueTemp = context.MakeTempVariable(optionType.elementType.UnderlyingType(context.ModuleContext));
            optionType.elementType.AssignFrom(context.ModuleContext, otherType).EmitAssign(context, valueTemp, expression, true);

            variable.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("defined"));
            valueTemp.EmitLoad(context);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("value"));
            if (!dropResult) variable.EmitLoad(context);
        }

        public void EmitConvert(IBlockContext context) {
            Type generatedType = optionType.GeneratedType(context.ModuleContext);
            ILocalRef value = context.IL.TempLocal(optionType.elementType.GeneratedType(context.ModuleContext));
            optionType.elementType.AssignFrom(context.ModuleContext, otherType).EmitConvert(context);
            value.EmitStore(context);
            ILocalRef someResult = context.IL.TempLocal(generatedType);
            someResult.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
            context.IL.Emit(OpCodes.Dup);
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("defined"));
            value.EmitLoad(context);
            context.IL.Emit(OpCodes.Stfld, generatedType.GetField("value"));
            someResult.EmitLoad(context);
        }
    }

    internal class OptionUnwrapOperator : IOperatorEmitter {
        private readonly OptionType optionType;

        internal OptionUnwrapOperator(OptionType _optionType) => optionType = _optionType;

        public bool Accepts(ModuleContext context, TO2Type _otherType) => _otherType == BuildinType.Unit;

        public TO2Type OtherType => BuildinType.Unit;

        public TO2Type ResultType => optionType.elementType;

        public bool RequirePtr => false;

        public void EmitCode(IBlockContext context, Node target) {
            OptionType expectedReturn = context.ExpectedReturn.UnderlyingType(context.ModuleContext) as OptionType;
            if (expectedReturn == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Operator ? is only allowed if function returns an option",
                    target.Start,
                    target.End
                ));
                return;
            }

            // Take success
            Type generatedType = optionType.GeneratedType(context.ModuleContext);
            if (optionType.elementType != BuildinType.Unit) {
                context.IL.Emit(OpCodes.Dup);
                context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("defined"));
            }

            LabelRef onSuccess = context.IL.DefineLabel(true);

            context.IL.Emit(OpCodes.Brtrue_S, onSuccess);
            // Keep track of stuff that is still on the stack at onSuccess
            int stackAdjust = context.IL.StackCount;
            if (optionType.elementType != BuildinType.Unit) {
                Type noneType = expectedReturn.GeneratedType(context.ModuleContext);
                ILocalRef noneResult = context.IL.TempLocal(noneType);
                // Clean stack entirely to make room for error result to return
                for (int i = context.IL.StackCount; i > 0; i--) context.IL.Emit(OpCodes.Pop);
                noneResult.EmitLoadPtr(context);
                context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
                noneResult.EmitLoad(context);
                if (context.IsAsync) {
                    context.IL.EmitNew(OpCodes.Newobj, context.MethodBuilder.ReturnType.GetConstructor(new Type[] { noneType }));
                }
            } else {
                context.IL.Emit(OpCodes.Ldc_I4_0);
                if (context.IsAsync) {
                    context.IL.EmitNew(OpCodes.Newobj, context.MethodBuilder.ReturnType.GetConstructor(new Type[] { typeof(bool) }));
                }
            }
            context.IL.EmitReturn(context.MethodBuilder.ReturnType);

            context.IL.MarkLabel(onSuccess);

            if (optionType.elementType != BuildinType.Unit) {
                // Readjust the stack counter
                context.IL.AdjustStack(stackAdjust);
                context.IL.Emit(OpCodes.Ldfld, generatedType.GetField("value"));
            }
        }

        public void EmitAssign(IBlockContext context, IBlockVariable variable, Node target) {
            EmitCode(context, target);
            variable.Type.AssignFrom(context.ModuleContext, ResultType).EmitConvert(context);
            variable.EmitStore(context);
        }
    }

    internal class OptionMapFactory : IMethodInvokeFactory {
        private readonly OptionType optionType;

        internal OptionMapFactory(OptionType _optionType) => optionType = _optionType;

        public TypeHint ReturnHint => null;

        public TypeHint ArgumentHint(int argumentIdx) => _ => argumentIdx == 0 ? new FunctionType(false, new List<TO2Type> { optionType.elementType }, BuildinType.Unit) : null;

        public string Description => "Map the content of the option";

        public TO2Type DeclaredReturn => new OptionType(BuildinType.Unit);

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter> { new FunctionParameter("mapper", new FunctionType(false, new List<TO2Type> { optionType.elementType }, BuildinType.Unit)) };

        public IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments) {
            if (arguments.Count != 1) return null;
            FunctionType mapper = arguments[0].UnderlyingType(context) as FunctionType;
            if (mapper == null || mapper.returnType == BuildinType.Unit) return null;

            Type generatedType = optionType.GeneratedType(context);
            MethodInfo methodInfo = generatedType.GetMethod("Map").MakeGenericMethod(mapper.returnType.GeneratedType(context));

            return new BoundMethodInvokeEmitter(new OptionType(mapper.returnType), new List<RealizedParameter> { new RealizedParameter("mapper", mapper) }, false, generatedType, methodInfo);
        }
    }

    internal class OptionOkOrFactory : IMethodInvokeFactory {
        private readonly OptionType optionType;

        internal OptionOkOrFactory(OptionType _optionType) => optionType = _optionType;

        public TypeHint ReturnHint => _ => new ResultType(optionType.elementType, BuildinType.Unit);

        public string Description => "Convert the option to a result, where None is mapped to the `if_none` error";

        public TypeHint ArgumentHint(int argumentIdx) => null;

        public TO2Type DeclaredReturn => new ResultType(optionType.elementType, BuildinType.Unit);

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter> { new FunctionParameter("on_error", BuildinType.Unit) };

        public IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments) {
            if (arguments.Count != 1) return null;
            RealizedType errorType = arguments[0].UnderlyingType(context);

            Type generatedType = optionType.GeneratedType(context);
            MethodInfo methodInfo = generatedType.GetMethod("OkOr").MakeGenericMethod(errorType.GeneratedType(context));

            return new BoundMethodInvokeEmitter(new ResultType(optionType.elementType, errorType), new List<RealizedParameter> { new RealizedParameter("if_none", errorType) }, false, generatedType, methodInfo);
        }
    }
}
