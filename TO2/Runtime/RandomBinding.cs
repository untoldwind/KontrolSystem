using System;
using System.Collections.Generic;
using KontrolSystem.TO2.AST;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Runtime {
    public static class RandomBinding {
        public static BoundType RandomType = Direct.BindType(DirectBindingMath.MODULE_NAME, "Random", "Random number generator", typeof(Random),
            BuildinType.NO_OPERATORS,
            BuildinType.NO_OPERATORS,
            new Dictionary<string, IMethodInvokeFactory> {
                {"next_float", new BoundMethodInvokeFactory("Get next random number between 0.0 and 1.0", () => BuildinType.Float, () => new List<FunctionParameter> { }, false, typeof(Random), typeof(Random).GetMethod("NextDouble") )},
                {"next_gaussian", new BoundMethodInvokeFactory("Get next gaussian distributed random number", () => BuildinType.Float, () => new List<FunctionParameter> { new FunctionParameter("mu", BuildinType.Float), new FunctionParameter("sigma", BuildinType.Float) }, false, typeof(RandomBinding), typeof(RandomBinding).GetMethod("NextGaussian") )},
            },
            BuildinType.NO_FIELDS);

        public static double NextGaussian(Random random, double mu, double sigma) {
            double u1 = random.NextDouble();
            double u2 = random.NextDouble();

            double rand_std_normal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                     Math.Sin(2.0 * Math.PI * u2);

            double rand_normal = mu + sigma * rand_std_normal;

            return rand_normal;
        }
    }
}
