using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using System.Linq;
using System.Collections.Generic;

namespace KontrolSystem.TO2.AST {
    public class FieldGet : Expression {
        public readonly Expression target;
        public readonly string fieldName;

        public FieldGet(Expression target, string fieldName, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            this.target = target;
            this.fieldName = fieldName;
        }

        public override void SetVariableContainer(IVariableContainer container) =>
            target.SetVariableContainer(container);

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
                return BuildinType.Unit;
            }

            return fieldAccess.FieldType;
        }

        public override void Prepare(IBlockContext context) {
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type targetType = target.ResultType(context);
            IFieldAccessEmitter fieldAccess =
                targetType.FindField(context.ModuleContext, fieldName)?.Create(context.ModuleContext);
            ;

            if (fieldAccess == null) {
                context.AddError(new StructuralError(
                    StructuralError.ErrorType.NoSuchField,
                    $"Type '{targetType.Name}' does not have a field '{fieldName}'",
                    Start,
                    End
                ));
                return;
            }

            if (!dropResult) {
                if (fieldAccess.RequiresPtr) target.EmitPtr(context);
                else target.EmitCode(context, false);

                if (context.HasErrors) return;

                fieldAccess.EmitLoad(context);
            }
        }
    }
}
