using System.Collections.Generic;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.Testing {
    [KSModule("ksp::testing")]
    public class KSPTesting : CoreTesting {
        public KSPTesting(IContext context, Dictionary<string, object> modules) : base(context, modules) { }

        [KSFunction]
        public void assert_vec2(Vector2d expected, Vector2d actual, double delta) {
            context?.IncrAssertions();
            if ((expected - actual).magnitude > delta) throw new AssertException($"assert_vec2: {expected} != {actual}");
        }

        [KSFunction]
        public void assert_vec3(Vector3d expected, Vector3d actual, double delta) {
            context?.IncrAssertions();
            if ((expected - actual).magnitude > delta) throw new AssertException($"assert_vec3: {expected} != {actual}");
        }
    }
}
