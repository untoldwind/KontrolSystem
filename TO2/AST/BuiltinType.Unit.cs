using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public abstract partial class BuiltinType : RealizedType {
        private class TO2Unit : BuiltinType {
            public override string Name => "Unit";
            public override Type GeneratedType(ModuleContext context) => typeof(object);
            public override bool IsAssignableFrom(ModuleContext context, TO2Type otherType) => otherType == this;
        }
    }
}
