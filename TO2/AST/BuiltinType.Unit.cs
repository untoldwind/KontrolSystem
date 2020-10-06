using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public abstract partial class BuiltinType {
        private class TO2Unit : BuiltinType {
            private readonly IAssignEmitter anyToUnitAssign;

            internal TO2Unit() {
                anyToUnitAssign = new AnyToUnitAssign();
            }

            public override string Name => "Unit";
            public override Type GeneratedType(ModuleContext context) => typeof(object);
            public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) => true;

            public override IAssignEmitter AssignFrom(ModuleContext context, TO2Type otherType) =>
                otherType != Unit ? anyToUnitAssign : DefaultAssignEmitter.Instance;
        }

        private class AnyToUnitAssign : IAssignEmitter {
            public void EmitAssign(IBlockContext context, IBlockVariable variable, Expression expression,
                bool dropResult) {
                expression.EmitCode(context, true);
                context.IL.Emit(OpCodes.Ldnull);
                if (!dropResult) context.IL.Emit(OpCodes.Dup);
                variable.EmitStore(context);
            }

            public void EmitConvert(IBlockContext context) {
                context.IL.Emit(OpCodes.Pop);
                context.IL.Emit(OpCodes.Ldnull);
            }
        }
    }
}
