﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public enum FunctionModifier {
        Public,
        Private,
        Test,
    }

    public class FunctionParameter : Node {
        public readonly string name;
        public readonly TO2Type type;
        public readonly Expression defaultValue;

        public FunctionParameter(string name, TO2Type type, Expression defaultValue = null, Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.name = name;
            this.type = type;
            this.defaultValue = defaultValue;
        }

        public bool HasDefault => defaultValue != null;

        public override string ToString() => $"{name} : {type}";
    }

    internal struct AsyncClass {
        internal readonly TypeInfo type;
        internal readonly ConstructorInfo constructor;

        internal AsyncClass(TypeInfo type, ConstructorInfo constructor) {
            this.type = type;
            this.constructor = constructor;
        }
    }

    public class FunctionDeclaration : Node, IModuleItem, IVariableContainer {
        public readonly FunctionModifier modifier;
        public readonly string name;
        public readonly string description;
        public readonly List<FunctionParameter> parameters = new List<FunctionParameter>();
        public readonly TO2Type declaredReturn;
        public readonly Expression expression;
        public readonly bool isAsync;
        private AsyncClass? asyncClass;

        public FunctionDeclaration(FunctionModifier modifier, bool isAsync, string name, string description,
                                   List<FunctionParameter> parameters, TO2Type declaredReturn, Expression expression,
                                   Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.modifier = modifier;
            this.name = name;
            this.description = description;
            this.isAsync = isAsync;
            this.parameters = parameters;
            this.declaredReturn = declaredReturn;
            this.expression = expression;
            this.expression.SetVariableContainer(this);
            this.expression.SetTypeHint(context => this.declaredReturn.UnderlyingType(context.ModuleContext));
        }

        public IVariableContainer ParentContainer => null;

        public TO2Type FindVariableLocal(IBlockContext context, string name) => parameters.Find(p => p.name == name)?.type;

        public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) => Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) => Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) => Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) {
            List<StructuralError> errors =
                parameters.Select(p => p.type).Concat(new TO2Type[] { declaredReturn }).Where(type => !type.IsValid(context)).Select(
                    type => new StructuralError(
                        StructuralError.ErrorType.InvalidType,
                        $"Invalid type name '{type.Name}'",
                        Start,
                        End
                    )).ToList();

            return errors;
        }

        public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) => Enumerable.Empty<StructuralError>();

        public void EmitCode(IBlockContext context) {
            TO2Type valueType = expression.ResultType(context);
            if (declaredReturn != BuildinType.Unit && !declaredReturn.IsAssignableFrom(context.ModuleContext, valueType)) {
                context.AddError(new StructuralError(
                               StructuralError.ErrorType.IncompatibleTypes,
                               $"Function '{name}' returns {valueType} but should return {declaredReturn}",
                               Start,
                               End
                           ));
                return;
            }

            if (isAsync) EmitCodeAsync(context);
            else EmitCodeSync(context);
        }

        public void EmitCodeSync(IBlockContext context) {
            expression.EmitCode(context, declaredReturn == BuildinType.Unit);
            if (!context.HasErrors && declaredReturn != BuildinType.Unit)
                declaredReturn.AssignFrom(context.ModuleContext, expression.ResultType(context)).EmitConvert(context);
            else if (declaredReturn == BuildinType.Unit) {
                context.IL.Emit(OpCodes.Ldnull);
            }
            context.IL.EmitReturn(context.MethodBuilder.ReturnType);
        }

        public void EmitCodeAsync(IBlockContext context) {
            if (!asyncClass.HasValue) asyncClass = CreateAsyncClass(context);

            for (int idx = 0; idx < parameters.Count; idx++)
                MethodParameter.EmitLoadArg(context.IL, idx);
            context.IL.EmitNew(OpCodes.Newobj, asyncClass.Value.constructor, parameters.Count);
            context.IL.EmitReturn(asyncClass.Value.type);
        }

        private AsyncClass CreateAsyncClass(IBlockContext parent) {
            Type returnType = declaredReturn.GeneratedType(parent.ModuleContext);
            Type typeParameter = returnType == typeof(void) ? typeof(object) : returnType;

            ModuleContext asyncModuleContext = parent.ModuleContext.DefineSubComtext($"AsyncFunction_{name}", typeof(Future<>).MakeGenericType(typeParameter));

            List<ClonedFieldVariable> clonedParameters = new List<ClonedFieldVariable>();

            foreach (FunctionParameter parameter in parameters) {
                FieldBuilder field = asyncModuleContext.typeBuilder.DefineField(parameter.name, parameter.type.GeneratedType(parent.ModuleContext), FieldAttributes.Private);
                clonedParameters.Add(new ClonedFieldVariable(parameter.type.UnderlyingType(parent.ModuleContext), field));
            }

            // ------------- PollValue -------------
            AsyncBlockContext asyncContext = new AsyncBlockContext(asyncModuleContext, FunctionModifier.Public, "PollValue", declaredReturn, typeof(FutureResult<>).MakeGenericType(typeParameter), clonedParameters);

            LabelRef applyState = asyncContext.IL.DefineLabel(false);
            LabelRef initialState = asyncContext.IL.DefineLabel(false);

            asyncContext.IL.Emit(OpCodes.Br, applyState);
            asyncContext.IL.MarkLabel(initialState);

            expression.EmitCode(asyncContext, false);
            if (!asyncContext.HasErrors) declaredReturn.AssignFrom(asyncContext.ModuleContext, expression.ResultType(asyncContext)).EmitConvert(asyncContext);

            asyncContext.IL.EmitNew(OpCodes.Newobj, asyncContext.MethodBuilder.ReturnType.GetConstructor(new Type[] { typeParameter }));
            asyncContext.IL.EmitReturn(asyncContext.MethodBuilder.ReturnType);

            // Apply state
            asyncContext.IL.MarkLabel(applyState);
            asyncContext.IL.Emit(OpCodes.Ldarg_0);
            asyncContext.IL.Emit(OpCodes.Ldfld, asyncContext.stateField);
            asyncContext.IL.Emit(OpCodes.Switch, initialState.Yield().Concat(asyncContext.asyncResumes.Select(ar => ar.pollLabel)));
            asyncContext.IL.Emit(OpCodes.Ldarg_0);
            asyncContext.IL.Emit(OpCodes.Ldfld, asyncContext.stateField);
            asyncContext.IL.EmitNew(OpCodes.Newobj, typeof(InvalidAsyncStateException).GetConstructor(new Type[] { typeof(int) }), 1);
            asyncContext.IL.Emit(OpCodes.Throw);

            foreach (AsyncResume asyncResume in asyncContext.asyncResumes) asyncResume.EmitPoll(asyncContext);

            // Restore state
            asyncContext.IL.MarkLabel(asyncContext.resume);
            foreach (StateRef stateRef in asyncContext.stateRefs) stateRef.EmitRestore(asyncContext);
            asyncContext.IL.Emit(OpCodes.Ldarg_0);
            asyncContext.IL.Emit(OpCodes.Ldfld, asyncContext.stateField);
            asyncContext.IL.Emit(OpCodes.Switch, initialState.Yield().Concat(asyncContext.asyncResumes.Select(ar => ar.resumeLabel)));
            asyncContext.IL.Emit(OpCodes.Ldarg_0);
            asyncContext.IL.Emit(OpCodes.Ldfld, asyncContext.stateField);
            asyncContext.IL.EmitNew(OpCodes.Newobj, typeof(InvalidAsyncStateException).GetConstructor(new Type[] { typeof(int) }), 1);
            asyncContext.IL.Emit(OpCodes.Throw);

            // Store state
            asyncContext.IL.MarkLabel(asyncContext.storeState);
            foreach (StateRef stateRef in asyncContext.stateRefs) stateRef.EmitStore(asyncContext);

            asyncContext.IL.MarkLabel(asyncContext.notReady);
            ILocalRef notReady = asyncContext.IL.TempLocal(asyncContext.MethodBuilder.ReturnType);
            notReady.EmitLoadPtr(asyncContext);
            asyncContext.IL.Emit(OpCodes.Initobj, asyncContext.MethodBuilder.ReturnType, 1, 0);
            notReady.EmitLoad(asyncContext);
            asyncContext.IL.EmitReturn(asyncContext.MethodBuilder.ReturnType);

            foreach (StructuralError error in asyncContext.AllErrors) parent.AddError(error);

            // ------------- Constructor -------------
            IEnumerable<FieldInfo> parameterFields = clonedParameters.Select(c => c.valueField);
            ConstructorBuilder constructorBuilder = asyncModuleContext.typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                    parameterFields.Select(f => f.FieldType).ToArray());
            IILEmitter constructorEmitter = new GeneratorILEmitter(constructorBuilder.GetILGenerator());

            int argIndex = 1;
            foreach (FieldInfo field in parameterFields) {
                constructorEmitter.Emit(OpCodes.Ldarg_0);
                MethodParameter.EmitLoadArg(constructorEmitter, argIndex++);
                constructorEmitter.Emit(OpCodes.Stfld, field);
            }

            constructorEmitter.Emit(OpCodes.Ldarg_0);
            constructorEmitter.Emit(OpCodes.Ldc_I4_0);
            constructorEmitter.Emit(OpCodes.Stfld, asyncContext.stateField);

            constructorEmitter.EmitReturn(typeof(void));

            return new AsyncClass(asyncModuleContext.typeBuilder, constructorBuilder);
        }
    }
}
