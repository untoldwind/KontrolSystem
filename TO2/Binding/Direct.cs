using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.Binding {
    public static class Direct {
        public static BoundType BindType(string moduleName, string localName, string description, Type runtimeType,
                                         OperatorCollection allowedPrefixOperators,
                                         OperatorCollection allowedSuffixOperators,
                                         Dictionary<string, IMethodInvokeFactory> allowedMethods,
                                         Dictionary<string, IFieldAccessFactory> allowedFields) {
            return new BoundType(moduleName, localName, description, runtimeType, allowedPrefixOperators, allowedSuffixOperators, allowedMethods, allowedFields);
        }

        public static CompiledKontrolFunction BindFunction(Type type, string methodName, string description, params Type[] parameterTypes) {
            string name = methodName.ToLower();
            MethodInfo methodInfo = type.GetMethod(methodName, parameterTypes);
            List<RealizedParameter> parameters = methodInfo.GetParameters().Select(p =>
                new RealizedParameter(p.Name, BindingGenerator.MapNativeType(p.ParameterType), BoundDefaultValue.DefaultValueFor(p))).ToList();

            if (methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Future<>)) {
                RealizedType returnType = BindingGenerator.MapNativeType(methodInfo.ReturnType.GetGenericArguments()[0]);

                return new CompiledKontrolFunction(name, description, true, parameters, returnType, methodInfo);
            } else {
                RealizedType returnType = BindingGenerator.MapNativeType(methodInfo.ReturnType);

                return new CompiledKontrolFunction(name, description, false, parameters, returnType, methodInfo);
            }
        }

        public static CompiledKontrolConstant BindConstant(Type type, string fieldName, string description) {
            string name = fieldName.ToUpperInvariant();
            FieldInfo fieldInfo = type.GetField(fieldName);
            TO2Type TO2Type = BindingGenerator.MapNativeType(fieldInfo.FieldType);

            return new CompiledKontrolConstant(name, description, TO2Type, fieldInfo);
        }

        public static CompiledKontrolModule BindModule(string name, string description,
                Type runtimeType,
                List<RealizedType> types,
                List<CompiledKontrolConstant> constants,
                List<CompiledKontrolFunction> functions) {
            return new CompiledKontrolModule(name, description, runtimeType, types.Select(t => (t.LocalName, t)), constants, functions, new List<CompiledKontrolFunction>());
        }
    }
}
