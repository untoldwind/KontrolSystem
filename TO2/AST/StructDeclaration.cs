using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public readonly struct StructField {
        public readonly string name;
        public readonly TO2Type type;
        public readonly string description;
        public readonly Position start;
        public readonly Position end;

        public StructField(string name, TO2Type type, string description, Position start = new Position(),
            Position end = new Position()) {
            this.name = name;
            this.type = type;
            this.description = description;
            this.start = start;
            this.end = end;
        }
    }

    public class StructDeclaration : Node, IModuleItem {
        private readonly bool exported;
        private readonly string name;
        private readonly string description;
        private readonly List<StructField> fields;
        private StructTypeAliasDelegate typeDelegate;

        public StructDeclaration(bool exported, string name, string description, List<StructField> fields,
            Position start = new Position(), Position end = new Position()) : base(start, end) {
            this.exported = exported;
            this.name = name;
            this.description = description;
            this.fields = fields;
        }

        public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) {
            typeDelegate = new StructTypeAliasDelegate(context, name, description, fields);
            if (exported) context.exportedTypes.Add((name, typeDelegate));
            return Enumerable.Empty<StructuralError>();
        }

        public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) {
            if (context.mappedTypes.ContainsKey(name))
                return new StructuralError(
                    StructuralError.ErrorType.DuplicateTypeName,
                    $"Type with name {name} already defined",
                    Start,
                    End
                ).Yield();
            context.mappedTypes.Add(name, typeDelegate);
            return Enumerable.Empty<StructuralError>();
        }

        public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();
    }

    public class StructTypeAliasDelegate : TO2Type {
        private readonly ModuleContext declaredModule;
        public readonly ModuleContext structContext;
        private readonly List<StructField> fields;
        public override string Name { get; }
        public override string Description { get; }
        private RecordStructType realizedType = null;

        internal StructTypeAliasDelegate(ModuleContext declaredModule, string name, string description,
            List<StructField> fields) {
            this.declaredModule = declaredModule;
            Name = name;
            Description = description;
            this.fields = fields;
            structContext = declaredModule.DefineSubContext(name, typeof(object));
        }

        public override RealizedType UnderlyingType(ModuleContext context) => RealizeType();

        public override Type GeneratedType(ModuleContext context) => RealizeType().GeneratedType(context);

        public override IMethodInvokeFactory FindMethod(ModuleContext context, string methodName) =>
            RealizeType().FindMethod(context, methodName);

        public override IFieldAccessFactory FindField(ModuleContext context, string fieldName) =>
            RealizeType().FindField(context, fieldName);

        public void AddMethod(string name, IMethodInvokeFactory methodInvokeFactory) =>
            RealizeType().DeclaredMethods.Add(name, methodInvokeFactory);

        private RealizedType RealizeType() {
            if (realizedType != null) return realizedType;

            List<RecordStructField> recordFields = new List<RecordStructField>();

            foreach (var field in fields) {
                RealizedType fieldTO2Type = field.type.UnderlyingType(declaredModule);
                Type fieldType = fieldTO2Type.GeneratedType(declaredModule);

                FieldInfo fieldInfo =
                    structContext.typeBuilder.DefineField(field.name, fieldType, FieldAttributes.Public);

                recordFields.Add(new RecordStructField(field.name, field.description, fieldTO2Type, fieldInfo));
            }

            ConstructorBuilder constructorBuilder = structContext.typeBuilder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            IILEmitter constructorEmitter = new GeneratorILEmitter(constructorBuilder.GetILGenerator());
            constructorEmitter.EmitReturn(typeof(void));

            realizedType = new RecordStructType(declaredModule.moduleName, Name, Description, structContext.typeBuilder,
                recordFields,
                new OperatorCollection(),
                new OperatorCollection(),
                new Dictionary<string, IMethodInvokeFactory>(),
                new Dictionary<string, IFieldAccessFactory>(),
                constructorBuilder);

            return realizedType;
        }
    }
}
