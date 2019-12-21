using KontrolSystem.TO2;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime {
    public class KontrolSystemKSPRegistry {
        public static KontrolRegistry CreateKSP() {
            KontrolRegistry registry = KontrolRegistry.CreateCore();

            registry.RegisterModule(KSPMathModule.Instance.module);

            KontrolSystem.KSP.Runtime.KSPVessel.KSPVesselModule.DirectBindings();
            registry.RegisterModule(BindingGenerator.BindModule(typeof(KontrolSystem.KSP.Runtime.KSPConsole.KSPConsoleModule)));
            registry.RegisterModule(BindingGenerator.BindModule(typeof(KontrolSystem.KSP.Runtime.KSPGame.KSPGameModule)));
            registry.RegisterModule(BindingGenerator.BindModule(typeof(KontrolSystem.KSP.Runtime.KSPGame.KSPGameWarpModule)));
            registry.RegisterModule(BindingGenerator.BindModule(typeof(KontrolSystem.KSP.Runtime.KSPOrbit.KSPOrbitModule)));
            registry.RegisterModule(BindingGenerator.BindModule(typeof(KontrolSystem.KSP.Runtime.KSPControl.KSPControlModule)));
            registry.RegisterModule(BindingGenerator.BindModule(typeof(KontrolSystem.KSP.Runtime.KSPVessel.KSPVesselModule)));
            registry.RegisterModule(BindingGenerator.BindModule(typeof(KontrolSystem.KSP.Runtime.KSPDebug.KSPDebugModule)));
            registry.RegisterModule(BindingGenerator.BindModule(typeof(KontrolSystem.KSP.Runtime.Testing.KSPTesting)));

            return registry;
        }
    }
}
