using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class MethodCall : Expression {
        public readonly Expression target;
        public readonly string methodName;
        public readonly List<Expression> arguments;
        private ILocalRef preparedResult;

        public MethodCall(Expression _target, string _methodName, List<Expression> _arguments, Position start = new Position(), Position end = new Position()) : base(start, end) {
            target = _target;
            methodName = _methodName;
            arguments = _arguments;
            for (int j = 0; j < arguments.Count; j++) {
                int i = j; // Copy for lambda
                arguments[i].SetTypeHint(context => {
                    TO2Type targetType = target.ResultType(context);
                    IMethodInvokeFactory methodInvoker = targetType.FindMethod(context.ModuleContext, methodName);

                    return methodInvoker?.ArgumentHint(i)?.Invoke(context);
                });
            }
        }

        public override void SetVariableContainer(IVariableContainer container) {
            target.SetVariableContainer(container);
            foreach (Expression argument in arguments) argument.SetVariableContainer(container);
        }

        public override void SetTypeHint(TypeHint typeHint) { }

        public override TO2Type ResultType(IBlockContext context) {
            TO2Type targetType = target.ResultType(context);
            IMethodInvokeEmitter methodInvoker = targetType.FindMethod(context.ModuleContext, methodName)?.Create(context.ModuleContext, arguments.Select(arg => arg.ResultType(context)).ToList());
            if (methodInvoker == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.NoSuchMethod,
                                       $"Type '{targetType.Name}' does not have a method '{methodName}'",
                                       Start,
                                       End
                                   ));
                return BuildinType.Unit;
            }
            return methodInvoker.ResultType;
        }

        public override void Prepare(IBlockContext context) {
            if (preparedResult != null) return;

            TO2Type targetType = target.ResultType(context);
            IMethodInvokeEmitter methodInvoker = targetType.FindMethod(context.ModuleContext, methodName)?.Create(context.ModuleContext, arguments.Select(arg => arg.ResultType(context)).ToList());

            if (methodInvoker == null || !methodInvoker.IsAsync || !context.IsAsync) return;

            EmitCode(context, false);
            preparedResult = context.DeclareHiddenLocal(methodInvoker.ResultType.GeneratedType(context.ModuleContext));
            preparedResult.EmitStore(context);
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            if (preparedResult != null) {
                if (!dropResult) preparedResult.EmitLoad(context);
                preparedResult = null;
                return;
            }

            TO2Type targetType = target.ResultType(context);
            IMethodInvokeEmitter methodInvoker = targetType.FindMethod(context.ModuleContext, methodName)?.Create(context.ModuleContext, arguments.Select(arg => arg.ResultType(context)).ToList());

            if (methodInvoker == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.NoSuchMethod,
                                       $"Type '{targetType.Name}' does not have a method '{methodName}'",
                                       Start,
                                       End
                                   ));
                return;
            }
            if (methodInvoker.IsAsync && !context.IsAsync) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.NoSuchFunction,
                                       $"Cannot call async method of variable '{targetType.Name}.{methodName}' from a sync context",
                                       Start,
                                       End
                                   ));
                return;
            }
            if (methodInvoker.RequiredParameterCount() > arguments.Count) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.ArgumentMismatch,
                                       $"Method '{targetType.Name}.{methodName}' requires {methodInvoker.RequiredParameterCount()} arguments",
                                       Start,
                                       End
                                   ));
                return;
            }
            int i;
            for (i = 0; i < arguments.Count; i++) {
                TO2Type argumentType = arguments[i].ResultType(context);
                if (!methodInvoker.Parameters[i].type.IsAssignableFrom(context.ModuleContext, argumentType)) {
                    context.AddError(new StructuralError(
                                           StructuralError.ErrorType.ArgumentMismatch,
                                           $"Argument {methodInvoker.Parameters[i].name} of '{targetType.Name}.{methodName}' has to be a {methodInvoker.Parameters[i].type}, but got {argumentType}",
                                           Start,
                                           End
                                       ));
                    return;
                }
            }

            foreach (Expression argument in arguments) {
                argument.Prepare(context);
            }

            if (methodInvoker.RequiresPtr)
                target.EmitPtr(context);
            else
                target.EmitCode(context, false);
            for (i = 0; i < arguments.Count; i++) {
                arguments[i].EmitCode(context, false);
                if (!context.HasErrors) methodInvoker.Parameters[i].type.AssignFrom(context.ModuleContext, arguments[i].ResultType(context)).EmitConvert(context);
            }
            if (!context.HasErrors) {
                for (; i < methodInvoker.Parameters.Count; i++) {
                    methodInvoker.Parameters[i].defaultValue.EmitCode(context);
                }
            }

            if (context.HasErrors) return;

            methodInvoker.EmitCode(context);
            if (methodInvoker.IsAsync) context.RegisterAsyncResume(methodInvoker.ResultType);
            if (dropResult && methodInvoker.ResultType != BuildinType.Unit) context.IL.Emit(OpCodes.Pop);
        }
    }
}
