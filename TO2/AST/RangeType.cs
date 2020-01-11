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
                {"map", new BoundMethodInvokeFactory("Map the elements of the range, i.e. convert it into an array.",
                                                     () => new ArrayType(new GenericParameter("T")),
                                                     () => new List<RealizedParameter> { new RealizedParameter("mapper", new FunctionType(false, new List<TO2Type> { BuildinType.Int }, new GenericParameter("T"))) },
                                                     false, typeof(Range), typeof(Range).GetMethod("Map"))}
            };
            allowedFields = new Dictionary<string, IFieldAccessFactory> {
                {"length", new BoundPropertyLikeFieldAccessFactory("The length of the range", () => BuildinType.Int, typeof(Range), typeof(Range).GetProperty("Length").GetMethod)}
            };
        }

        public override string Name => "Range";

        public override bool IsValid(ModuleContext context) => true;

        public override RealizedType UnderlyingType(ModuleContext context) => this;

        public override Type GeneratedType(ModuleContext context) => typeof(Range);

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

        public void EmitNext(IBlockContext context) => currentIndex.EmitLoad(context);
    }
}
