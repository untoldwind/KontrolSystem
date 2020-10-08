using System;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class FieldAssign : Expression {
        private readonly Expression target;
        private readonly string fieldName;
        private readonly Operator op;
        private readonly Expression expression;

        public FieldAssign(Expression target, string fieldName, Operator op, Expression expression,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.target = target;
            this.fieldName = fieldName;
            this.op = op;
            this.expression = expression;
        }

        public override IVariableContainer VariableContainer {
            set => target.VariableContainer = value;
        }

        public override TO2Type ResultType(IBlockContext context) {
            TO2Type targetType = target.ResultType(context);
            IFieldAccessEmitter fieldAccess =
                targetType.FindField(context.ModuleContext, fieldName)?.Create(context.ModuleContext);
            if (fieldAccess == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchField,
                    $"Type '{targetType.Name}' does not have a field '{fieldName}'",
                    Start,
                    End
                ));
                return BuiltinType.Unit;
            }

            if (!fieldAccess.CanStore) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchField,
                    $"Type '{targetType.Name}' field '{fieldName}' is read-only",
                    Start,
                    End
                ));
                return BuiltinType.Unit;
            }

            return fieldAccess.FieldType;
        }

        public override void Prepare(IBlockContext context) {
            target.Prepare(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type targetType = target.ResultType(context);
            IFieldAccessEmitter fieldAccess =
                targetType.FindField(context.ModuleContext, fieldName)?.Create(context.ModuleContext);
            TO2Type valueType = expression.ResultType(context);

            if (fieldAccess == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchField,
                    $"Type '{targetType.Name}' does not have a field '{fieldName}'",
                    Start,
                    End
                ));
                return;
            }

            if (!fieldAccess.CanStore) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchField,
                    $"Type '{targetType.Name}' field '{fieldName}' is read-only",
                    Start,
                    End
                ));
                return;
            }

            if (target is IAssignContext assignContext) {
                if (fieldAccess.RequiresPtr && assignContext.IsConst(context)) {
                    context.AddError(new StructuralError(
                        StructuralError.ErrorType.NoSuchField,
                        $"Type '{targetType.Name}' field '{fieldName}' can not be set on a read-only variable",
                        Start,
                        End
                    ));
                    return;
                }
            } else {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.CoreGeneration,
                    $"Field assign '{targetType.Name}'.'{fieldName}' on invalid target expression",
                    Start,
                    End
                ));
                return;
            }

            if (!fieldAccess.FieldType.IsAssignableFrom(context.ModuleContext, valueType)) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Type '{targetType.Name}' field '{fieldName}' is of type {fieldAccess.FieldType} but is assigned to {valueType}",
                    Start,
                    End
                ));
                return;
            }

            if (fieldAccess.RequiresPtr) target.EmitPtr(context);
            else target.EmitCode(context, false);
            expression.EmitCode(context, false);
            fieldAccess.FieldType.AssignFrom(context.ModuleContext, valueType).EmitConvert(context);

            IBlockVariable tmpResult = null;

            if (!dropResult) {
                tmpResult = context.MakeTempVariable(fieldAccess.FieldType);

                context.IL.Emit(OpCodes.Dup);
                tmpResult.EmitStore(context);
            }

            fieldAccess.EmitStore(context);

            if (!dropResult) {
                tmpResult.EmitLoad(context);
            }
        }
    }
}
