using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public interface IMethodInvokeEmitter {
        RealizedType ResultType {
            get;
        }

        List<RealizedParameter> Parameters {
            get;
        }

        bool RequiresPtr {
            get;
        }

        bool IsAsync {
            get;
        }

        void EmitCode(IBlockContext context);
    }

    public static class MethodInvokeEmitterExtensions {
        public static int RequiredParameterCount(this IMethodInvokeEmitter method) => method.Parameters.Where(p => !p.HasDefault).Count();
    }

    public interface IMethodInvokeFactory {
        TypeHint ReturnHint { get; }

        TypeHint ArgumentHint(int argumentIdx);

        string Description { get; }
        TO2Type DeclaredReturn { get; }

        List<FunctionParameter> DeclaredParameters { get; }

        IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments);

        IMethodInvokeFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments);
    }

    public class InlineMethodInvokeFactory : IMethodInvokeFactory {
        private readonly string description;
        private readonly Func<RealizedType> resultType;
        private readonly OpCode[] opCodes;

        public InlineMethodInvokeFactory(string _description, Func<RealizedType> _returnType, params OpCode[] _opCodes) {
            description = _description;
            resultType = _returnType;
            opCodes = _opCodes;
        }

        public TypeHint ReturnHint => _ => resultType();

        public TypeHint ArgumentHint(int argumentIdx) => null;

        public string Description => description;
        public TO2Type DeclaredReturn => resultType();

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter>();

        public IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments) => new InlineMethodInvokeEmitter(resultType(), new List<RealizedParameter>(), opCodes);

        public IMethodInvokeFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) => this;
    }

    public class InlineMethodInvokeEmitter : IMethodInvokeEmitter {
        private readonly RealizedType resultType;
        private readonly List<RealizedParameter> parameters;
        private readonly OpCode[] opCodes;

        public InlineMethodInvokeEmitter(RealizedType _returnType, List<RealizedParameter> _parameters, params OpCode[] _opCodes) {
            resultType = _returnType;
            parameters = _parameters;
            opCodes = _opCodes;
        }

        public RealizedType ResultType => resultType;

        public List<RealizedParameter> Parameters => parameters;

        public bool IsAsync => false;

        public bool RequiresPtr => false;

        public void EmitCode(IBlockContext context) {
            foreach (OpCode opCode in opCodes) {
                context.IL.Emit(opCode);
            }
        }
    }

    public class BoundMethodInvokeFactory : IMethodInvokeFactory {
        private readonly string description;
        private readonly Func<RealizedType> resultType;
        private readonly Func<List<RealizedParameter>> parameters;
        private readonly bool isAsync;
        private readonly MethodInfo methodInfo;
        private readonly Type methodTarget;

        public BoundMethodInvokeFactory(string _description, Func<RealizedType> _resultType, Func<List<RealizedParameter>> _parameters, bool _isAsync, Type _methodTarget, MethodInfo _methodInfo) {
            description = _description;
            resultType = _resultType;
            parameters = _parameters;
            methodInfo = _methodInfo;
            methodTarget = _methodTarget;
            isAsync = _isAsync;
        }

        public TypeHint ReturnHint => _ => resultType();

        public TypeHint ArgumentHint(int argumentIdx) {
            List<RealizedParameter> p = parameters();

            return context => argumentIdx >= 0 && argumentIdx < p.Count ? p[argumentIdx].type : null;
        }

        public string Description => description;

        public TO2Type DeclaredReturn => resultType();

        public List<FunctionParameter> DeclaredParameters => parameters().Select(p => new FunctionParameter(p.name, p.type)).ToList();

        public IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments) => new BoundMethodInvokeEmitter(resultType(), parameters(), isAsync, methodTarget, methodInfo);

        public IMethodInvokeFactory FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) {
            if (methodTarget.IsGenericType) {
                Type[] arguments = methodTarget.GetGenericArguments().Select(t => {
                    if (!typeArguments.ContainsKey(t.Name)) throw new ArgumentException($"Generic parameter {t.Name} not found");
                    return typeArguments[t.Name].GeneratedType(context);
                }).ToArray();
                Type genericTarget = methodTarget.MakeGenericType(arguments);
                List<RealizedParameter> genericParams = parameters().Select(p => p.FillGenerics(context, typeArguments)).ToList();
                MethodInfo genericMethod = genericTarget.GetMethod(methodInfo.Name, genericParams.Select(p => p.type.GeneratedType(context)).ToArray());

                if (genericMethod == null) throw new ArgumentException($"Unable to relocate method {methodInfo.Name} on {methodTarget} for type arguments {typeArguments}");

                return new BoundMethodInvokeFactory(description, () => resultType().FillGenerics(context, typeArguments), () => genericParams, isAsync, genericTarget, genericMethod);
            }
            return this;
        }
    }

    public class BoundMethodInvokeEmitter : IMethodInvokeEmitter {
        private readonly RealizedType resultType;
        private readonly List<RealizedParameter> parameters;
        private readonly bool isAsync;
        private readonly MethodInfo methodInfo;
        private readonly Type methodTarget;

        public BoundMethodInvokeEmitter(RealizedType _resultType, List<RealizedParameter> _parameters, bool _isAsync, Type _methodTarget, MethodInfo _methodInfo) {
            resultType = _resultType;
            parameters = _parameters;
            methodInfo = _methodInfo;
            methodTarget = _methodTarget;
            isAsync = _isAsync;
        }

        public RealizedType ResultType => resultType;

        public List<RealizedParameter> Parameters => parameters;

        public bool IsAsync => isAsync;

        public bool RequiresPtr => methodTarget.IsValueType && (methodInfo.CallingConvention & CallingConventions.HasThis) != 0;

        public void EmitCode(IBlockContext context) {
            if (methodInfo.IsVirtual) context.IL.EmitCall(OpCodes.Callvirt, methodInfo, Parameters.Count + 1);
            else context.IL.EmitCall(OpCodes.Call, methodInfo, Parameters.Count + 1);
        }
    }
}
