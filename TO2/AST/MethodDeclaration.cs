using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class MethodDeclaration : Node, IVariableContainer {
        public readonly string name;
        private readonly string description;
        private readonly bool isAsync;
        private readonly bool isConst;
        private readonly List<FunctionParameter> parameters;
        private readonly TO2Type declaredReturn;
        private readonly Expression expression;
        private SyncBlockContext syncBlockContext;
        private StructTypeAliasDelegate structType;

        public MethodDeclaration(bool isAsync, string name, string description, bool isConst,
            List<FunctionParameter> parameters,
            TO2Type declaredReturn, Expression expression, Position start = new Position(),
            Position end = new Position()) : base(start, end) {
            this.name = name;
            this.description = description;
            this.isAsync = isAsync;
            this.isConst = isConst;
            this.parameters = parameters;
            this.declaredReturn = declaredReturn;
            this.expression = expression;
            this.expression.VariableContainer = this;
            this.expression.TypeHint = context => this.declaredReturn.UnderlyingType(context.ModuleContext);
        }

        public IVariableContainer ParentContainer => null;

        public TO2Type FindVariableLocal(IBlockContext context, string variableName) =>
            variableName == "self" ? structType : parameters.Find(p => p.name == variableName)?.type;

        public StructTypeAliasDelegate StructType {
            set => structType = value;
        }

        public IMethodInvokeFactory CreateInvokeFactory() {
            syncBlockContext ??= new SyncBlockContext(structType, isConst, name, declaredReturn, parameters);

            return new BoundMethodInvokeFactory(
                description,
                isConst,
                () => declaredReturn.UnderlyingType(structType.structContext),
                () => parameters.Select(p => new RealizedParameter(syncBlockContext, p)).ToList(),
                false,
                structType.structContext.typeBuilder,
                syncBlockContext.MethodBuilder
            );
        }

        public IEnumerable<StructuralError> EmitCode() {
            if (isAsync)
                return new StructuralError(StructuralError.ErrorType.CoreGeneration,
                    "Only sync methods are supported at the moment", Start, End).Yield();
            
            TO2Type valueType = expression.ResultType(syncBlockContext);
            if (declaredReturn != BuiltinType.Unit &&
                !declaredReturn.IsAssignableFrom(syncBlockContext.ModuleContext, valueType)) {
                syncBlockContext.AddError(new StructuralError(
                    StructuralError.ErrorType.IncompatibleTypes,
                    $"Function '{name}' returns {valueType} but should return {declaredReturn}",
                    Start,
                    End
                ));
                return syncBlockContext.AllErrors;
            }

            expression.EmitCode(syncBlockContext, declaredReturn == BuiltinType.Unit);
            if (!syncBlockContext.HasErrors && declaredReturn != BuiltinType.Unit)
                declaredReturn.AssignFrom(syncBlockContext.ModuleContext, expression.ResultType(syncBlockContext))
                    .EmitConvert(syncBlockContext);
            else if (declaredReturn == BuiltinType.Unit) {
                syncBlockContext.IL.Emit(OpCodes.Ldnull);
            }

            syncBlockContext.IL.EmitReturn(syncBlockContext.MethodBuilder.ReturnType);

            return syncBlockContext.AllErrors;
        }
    }
}
