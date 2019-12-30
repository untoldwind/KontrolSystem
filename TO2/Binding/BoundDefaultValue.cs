using System;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.Binding {
    public class BoundDefaultValue : Expression {
        private readonly TO2Type resultType;
        private readonly object defaultValue;

        public BoundDefaultValue(TO2Type _resultType, object _defaultValue) : base(new Position(), new Position()) {
            resultType = _resultType;
            defaultValue = _defaultValue;
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (dropResult) return;
            if (defaultValue == null) {
                Type generatedType = resultType.GeneratedType(context.ModuleContext);
                if (generatedType.IsValueType) {
                    ILocalRef temp = context.IL.TempLocal(generatedType);
                    temp.EmitLoadPtr(context);
                    context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
                    temp.EmitLoad(context);
                } else {
                    context.IL.Emit(OpCodes.Ldnull);
                }
                return;
            }
            switch (defaultValue) {
            case bool b:
                context.IL.Emit(b ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                return;
            case double d:
                context.IL.Emit(OpCodes.Ldc_R8, d);
                return;
            case long l:
                context.IL.Emit(OpCodes.Ldc_I8, l);
                return;
            default:
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.CoreGeneration,
                    $"Do not know how to handle default value of type {defaultValue.GetType()}",
                    Start,
                    End
                ));
                return;
            }
        }

        public override void Prepare(IBlockContext context) { }

        public override TO2Type ResultType(IBlockContext context) => resultType;

        public override void SetTypeHint(TypeHint typeHint) { }

        public override void SetVariableContainer(IVariableContainer container) { }
    }
}
