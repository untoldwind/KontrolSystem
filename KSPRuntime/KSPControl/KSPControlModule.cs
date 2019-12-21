using System.Collections.Generic;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPControl {
    [KSModule("ksp::control")]
    public partial class KSPControlModule {
        IKSPContext context;

        public KSPControlModule(IContext _context, Dictionary<string, object> modules) => context = _context as IKSPContext;
    }
}
