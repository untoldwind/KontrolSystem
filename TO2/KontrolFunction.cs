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

        public RealizedParameter(string name, RealizedType type, IDefaultValue defaultValue = null) {
            this.name = name;
            this.type = type;
            this.defaultValue = defaultValue;
        }

        public RealizedParameter(IBlockContext context, FunctionParameter parameter) {
            name = parameter.name;
            type = parameter.type.UnderlyingType(context.ModuleContext);
            defaultValue = DefaultValue.ForParameter(context, parameter);
        }

        public bool HasDefault => defaultValue != null;

        public RealizedParameter FillGenerics(ModuleContext context, Dictionary<string, RealizedType> typeArguments) =>
            new RealizedParameter(name, type.FillGenerics(context, typeArguments), defaultValue);
    }

    public interface IKontrolFunction {
        IKontrolModule Module { get; }

        string Name { get; }

        string Description { get; }

        List<RealizedParameter> Parameters { get; }

        RealizedType ReturnType { get; }

        MethodInfo RuntimeMethod { get; }

        bool IsCompiled { get; }

        bool IsAsync { get; }

        object Invoke(IContext context, params object[] args);
    }

    public static class KontrolFunctionExtensions {
        public static RealizedType DelegateType(this IKontrolFunction function) {
            return new FunctionType(function.IsAsync, function.Parameters.Select(p => p.type as TO2Type).ToList(),
                function.ReturnType);
        }

        public static int RequiredParameterCount(this IKontrolFunction function) =>
            function.Parameters.Where(p => !p.HasDefault).Count();
    }

    public class CompiledKontrolFunction : IKontrolFunction {
        private CompiledKontrolModule module;
        private readonly string name;
        private readonly string description;
        private readonly List<RealizedParameter> parameters;
        private readonly RealizedType returnType;
        private readonly MethodInfo runtimeMethod;
        private readonly bool isAsync;

        public CompiledKontrolFunction(string name, string description, bool isAsync,
            List<RealizedParameter> parameters, RealizedType returnType, MethodInfo runtimeMethod) {
            this.name = name;
            this.description = description;
            this.isAsync = isAsync;
            this.parameters = parameters;
            this.returnType = returnType;
            this.runtimeMethod = runtimeMethod;
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
                ContextHolder.CurrentContext.Value = context;
                return RuntimeMethod.Invoke(null, args);
            } catch (TargetInvocationException e) {
                throw e.InnerException;
            } finally {
                ContextHolder.CurrentContext.Value = null;
            }
        }

        internal void SetModule(CompiledKontrolModule module) => this.module = module;
    }

    public class DeclaredKontrolFunction : IKontrolFunction {
        private readonly DeclaredKontrolModule module;
        private readonly List<RealizedParameter> parameters;
        private readonly RealizedType returnType;
        public readonly IBlockContext methodContext;
        public readonly FunctionDeclaration to2Function;

        public DeclaredKontrolFunction(DeclaredKontrolModule module, IBlockContext methodContext,
            FunctionDeclaration to2Function) {
            this.module = module;
            parameters = to2Function.parameters.Select(p => new RealizedParameter(methodContext, p)).ToList();
            returnType = to2Function.declaredReturn.UnderlyingType(methodContext.ModuleContext);
            this.methodContext = methodContext;
            this.to2Function = to2Function;
        }

        public IKontrolModule Module => module;

        public string Name => to2Function.name;

        public string Description => to2Function.description;

        public List<RealizedParameter> Parameters => parameters;

        public RealizedType ReturnType => returnType;

        public MethodInfo RuntimeMethod => methodContext.MethodBuilder;

        public bool IsCompiled => false;

        public bool IsAsync => to2Function.isAsync;

        public object Invoke(IContext context, params object[] args) =>
            throw new NotImplementedException($"Function {to2Function.name} not yet compiled");
    }
}
