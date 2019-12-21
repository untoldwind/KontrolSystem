using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class Call : Expression {
        public readonly string moduleName;
        public readonly string name;
        public List<Expression> arguments;
        private IVariableContainer variableContainer;

        public Call(List<string> namePath, List<Expression> _arguments, Position start, Position end) : base(start, end) {
            if (namePath.Count > 1) {
                moduleName = String.Join("::", namePath.Take(namePath.Count - 1));
                name = namePath.Last();
            } else {
                moduleName = null;
                name = namePath.Last();
            }
            arguments = _arguments;
            for (int j = 0; j < arguments.Count; j++) {
                int i = j; // Copy for lambda
                arguments[i].SetTypeHint(context => {
                    List<RealizedType> parameterTypes = null;
                    TO2Type variable = ReferencedVariable(context);
                    if (variable != null) parameterTypes = (variable as FunctionType)?.parameterTypes.Select(t => t.UnderlyingType(context.ModuleContext)).ToList();
                    else parameterTypes = ReferencedFunction(context.ModuleContext)?.Parameters.Select(p => p.type.UnderlyingType(context.ModuleContext)).ToList();

                    return parameterTypes != null && i < parameterTypes.Count ? parameterTypes[i] : null;
                });
            }
        }

        public override void SetVariableContainer(IVariableContainer container) {
            variableContainer = container;
            foreach (Expression argument in arguments) argument.SetVariableContainer(container);
        }

        public override void SetTypeHint(TypeHint typeHint) { }

        public override TO2Type ResultType(IBlockContext context) {
            TO2Type variable = ReferencedVariable(context);
            if (variable != null) return (variable as FunctionType)?.returnType ?? BuildinType.Unit;

            IKontrolFunction function = ReferencedFunction(context.ModuleContext);
            if (function == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.NoSuchFunction,
                                       $"Function '{FullName}' not found",
                                       Start,
                                       End
                                   ));
                return BuildinType.Unit;
            }

            return function.ReturnType;
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            TO2Type variable = ReferencedVariable(context);

            if (variable != null) {
                if (!(variable is FunctionType)) {
                    context.AddError(
                        new StructuralError(
                            StructuralError.ErrorType.InvalidType,
                            $"Type {variable} cannot be called as a function",
                            Start,
                            End
                        )
                    );
                    return;
                }
                EmitCodeDelegate((FunctionType)variable, context, dropResult);
            } else
                EmitCodeFunction(context, dropResult);
        }

        private void EmitCodeDelegate(FunctionType functionType, IBlockContext context, bool dropResult) {
            IBlockVariable blockVariable = context.FindVariable(name);

            if (blockVariable == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.NoSuchVariable,
                                       $"No local variable '{name}'",
                                       Start,
                                       End
                                   ));
                return;
            }
            if (functionType.isAsync && !context.IsAsync) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.NoSuchFunction,
                                       $"Cannot call async function of variable '{name}' from a sync context",
                                       Start,
                                       End
                                   ));
                return;
            }

            blockVariable.EmitLoad(context);

            if (functionType.parameterTypes.Count != arguments.Count) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.ArgumentMismatch,
                                       $"Call to variable '{name}' requires {functionType.parameterTypes.Count} arguments",
                                       Start,
                                       End
                                   ));
                return;
            }

            for (int i = 0; i < arguments.Count; i++) {
                arguments[i].EmitCode(context, false);
                if (!context.HasErrors) functionType.parameterTypes[i].AssignFrom(context.ModuleContext, arguments[i].ResultType(context)).EmitConvert(context);
            }

            if (context.HasErrors) return;

            for (int i = 0; i < arguments.Count; i++) {
                TO2Type argumentType = arguments[i].ResultType(context);
                if (!functionType.parameterTypes[i].IsAssignableFrom(context.ModuleContext, argumentType)) {
                    context.AddError(new StructuralError(
                                           StructuralError.ErrorType.ArgumentMismatch,
                                           $"Argument {i + 1} of variable '{name}' has to be a {functionType.parameterTypes[i]}, but {argumentType} was given",
                                           Start,
                                           End
                                       ));
                    return;
                }
            }
            MethodInfo invokeMethod = functionType.GeneratedType(context.ModuleContext).GetMethod("Invoke");

            context.IL.EmitCall(OpCodes.Callvirt, invokeMethod, arguments.Count + 1);
            if (functionType.isAsync) context.RegisterAsyncResume(functionType.returnType);
            if (dropResult && functionType.returnType != BuildinType.Unit) context.IL.Emit(OpCodes.Pop);
        }

        private void EmitCodeFunction(IBlockContext context, bool dropResult) {
            IKontrolFunction function = ReferencedFunction(context.ModuleContext);

            if (function == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.NoSuchFunction,
                                       $"Function '{FullName}' not found",
                                       Start,
                                       End
                                   ));
                return;
            }
            if (function.IsAsync && !context.IsAsync) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.NoSuchFunction,
                                       $"Cannot call async function '{FullName}' from a sync context",
                                       Start,
                                       End
                                   ));
                return;
            }

            if (!function.RuntimeMethod.IsStatic) {
                context.IL.Emit(OpCodes.Ldarg_0);

                if (function.Module.Name != context.ModuleContext.moduleName) {
                    FieldInfo moduleField = context.ModuleContext.RegisterImportedModule(function.Module);

                    context.IL.Emit(OpCodes.Ldfld, moduleField);
                }
            }

            if (function.Parameters.Count != arguments.Count) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.ArgumentMismatch,
                                       $"Function '{FullName}' requires {function.Parameters.Count} arguments",
                                       Start,
                                       End
                                   ));
                return;
            }

            for (int i = 0; i < arguments.Count; i++) {
                TO2Type argumentType = arguments[i].ResultType(context);
                if (!function.Parameters[i].type.IsAssignableFrom(context.ModuleContext, argumentType)) {
                    context.AddError(new StructuralError(
                                           StructuralError.ErrorType.ArgumentMismatch,
                                           $"Argument {function.Parameters[i].name} of '{FullName}' has to be a {function.Parameters[i].type}, but {argumentType} was given",
                                           Start,
                                           End
                                       ));
                    return;
                }
            }

            for (int i = 0; i < arguments.Count; i++) {
                arguments[i].EmitCode(context, false);
                if (!context.HasErrors) function.Parameters[i].type.AssignFrom(context.ModuleContext, arguments[i].ResultType(context)).EmitConvert(context);
            }

            if (context.HasErrors) return;

            context.IL.EmitCall(OpCodes.Call, function.RuntimeMethod, function.RuntimeMethod.IsStatic ? arguments.Count : arguments.Count + 1);
            if (function.IsAsync) context.RegisterAsyncResume(function.ReturnType);
            if (dropResult && function.ReturnType != BuildinType.Unit) context.IL.Emit(OpCodes.Pop);
        }

        private string FullName => moduleName != null ? $"{moduleName}::{name}" : name;

        private TO2Type ReferencedVariable(IBlockContext context) => moduleName != null ? null : variableContainer.FindVariable(context, name);

        private IKontrolFunction ReferencedFunction(ModuleContext context) => moduleName != null ? context.FindModule(moduleName)?.FindFunction(name) : context.mappedFunctions.Get(name);
    }
}
