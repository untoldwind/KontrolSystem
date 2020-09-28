using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class Call : Expression {
        private readonly string moduleName;
        private readonly string name;
        private readonly List<Expression> arguments;
        private IVariableContainer variableContainer;
        private ILocalRef preparedResult;
        private TypeHint typeHint;

        public Call(List<string> namePath, List<Expression> arguments, Position start, Position end) : base(start, end) {
            if (namePath.Count > 1) {
                moduleName = String.Join("::", namePath.Take(namePath.Count - 1));
                name = namePath.Last();
            } else {
                moduleName = null;
                name = namePath.Last();
            }
            this.arguments = arguments;
            for (int i = 0; i < this.arguments.Count; i++) {
                this.arguments[i].SetTypeHint(ArgumentTypeHint(i));
            }
        }

        public override void SetVariableContainer(IVariableContainer container) {
            variableContainer = container;
            foreach (Expression argument in arguments) argument.SetVariableContainer(container);
        }

        public override void SetTypeHint(TypeHint typeHint) => this.typeHint = typeHint;

        public override TO2Type ResultType(IBlockContext context) {
            IKontrolConstant constant = ReferencedConstant(context.ModuleContext);
            if (constant != null) return (constant.Type as FunctionType)?.returnType ?? BuildinType.Unit;

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

            (_, RealizedType genericResult, _) = Helpers.MakeGeneric(context,
                function.ReturnType, function.Parameters, function.RuntimeMethod,
                typeHint?.Invoke(context), arguments.Select(e => e.ResultType(context)), Enumerable.Empty<(string name, RealizedType type)>(),
                this);

            return genericResult;
        }

        public override void Prepare(IBlockContext context) {
            if (preparedResult != null) return;
            if (ReferencedVariable(context) != null) return;

            IKontrolFunction function = ReferencedFunction(context.ModuleContext);

            if (function == null || !function.IsAsync || !context.IsAsync) return;

            EmitCodeFunction(context, false);
            preparedResult = context.DeclareHiddenLocal(function.ReturnType.GeneratedType(context.ModuleContext));
            preparedResult.EmitStore(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (preparedResult != null) {
                if (!dropResult) preparedResult.EmitLoad(context);
                preparedResult = null;
                return;
            }

            IKontrolConstant constant = ReferencedConstant(context.ModuleContext);

            if (constant != null) {
                if (!(constant.Type is FunctionType)) {
                    context.AddError(
                        new StructuralError(
                            StructuralError.ErrorType.InvalidType,
                            $"Constant {constant.Name} with type {constant.Type} cannot be called as a function",
                            Start,
                            End
                        )
                    );
                    return;
                }

                context.IL.Emit(OpCodes.Ldsfld, constant.RuntimeField);

                EmitCodeDelegate((FunctionType)constant.Type, context, dropResult);
                return;
            }

            TO2Type variable = ReferencedVariable(context);

            if (variable != null) {
                if (!(variable is FunctionType)) {
                    context.AddError(
                        new StructuralError(
                            StructuralError.ErrorType.InvalidType,
                            $"Variable {name} with type {variable} cannot be called as a function",
                            Start,
                            End
                        )
                    );
                    return;
                }
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

                blockVariable.EmitLoad(context);

                EmitCodeDelegate((FunctionType)variable, context, dropResult);
            } else
                EmitCodeFunction(context, dropResult);
        }

        private void EmitCodeDelegate(FunctionType functionType, IBlockContext context, bool dropResult) {
            if (functionType.isAsync && !context.IsAsync) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.NoSuchFunction,
                                       $"Cannot call async function of variable '{name}' from a sync context",
                                       Start,
                                       End
                                   ));
                return;
            }

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
            if (dropResult && invokeMethod.ReturnType != typeof(void)) context.IL.Emit(OpCodes.Pop);
            if (!dropResult && invokeMethod.ReturnType == typeof(void)) context.IL.Emit(OpCodes.Ldnull);
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

            if (function.RequiredParameterCount() > arguments.Count) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.ArgumentMismatch,
                                       $"Function '{FullName}' requires at least {function.RequiredParameterCount()} arguments",
                                       Start,
                                       End
                                   ));
                return;
            }

            (MethodInfo genericMethod, RealizedType genericResult, List<RealizedParameter> genericParameters) = Helpers.MakeGeneric(context,
                function.ReturnType, function.Parameters, function.RuntimeMethod,
                typeHint?.Invoke(context), arguments.Select(e => e.ResultType(context)), Enumerable.Empty<(string name, RealizedType type)>(),
                this);

            int i;
            for (i = 0; i < arguments.Count; i++) {
                TO2Type argumentType = arguments[i].ResultType(context);
                if (!genericParameters[i].type.IsAssignableFrom(context.ModuleContext, argumentType)) {
                    context.AddError(new StructuralError(
                                           StructuralError.ErrorType.ArgumentMismatch,
                                           $"Argument {function.Parameters[i].name} of '{FullName}' has to be a {genericParameters[i].type}, but {argumentType} was given",
                                           Start,
                                           End
                                       ));
                    return;
                }
            }

            foreach (Expression argument in arguments) {
                argument.Prepare(context);
            }

            for (i = 0; i < arguments.Count; i++) {
                arguments[i].EmitCode(context, false);
                if (!context.HasErrors) function.Parameters[i].type.AssignFrom(context.ModuleContext, arguments[i].ResultType(context)).EmitConvert(context);
            }
            if (!context.HasErrors) {
                for (; i < function.Parameters.Count; i++) {
                    function.Parameters[i].defaultValue.EmitCode(context);
                }
            }


            if (context.HasErrors) return;

            context.IL.EmitCall(OpCodes.Call, genericMethod, function.Parameters.Count);
            if (function.IsAsync) context.RegisterAsyncResume(genericResult);
            if (dropResult && genericMethod.ReturnType != typeof(void)) context.IL.Emit(OpCodes.Pop);
            if (!dropResult && genericMethod.ReturnType == typeof(void)) context.IL.Emit(OpCodes.Ldnull);
        }

        private string FullName => moduleName != null ? $"{moduleName}::{name}" : name;

        private IKontrolConstant ReferencedConstant(ModuleContext context) => moduleName != null ? context.FindModule(moduleName)?.FindConstant(name) : context.mappedConstants.Get(name);

        private TO2Type ReferencedVariable(IBlockContext context) => moduleName != null ? null : variableContainer.FindVariable(context, name);

        private IKontrolFunction ReferencedFunction(ModuleContext context) => moduleName != null ? context.FindModule(moduleName)?.FindFunction(name) : BuildinFunctions.ByName.Get(name) ?? context.mappedFunctions.Get(name);

        private TypeHint ArgumentTypeHint(int argumentIdx) {
            return context => {
                RealizedType returnType = null;
                List<RealizedType> parameterTypes = null;
                TO2Type variable = ReferencedVariable(context);
                if (variable != null) {
                    FunctionType functionVariable = variable as FunctionType;
                    if (functionVariable == null) return null;

                    returnType = functionVariable.returnType.UnderlyingType(context.ModuleContext);
                    parameterTypes = functionVariable.parameterTypes.Select(t => t.UnderlyingType(context.ModuleContext)).ToList();
                } else {
                    IKontrolFunction function = ReferencedFunction(context.ModuleContext);
                    if (function == null) return null;

                    returnType = function.ReturnType;
                    parameterTypes = function.Parameters.Select(p => p.type.UnderlyingType(context.ModuleContext)).ToList();
                }
                RealizedType expectedReturn = typeHint?.Invoke(context);
                if (expectedReturn != null && returnType.GenericParameters.Length > 0) {
                    Dictionary<string, RealizedType> typeArguments = returnType.InferGenericArgument(context.ModuleContext, expectedReturn).ToDictionary(t => t.name, t => t.type);

                    return argumentIdx < parameterTypes.Count ? parameterTypes[argumentIdx].FillGenerics(context.ModuleContext, typeArguments) : null;
                }

                return argumentIdx < parameterTypes.Count ? parameterTypes[argumentIdx] : null;
            };
        }
    }
}
