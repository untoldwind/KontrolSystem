using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    [KSModule("ksp::orbit")]
    public partial class KSPOrbitModule {
        IKSPContext context;

        public KSPOrbitModule(IContext _context, Dictionary<string, object> modules) {
            context = _context as IKSPContext;
            if (context == null) throw new ArgumentException($"{_context} is not an IKSPContext");
        }


        [KSFunction]
        public Result<IBody, string> FindBody(string name) {
            IBody body = context?.Bodies.FirstOrDefault(b => b.Name == name);
            return body != null ? Result<IBody, string>.successful(body) : Result<IBody, string>.failure($"No such body '{name}'");
        }
    }
}
