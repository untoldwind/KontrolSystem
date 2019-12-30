using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class RangeType : RealizedType {
        private Dictionary<string, IMethodInvokeFactory> allowedMethods;
        private Dictionary<string, IFieldAccessFactory> allowedFields;

        public RangeType() {
            allowedMethods = new Dictionary<string, IMethodInvokeFactory> {
                {"map", new RangeMapFactory()}
            };
            allowedFields = new Dictionary<string, IFieldAccessFactory> {
                {"length", new BoundPropertyLikeFieldAccessFactory("The length of the range", () => BuildinType.Int, typeof(Range), typeof(Range).GetProperty("Length").GetMethod)}
            };
        }

        public override string Name => "Range";

        public override bool IsValid(ModuleContext context) => true;

        public override RealizedType UnderlyingType(ModuleContext context) => this;

        public override Type GeneratedType(ModuleContext context) => typeof(Range);

        public override IOperatorCollection AllowedPrefixOperators(ModuleContext context) => BuildinType.NO_OPERATORS;

        public override IOperatorCollection AllowedSuffixOperators(ModuleContext context) => BuildinType.NO_OPERATORS;

        public override Dictionary<string, IMethodInvokeFactory> DeclaredMethods => allowedMethods;

        public override Dictionary<string, IFieldAccessFactory> DeclaredFields => allowedFields;

        public override IIndexAccessEmitter AllowedIndexAccess(ModuleContext context, IndexSpec indexSpec) => null;

        public override IForInSource ForInSource(ModuleContext context, TO2Type typeHint) => new RangeForInSource();
    }

    public class RangeForInSource : IForInSource {
        private ILocalRef currentIndex;
        private ILocalRef rangeRef;

        public RangeForInSource() { }

        public RealizedType ElementType => BuildinType.Int;

        public void EmitInitialize(IBlockContext context) {
            rangeRef = context.DeclareHiddenLocal(typeof(Range));
            rangeRef.EmitStore(context);
            currentIndex = context.DeclareHiddenLocal(typeof(long));
            rangeRef.EmitLoad(context);
            context.IL.Emit(OpCodes.Ldfld, typeof(Range).GetField("from"));
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Conv_I8);
            context.IL.Emit(OpCodes.Sub);
            currentIndex.EmitStore(context);
        }

        public void EmitCheckDone(IBlockContext context, LabelRef loop) {
            currentIndex.EmitLoad(context);
            context.IL.Emit(OpCodes.Ldc_I4_1);
            context.IL.Emit(OpCodes.Conv_I8);
            context.IL.Emit(OpCodes.Add);
            context.IL.Emit(OpCodes.Dup);
            currentIndex.EmitStore(context);
            rangeRef.EmitLoad(context);
            context.IL.Emit(OpCodes.Ldfld, typeof(Range).GetField("to"));
            context.IL.Emit(loop.isShort ? OpCodes.Blt_S : OpCodes.Blt, loop);
        }

        public void EmitNext(IBlockContext context) {
            currentIndex.EmitLoad(context);
        }

        public void EmitFinalize(IBlockContext context) { }
    }

    internal class RangeMapFactory : IMethodInvokeFactory {
        public TypeHint ReturnHint => null;

        public TypeHint ArgumentHint(int argumentIdx) => _ => argumentIdx == 0 ? new FunctionType(false, new List<TO2Type> { BuildinType.Int }, BuildinType.Unit) : null;

        public string Description => "Map the elements of the range, i.e. convert it into an array.";

        public TO2Type DeclaredReturn => new OptionType(BuildinType.Unit);

        public List<FunctionParameter> DeclaredParameters => new List<FunctionParameter> { new FunctionParameter("mapper", new FunctionType(false, new List<TO2Type> { BuildinType.Int }, BuildinType.Unit)) };

        public IMethodInvokeEmitter Create(ModuleContext context, List<TO2Type> arguments) {
            if (arguments.Count != 1) return null;
            FunctionType mapper = arguments[0].UnderlyingType(context) as FunctionType;
            if (mapper == null || mapper.returnType == BuildinType.Unit) return null;

            MethodInfo methodInfo = typeof(Range).GetMethod("Map").MakeGenericMethod(mapper.returnType.GeneratedType(context));

            return new BoundMethodInvokeEmitter(new ArrayType(mapper.returnType), new List<RealizedParameter> { new RealizedParameter("mapper", mapper, null) }, false, typeof(Range), methodInfo);
        }
    }
}
