using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("ModuleEngines")]
        public class ModuleEngineAdapter : PartModuleAdapter {
            private readonly ModuleEngines moduleEngines;

            public ModuleEngineAdapter(VesselAdapter vesselAdapter, ModuleEngines moduleEngines) : base(vesselAdapter,
                moduleEngines) => this.moduleEngines = moduleEngines;

            [KSMethod]
            public void Activate() => moduleEngines.Activate();

            [KSMethod]
            public void Shutdown() => moduleEngines.Shutdown();

            [KSField] public string Id => moduleEngines.engineID;

            [KSField] public string Name => moduleEngines.engineName;

            [KSField] public string Type => moduleEngines.engineType.ToString();

            [KSField] public bool IsShutdown => moduleEngines.engineShutdown;

            [KSField] public bool HasIgnited => moduleEngines.EngineIgnited;

            [KSField] public bool IsFlameout => moduleEngines.flameout;

            [KSField] public bool IsStaged => moduleEngines.staged;

            [KSField] public double MinThrust => moduleEngines.minThrust;

            [KSField] public double MaxThrust => moduleEngines.maxThrust;
        }
    }
}
