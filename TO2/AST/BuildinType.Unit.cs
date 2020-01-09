using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public abstract partial class BuildinType : RealizedType {
        internal class TO2Unit : BuildinType {
            public override string Name => "Unit";
            public override Type GeneratedType(ModuleContext context) => typeof(void);
        }
    }
}
