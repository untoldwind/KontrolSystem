using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2 {
    public struct RealizedParameter {
        public readonly string name;
        public readonly RealizedType type;
        public readonly IDefaultValue defaultValue;

        public RealizedParameter(string _name, RealizedType _type, IDefaultValue _defaultValue = null) {
            name = _name;
            type = _type;
            defaultValue = _defaultValue;
        }

        public RealizedParameter(IBlockContext context, FunctionParameter parameter) {
            name = parameter.name;
            type = parameter.type.UnderlyingType(context.ModuleContext);
            defaultValue = DefaultValue.ForParameter(context, parameter);
        }

        public bool HasDefault => defaultValue != null;
    }

    public interface IKontrolFunction {
        IKontrolModule Module {
            get;
        }

        string Name {
            get;
        }

        string Description {
            get;
        }

        List<RealizedParameter> Parameters {
            get;
        }

        RealizedType ReturnType {
            get;
        }

        MethodInfo RuntimeMethod {
            get;
        }

        bool IsCompiled {
            get;
        }

        bool IsAsync { get; }

        object Invoke(IContext context, params object[] args);
    }

    public static class KontrolFunctionExtensions {
        public static RealizedType DelegateType(this IKontrolFunction function) {
            return new FunctionType(function.IsAsync, function.Parameters.Select(p => p.type as TO2Type).ToList(), function.ReturnType);
        }

        public static int RequiredParameterCount(this IKontrolFunction function) => function.Parameters.Where(p => !p.HasDefault).Count();
    }

    public class CompiledKontrolFunction : IKontrolFunction {
        private CompiledKontrolModule module;
        private readonly string name;
        private readonly string description;
        private readonly List<RealizedParameter> parameters;
        private readonly RealizedType returnType;
        private readonly MethodInfo runtimeMethod;
        private readonly bool isAsync;

        public CompiledKontrolFunction(string _name, string _description, bool _isAsync, List<RealizedParameter> _parameters, RealizedType _returnType, MethodInfo _runtimeMethod) {
            name = _name;
            description = _description;
            isAsync = _isAsync;
            parameters = _parameters;
            returnType = _returnType;
            runtimeMethod = _runtimeMethod;
        }

        public IKontrolModule Module => module;

        public string Name => name;

        public string Description => description;

        public List<RealizedParameter> Parameters => parameters;

        public RealizedType ReturnType => returnType;

        public MethodInfo RuntimeMethod => runtimeMethod;

        public bool IsCompiled => true;

        public bool IsAsync => isAsync;

        public object Invoke(IContext context, params object[] args) {
            try {
                if (RuntimeMethod.IsStatic) {
                    return RuntimeMethod.Invoke(null, args);
                } else {
                    object instance = Activator.CreateInstance(module.RuntimeType, new object[] { context });
                    return RuntimeMethod.Invoke(instance, args);
                }
            } catch (TargetInvocationException e) {
                throw e.InnerException;
            }
        }

        internal void SetModule(CompiledKontrolModule _module) => module = _module;
    }

    public class DeclaredKontrolFunction : IKontrolFunction {
        private readonly DeclaredKontrolModule module;
        private readonly List<RealizedParameter> parameters;
        private readonly RealizedType returnType;
        public readonly IBlockContext methodContext;
        public readonly FunctionDeclaration to2Function;

        public DeclaredKontrolFunction(DeclaredKontrolModule _module, IBlockContext _methodContext, FunctionDeclaration _to2Function) {
            module = _module;
            parameters = _to2Function.parameters.Select(p => new RealizedParameter(_methodContext, p)).ToList();
            returnType = _to2Function.declaredReturn.UnderlyingType(_methodContext.ModuleContext);
            methodContext = _methodContext;
            to2Function = _to2Function;
        }

        public IKontrolModule Module => module;

        public string Name => to2Function.name;

        public string Description => to2Function.description;

        public List<RealizedParameter> Parameters => parameters;

        public RealizedType ReturnType => returnType;

        public MethodInfo RuntimeMethod => methodContext.MethodBuilder;

        public bool IsCompiled => false;

        public bool IsAsync => to2Function.isAsync;

        public object Invoke(IContext context, params object[] args) => throw new NotImplementedException($"Function {to2Function.name} not yet compiled");
    }
}
