using System.Collections.Generic;
using System.Linq;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    [KSModule("ksp::orbit")]
    public partial class KSPOrbitModule {
        IKSPContext context;

        public KSPOrbitModule(IContext _context, Dictionary<string, object> modules) => context = _context as IKSPContext;

        [KSFunction]
        public Option<IBody> FindBody(string name) {
            IBody body = context?.Bodies.FirstOrDefault(b => b.Name == name);
            return body != null ? new Option<IBody>(body) : new Option<IBody>();
        }
    }
}
