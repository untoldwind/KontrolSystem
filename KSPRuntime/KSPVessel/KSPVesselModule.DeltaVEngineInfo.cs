using System.Collections.Generic;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("DeltaVEngineInfo")]
        public class DeltaVEngineInfoAdapter {
            private readonly VesselAdapter vesselAdapter;
            private readonly DeltaVEngineInfo deltaVEngineInfo;

            internal DeltaVEngineInfoAdapter(VesselAdapter _vesselAdapter, DeltaVEngineInfo _deltaVEngineInfo) {
                vesselAdapter = _vesselAdapter;
                deltaVEngineInfo = _deltaVEngineInfo;
            }

            [KSField]
            public double ISPActual => deltaVEngineInfo.ispActual;

            [KSField]
            public double ISPASL => deltaVEngineInfo.ispASL;

            [KSField]
            public double ISPVac => deltaVEngineInfo.ispVac;

            [KSField]
            public long StartBurnStage => deltaVEngineInfo.startBurnStage;

            [KSField]
            public ModuleEngineAdapter Engine => new ModuleEngineAdapter(vesselAdapter, deltaVEngineInfo.engine);
        }
    }
}
