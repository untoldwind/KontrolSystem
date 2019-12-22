using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class CellType : RealizedType {
        public readonly TO2Type elementType;

        private Type generatedType;
        private Dictionary<string, IMethodInvokeFactory> allowedMethods;
        private Dictionary<string, IFieldAccessFactory> allowedFields;

        public CellType(TO2Type _elementType) {
            elementType = _elementType;
            allowedMethods = new Dictionary<string, IMethodInvokeFactory> {
                {"set_value", new CellSetValueFactory(this) },
                {"update", new CellUpdateFactory(this) },
            };
            allowedFields = new Dictionary<string, IFieldAccessFactory> {
                {"value", new CellValueAccess(this) }
            };
        }

        public override string Name => $"Cell<{elementType}>";

        public override bool IsValid(ModuleContext context) => elementType.IsValid(context);

        public override RealizedType UnderlyingType(ModuleContext context) => new CellType(elementType.UnderlyingType(context));

        public override Type GeneratedType(ModuleContext context) => generatedType ?? (generatedType = DeriveType(context));

        public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => BuildinType.NO_OPERATORS;

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => BuildinType.NO_OPERATORS;

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

        public override IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) => null;

        public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) {
            Type generatedOther = otherType.GeneratedType(context);

            return GeneratedType(context).IsAssignableFrom(generatedOther);
        }

        public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) => DefaultAssignEmitter.Instance;

        private Type DeriveType(ModuleContext context) => typeof(Cell<>).MakeGenericType(elementType.GeneratedType(context));
    }

    internal class CellValueAccess : IFieldAccessFactory {
        private CellType cellType;

        internal CellValueAccess(CellType _cellType) => cellType = _cellType;

        public TO2Type DeclaredType => cellType.elementType;

        public string Description => "Current value of the cell";

        public IFieldAccessEmitter Create(ModuleContext context) {
            Type generateType = cellType.GeneratedType(context);

            return new BoundPropertyLikeFieldAccessEmitter(cellType.elementType.UnderlyingType(context), generateType, generateType.GetProperty("Value").GetMethod, new OpCode[0]);
        }
    }

    internal class CellSetValueFactory : IMethodInvokeFactory {
        private readonly CellType cellType;

        internal CellSetValueFactory(CellType _cellType) => cellType = _cellType;

        public TypeHint ReturnHint => null;

        public TypeHint ArgumentHint(int argumentIdx) => context => argumentIdx == 0 ? cellType.elementType.UnderlyingType(context.ModuleContext) : null;

        public string Description => "Set/update the value of the cell";

        public TO2Type DeclaredReturn => BuildinType.Unit;

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter> { new FunctionParameter("new_value", cellType.elementType) };

        public IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments) {
            if (arguments.Count != 1) return null;
            Type generatedType = cellType.GeneratedType(context);
            MethodInfo methodInfo = generatedType.GetProperty("Value").SetMethod;

            return new BoundMethodInvokeEmitter(BuildinType.Unit, new List<RealizedParameter> { new RealizedParameter("new_value", cellType.elementType.UnderlyingType(context)) }, false, generatedType, methodInfo);
        }
    }

    internal class CellUpdateFactory : IMethodInvokeFactory {
        private readonly CellType cellType;

        internal CellUpdateFactory(CellType _cellType) => cellType = _cellType;

        public TypeHint ReturnHint => null;

        public TypeHint ArgumentHint(int argumentIdx) => _ => argumentIdx == 0 ? new FunctionType(false, new List<TO2Type> { cellType.elementType }, cellType.elementType) : null;

        public string Description => "Atomically update the content of the cell";

        public TO2Type DeclaredReturn => cellType;

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter> { new FunctionParameter("updater", new FunctionType(false, new List<TO2Type> { cellType.elementType }, cellType.elementType)) };

        public IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments) {
            if (arguments.Count != 1) return null;
            FunctionType updater = arguments[0].UnderlyingType(context) as FunctionType;
            if (updater == null || !cellType.elementType.IsAssignableFrom(context, updater.returnType)) return null;

            Type generatedType = cellType.GeneratedType(context);
            MethodInfo methodInfo = generatedType.GetMethod("Update");

            return new BoundMethodInvokeEmitter(cellType, new List<RealizedParameter> { new RealizedParameter("updater", updater) }, false, generatedType, methodInfo);
        }
    }
}
