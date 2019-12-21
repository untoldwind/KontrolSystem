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

    public interface IMethodInvokeFactory {
        TypeHint ReturnHint { get; }

        TypeHint ArgumentHint(int argumentIdx);

        string Description { get; }
        TO2Type DeclaredReturn { get; }

        List<FunctionParameter> DeclaredParameters { get; }

        IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments);
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
        private readonly Func<List<FunctionParameter>> parameters;
        private readonly bool isAsync;
        private readonly MethodInfo methodInfo;
        private readonly Type methodTarget;

        public BoundMethodInvokeFactory(string _description, Func<RealizedType> _resultType, Func<List<FunctionParameter>> _parameters, bool _isAsync, Type _methodTarget, MethodInfo _methodInfo) {
            description = _description;
            resultType = _resultType;
            parameters = _parameters;
            methodInfo = _methodInfo;
            methodTarget = _methodTarget;
            isAsync = _isAsync;
        }

        public TypeHint ReturnHint => _ => resultType();

        public TypeHint ArgumentHint(int argumentIdx) {
            List<FunctionParameter> p = parameters();

            return context => argumentIdx >= 0 && argumentIdx < p.Count ? p[argumentIdx].type.UnderlyingType(context.ModuleContext) : null;
        }

        public string Description => description;

        public TO2Type DeclaredReturn => resultType();

        public List<FunctionParameter> DeclaredParameters => parameters();

        public IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments) => new BoundMethodInvokeEmitter(resultType(), parameters().Select(p => new RealizedParameter(context, p)).ToList(), isAsync, methodTarget, methodInfo);
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
