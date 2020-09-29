using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.Binding {
    public static class BindingGenerator {
        private static Dictionary<Type, RealizedType> typeMappings = new Dictionary<Type, RealizedType> {
            {typeof(bool), BuildinType.Bool},
            {typeof(long), BuildinType.Int},
            {typeof(double), BuildinType.Float},
            {typeof(string), BuildinType.String},
            {typeof(void), BuildinType.Unit},
        };

        private static Dictionary<Type, CompiledKontrolModule> boundModules =
            new Dictionary<Type, CompiledKontrolModule>();

        public static CompiledKontrolModule BindModule(Type moduleType) {
            lock (boundModules) {
                if (boundModules.ContainsKey(moduleType)) return boundModules[moduleType];

                KSModule ksModule = moduleType.GetCustomAttribute<KSModule>();

                if (ksModule == null) throw new ArgumentException($"Type {moduleType} must have a kSClass attribute");

                Type runtimeType = moduleType;
                List<CompiledKontrolFunction> functions = new List<CompiledKontrolFunction>();
                List<BoundType> types = new List<BoundType>();
                List<CompiledKontrolConstant> constants = new List<CompiledKontrolConstant>();

                while (runtimeType != null && runtimeType != typeof(object)) {
                    foreach (Type nested in runtimeType.GetNestedTypes(BindingFlags.Public)) {
                        if (nested.GetCustomAttribute<KSClass>() != null) types.Add(BindType(ksModule.Name, nested));
                    }

                    foreach (BoundType type in types) LinkType(type);

                    foreach (FieldInfo field in runtimeType.GetFields(BindingFlags.Public | BindingFlags.Static)) {
                        KSConstant ksConstant = field.GetCustomAttribute<KSConstant>();
                        if (ksConstant == null) continue;

                        TO2Type to2Type = BindingGenerator.MapNativeType(field.FieldType);

                        constants.Add(new CompiledKontrolConstant(
                            ksConstant.Name ?? ToSnakeCase(field.Name).ToUpperInvariant(),
                            NormalizeDescription(ksConstant.Description), to2Type, field));
                    }

                    foreach (MethodInfo method in runtimeType.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
                        KSFunction ksFunction = method.GetCustomAttribute<KSFunction>();
                        if (ksFunction == null) continue;

                        List<RealizedParameter> parameters = method.GetParameters().Select(p =>
                            new RealizedParameter(p.Name, MapNativeType(p.ParameterType),
                                BoundDefaultValue.DefaultValueFor(p))).ToList();
                        if (method.ReturnType.IsGenericType &&
                            method.ReturnType.GetGenericTypeDefinition() == typeof(Future<>)) {
                            Type typeArg = method.ReturnType.GetGenericArguments()[0];
                            RealizedType resultType =
                                typeArg == typeof(object) ? BuildinType.Unit : MapNativeType(typeArg);
                            functions.Add(new CompiledKontrolFunction(ksFunction.Name ?? ToSnakeCase(method.Name),
                                NormalizeDescription(ksFunction.Description), true, parameters, resultType, method));
                        } else {
                            RealizedType resultType = MapNativeType(method.ReturnType);
                            functions.Add(new CompiledKontrolFunction(ksFunction.Name ?? ToSnakeCase(method.Name),
                                NormalizeDescription(ksFunction.Description), false, parameters, resultType, method));
                        }
                    }

                    runtimeType = runtimeType.BaseType;
                }

                CompiledKontrolModule module = new CompiledKontrolModule(ksModule.Name,
                    NormalizeDescription(ksModule.Description), types.Select(t => (t.localName, t as RealizedType)),
                    constants, functions, new List<CompiledKontrolFunction>());
                boundModules.Add(moduleType, module);
                return module;
            }
        }

        public static BoundType BindType(string modulePrefix, Type type) {
            KSClass ksClass = type.GetCustomAttribute<KSClass>();

            if (ksClass == null) throw new ArgumentException($"Type {type} must have a kSClass attribute");

            BoundType boundType = new BoundType(modulePrefix, ksClass.Name ?? type.Name,
                NormalizeDescription(ksClass.Description), type,
                BuildinType.NoOperators, BuildinType.NoOperators,
                Enumerable.Empty<(string name, IMethodInvokeFactory invoker)>(),
                Enumerable.Empty<(string name, IFieldAccessFactory access)>()
            );
            RegisterTypeMapping(type, boundType);
            return boundType;
        }

        public static void LinkType(BoundType boundType) {
            foreach (MethodInfo method in boundType.runtimeType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            ) {
                KSMethod ksMethod = method.GetCustomAttribute<KSMethod>();
                if (ksMethod == null) continue;
                boundType.allowedMethods.Add(ksMethod.Name ?? ToSnakeCase(method.Name),
                    BindMethod(NormalizeDescription(ksMethod.Description), boundType.runtimeType, method));
            }

            foreach (PropertyInfo property in boundType.runtimeType.GetProperties(BindingFlags.Public |
                BindingFlags.Instance)) {
                KSField ksField = property.GetCustomAttribute<KSField>();
                if (ksField == null) continue;

                if (property.CanRead)
                    boundType.allowedFields.Add(ksField.Name ?? ToSnakeCase(property.Name),
                        BindProperty(NormalizeDescription(ksField.Description), boundType.runtimeType, property));
                if (ksField?.IncludeSetter ?? false && property.CanWrite)
                    boundType.allowedMethods.Add("set_" + (ksField.Name ?? ToSnakeCase(property.Name)),
                        BindMethod(NormalizeDescription(ksField.Description), boundType.runtimeType,
                            property.GetSetMethod()));
            }
        }

        static IMethodInvokeFactory BindMethod(string description, Type type, MethodInfo method) {
            List<RealizedParameter> parameters = method.GetParameters().Select(p =>
                    new RealizedParameter(p.Name, MapNativeType(p.ParameterType), BoundDefaultValue.DefaultValueFor(p)))
                .ToList();
            if (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Future<>)) {
                Type typeArg = method.ReturnType.GetGenericArguments()[0];
                RealizedType resultType = typeArg == typeof(object) ? BuildinType.Unit : MapNativeType(typeArg);
                return new BoundMethodInvokeFactory(description, () => resultType, () => parameters, true, type,
                    method);
            } else {
                RealizedType resultType = MapNativeType(method.ReturnType);
                return new BoundMethodInvokeFactory(description, () => resultType, () => parameters, false, type,
                    method);
            }
        }

        static IFieldAccessFactory BindProperty(string description, Type type, PropertyInfo property) {
            RealizedType result = MapNativeType(property.PropertyType);

            return new BoundPropertyLikeFieldAccessFactory(description, () => result, type, property.GetGetMethod());
        }

        public static void RegisterTypeMapping(Type type, RealizedType to2Type) {
            if (typeMappings.ContainsKey(type))
                typeMappings[type] = to2Type;
            else
                typeMappings.Add(type, to2Type);
        }

        internal static RealizedType MapNativeType(Type type) {
            if (type.IsGenericParameter) return new GenericParameter(type.Name);
            if (type.IsGenericType) {
                Type baseType = type.GetGenericTypeDefinition();
                Type[] typeArgs = type.GetGenericArguments();

                if (baseType == typeof(Option<>)) {
                    TO2Type innerType = typeArgs[0] == typeof(object) ? BuildinType.Unit : MapNativeType(typeArgs[0]);

                    return new OptionType(innerType);
                }

                if (baseType == typeof(Result<,>)) {
                    TO2Type successType = typeArgs[0] == typeof(object) ? BuildinType.Unit : MapNativeType(typeArgs[0]);
                    TO2Type errorType = typeArgs[1] == typeof(object) ? BuildinType.Unit : MapNativeType(typeArgs[1]);

                    return new ResultType(successType, errorType);
                }

                if (baseType.FullName.StartsWith("System.Func")) {
                    List<TO2Type> parameterTypes = typeArgs.Take(typeArgs.Length - 1)
                        .Select(t => MapNativeType(t) as TO2Type).ToList();
                    TO2Type returnType = MapNativeType(typeArgs[typeArgs.Length - 1]);
                    return new FunctionType(false, parameterTypes, returnType);
                }

                if (baseType.FullName.StartsWith("System.Action")) {
                    List<TO2Type> parameterTypes = typeArgs.Select(t => MapNativeType(t) as TO2Type).ToList();
                    return new FunctionType(false, parameterTypes, BuildinType.Unit);
                }

                RealizedType to2BaseType = typeMappings.Get(baseType);
                if (to2BaseType != null) {
                    List<TO2Type> typeArguments = typeArgs.Select(MapNativeType).Cast<TO2Type>().ToList();
                    return new DirectTypeReference(to2BaseType, typeArguments);
                }
            } else if (type.IsArray) {
                TO2Type elementType = MapNativeType(type.GetElementType());

                return new ArrayType(elementType);
            }

            return typeMappings.Get(type) ?? throw new ArgumentException($"No mapping for {type}");
        }

        internal static string ToSnakeCase(string name) => String
            .Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();

        internal static string NormalizeDescription(string description) {
            if (description == null) return null;

            StringBuilder sb = new StringBuilder();

            foreach (string line in description.Split('\n')) {
                sb.Append(line.Trim());
                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}
