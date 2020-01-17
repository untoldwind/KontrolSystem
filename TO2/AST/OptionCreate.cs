using System;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class OptionNoneCreate : Expression {
        public OptionNoneCreate(Position start = new Position(), Position end = new Position()) : base(start, end) { }

        private TypeHint typeHint;

        public override void SetVariableContainer(IVariableContainer container) { }

        public override void SetTypeHint(TypeHint _typeHint) {
            typeHint = _typeHint;
        }

        public override TO2Type ResultType(IBlockContext context) {
            OptionType optionHint = typeHint?.Invoke(context) as OptionType;

            return optionHint ?? BuildinType.Unit;
        }

        public override void Prepare(IBlockContext context) { }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            OptionType optionType = ResultType(context) as OptionType;

            if (optionType == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidType,
                                       $"Unable to infer type of option. Please add some type hint",
                                       Start,
                                       End
                                   ));
                return;
            }

            Type generatedType = optionType.GeneratedType(context.ModuleContext);

            IBlockVariable tempVariable = context.MakeTempVariable(optionType);
            EmitStore(context, tempVariable, dropResult);
        }

        public override void EmitStore(IBlockContext context, IBlockVariable variable, bool dropResult) {
            Type generatedType = variable.Type.GeneratedType(context.ModuleContext);

            variable.EmitLoadPtr(context);
            context.IL.Emit(OpCodes.Initobj, generatedType, 1, 0);
            if (!dropResult) variable.EmitLoad(context);
        }
    }
}
