using System;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    [KSModule("ksp::orbit")]
    public partial class KSPOrbitModule {
        [KSFunction]
        public static Result<IBody, string> FindBody(string name) {
            IBody body = KSPContext.CurrentContext.Bodies.FirstOrDefault(b => b.Name == name);
            return body != null ? Result<IBody, string>.successful(body) : Result<IBody, string>.failure($"No such body '{name}'");
        }
    }
}
