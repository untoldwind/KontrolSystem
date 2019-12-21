using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using KontrolSystem.TO2.Generator;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2.AST {
    internal struct LambaClass {
        internal readonly TypeInfo type;
        internal readonly List<IKontrolModule> importedModules;
        internal readonly List<(IBlockVariable source, ClonedFieldVariable target)> clonedVariables;
        internal readonly ConstructorInfo constructor;
        internal readonly MethodInfo lambdaImpl;

        internal LambaClass(TypeInfo _type,
                            List<IKontrolModule> _importedModules,
                            List<(IBlockVariable source, ClonedFieldVariable target)> _clonedVariables,
                            ConstructorInfo _constructor, MethodInfo _lambdaImpl) {
            type = _type;
            importedModules = _importedModules;
            clonedVariables = _clonedVariables;
            constructor = _constructor;
            lambdaImpl = _lambdaImpl;
        }
    }

    public class Lambda : Expression, IVariableContainer {
        public readonly List<FunctionParameter> parameters;
        public readonly Expression expression;

        private IVariableContainer parentContainer;

        private TypeHint typeHint;
        private LambaClass? lambaClass;

        private FunctionType resolvedType;

        public Lambda(List<FunctionParameter> _parameters, Expression _expression, Position start = new Position(), Position end = new Position()) : base(start, end) {
            parameters = _parameters;
            expression = _expression;
        }

        public IVariableContainer ParentContainer => parentContainer;

        public TO2Type FindVariableLocal(IBlockContext context, string name) {
            int idx = parameters.FindIndex(p => p.name == name);

            if (idx < 0 || idx >= parameters.Count) return null;

            TO2Type parameterType = parameters[idx].type;
            if (parameterType != null) return parameterType;
            FunctionType resultType = ResultType(context) as FunctionType;
            if (resolvedType == null || idx >= resolvedType.parameterTypes.Count) return null;

            return resolvedType.parameterTypes[idx];
        }

        public override void SetVariableContainer(IVariableContainer container) {
            parentContainer = container;
            expression.SetVariableContainer(this);
        }

        public override void SetTypeHint(TypeHint _typeHint) => typeHint = _typeHint;

        public override TO2Type ResultType(IBlockContext context) {
            if (resolvedType != null) return resolvedType;
            // Make an assumption ...
            if (parameters.All(p => p.type != null)) resolvedType = new FunctionType(false, parameters.Select(p => p.type).ToList(), BuildinType.Unit);
            else resolvedType = typeHint?.Invoke(context) as FunctionType;
            if (resolvedType != null) {
                // ... so that it is possible to determine the return type
                TO2Type returnType = expression.ResultType(context);
                // ... so that the assumption can be replaced with the (hopefully) real thing
                resolvedType = new FunctionType(false, resolvedType.parameterTypes, returnType);
            }
            return resolvedType ?? BuildinType.Unit;
        }

        public override void EmitCode(IBlockContext context, bool dropResult) {
            FunctionType lambdaType = ResultType(context) as FunctionType;

            if (lambdaType == null) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidType,
                                       $"Unable to infer type of lamba. Please add some type hint",
                                       Start,
                                       End
                                   ));
                return;
            }
            if (lambdaType.parameterTypes.Count != parameters.Count)
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidType,
                                       $"Expected lambda to have {lambdaType.parameterTypes.Count} parameters, found {parameters.Count}",
                                       Start,
                                       End
                                   ));
            for (int i = 0; i < parameters.Count; i++) {
                if (parameters[i].type == null) continue;
                if (!lambdaType.parameterTypes[i].IsAssignableFrom(context.ModuleContext, parameters[i].type))
                    context.AddError(new StructuralError(
                                        StructuralError.ErrorType.InvalidType,
                                        $"Expected parameter {parameters[i].name} of lambda to have type {lambdaType.parameterTypes[i]}, found {parameters[i].type}",
                                        Start,
                                        End
                                    ));
            }

            if (context.HasErrors) return;

            if (dropResult) return;

            if (!lambaClass.HasValue) lambaClass = CreateLambaClass(context, lambdaType);

            context.IL.Emit(OpCodes.Ldarg_0);
            context.IL.Emit(OpCodes.Ldfld, context.ModuleContext.contextField);

            foreach (IKontrolModule importedModule in lambaClass.Value.importedModules) {
                context.IL.Emit(OpCodes.Ldarg_0);
                context.IL.Emit(OpCodes.Ldfld, context.ModuleContext.RegisterImportedModule(importedModule));
            }
            foreach ((IBlockVariable source, _) in lambaClass.Value.clonedVariables) {
                source.EmitLoad(context);
            }
            context.IL.EmitNew(OpCodes.Newobj, lambaClass.Value.constructor, lambaClass.Value.importedModules.Count + lambaClass.Value.clonedVariables.Count + 1);
            context.IL.EmitPtr(OpCodes.Ldftn, lambaClass.Value.lambdaImpl);
            context.IL.EmitNew(OpCodes.Newobj, lambdaType.GeneratedType(context.ModuleContext).GetConstructor(new Type[] { typeof(object), typeof(IntPtr) }));
        }

        private LambaClass CreateLambaClass(IBlockContext parent, FunctionType lambdaType) {
            ModuleContext lambdaModuleContext = parent.ModuleContext.DefineSubComtext($"Lambda{Start.position}", typeof(object));
            SyncBlockContext lambdaContext = new SyncBlockContext(lambdaModuleContext, FunctionModifier.Public, false, "LambdaImpl", lambdaType.returnType, FixedParameters(lambdaType));
            SortedDictionary<string, (IBlockVariable source, ClonedFieldVariable target)> clonedVariables = new SortedDictionary<string, (IBlockVariable source, ClonedFieldVariable target)>();

            lambdaContext.SetExternVariables(name => {
                if (clonedVariables.ContainsKey(name)) return clonedVariables[name].target;
                IBlockVariable externalVariable = parent.FindVariable(name);
                if (externalVariable == null) return null;
                FieldBuilder field = lambdaModuleContext.typeBuilder.DefineField(name, externalVariable.Type.GeneratedType(parent.ModuleContext), FieldAttributes.InitOnly | FieldAttributes.Private);
                ClonedFieldVariable clonedVariable = new ClonedFieldVariable(externalVariable.Type, field);
                clonedVariables.Add(name, (externalVariable, clonedVariable));
                return clonedVariable;
            });

            expression.EmitCode(lambdaContext, lambdaType.returnType == BuildinType.Unit);
            lambdaContext.IL.EmitReturn(lambdaContext.MethodBuilder.ReturnType);

            foreach (StructuralError error in lambdaContext.AllErrors) parent.AddError(error);

            IEnumerable<FieldInfo> lambdaFields = lambdaModuleContext.contextField.Yield().Concat(lambdaModuleContext.ImportedModules.Select(m => m.moduleField)).Concat(clonedVariables.Values.Select(c => c.target.valueField));
            ConstructorBuilder constructorBuilder = lambdaModuleContext.typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                    lambdaFields.Select(f => f.FieldType).ToArray());
            IILEmitter constructorEmitter = new GeneratorILEmitter(constructorBuilder.GetILGenerator());

            int argIndex = 1;
            foreach (FieldInfo field in lambdaFields) {
                constructorEmitter.Emit(OpCodes.Ldarg_0);
                MethodParameter.EmitLoadArg(constructorEmitter, argIndex++);
                constructorEmitter.Emit(OpCodes.Stfld, field);
            }
            constructorEmitter.EmitReturn(typeof(void));

            lambdaType.GeneratedType(parent.ModuleContext);

            return new LambaClass(lambdaModuleContext.typeBuilder, lambdaModuleContext.ImportedModules.Select(m => m.module).ToList(), clonedVariables.Values.ToList(), constructorBuilder, lambdaContext.MethodBuilder);
        }

        private List<FunctionParameter> FixedParameters(FunctionType lambdaType) =>
            Enumerable.Zip(parameters, lambdaType.parameterTypes, (p, f) => new FunctionParameter(p.name, p.type ?? f)).ToList();
    }
}
