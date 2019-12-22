using System;
using System.Collections.Generic;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    [KSModule("ksp::control")]
    public partial class KSPControlModule {
        IKSPContext context;

        public KSPControlModule(IContext _context, Dictionary<string, object> modules) {
            context = _context as IKSPContext;
            if (context == null) throw new ArgumentException($"{_context} is not an IKSPContext");
        }
    }
}
