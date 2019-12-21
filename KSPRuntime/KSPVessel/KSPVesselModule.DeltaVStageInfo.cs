using System.Collections.Generic;
using System.Linq;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("VesselDeltaV")]
        public class DeltaVStageInfoAdapter {
            private readonly VesselAdapter vesselAdapter;
            private readonly DeltaVStageInfo deltaVStageInfo;

            internal DeltaVStageInfoAdapter(VesselAdapter _vesselAdapter, DeltaVStageInfo _deltaVStageInfo) {
                vesselAdapter = _vesselAdapter;
                deltaVStageInfo = _deltaVStageInfo;
            }

            [KSField]
            public long Stage => deltaVStageInfo.stage;

            [KSField]
            public double BurnTime => deltaVStageInfo.stageBurnTime;

            [KSField]
            public double DeltaVInVac => deltaVStageInfo.deltaVinVac;

            [KSField]
            public double DeltaVInASL => deltaVStageInfo.deltaVatASL;

            [KSField]
            public double StartMass => deltaVStageInfo.startMass;

            [KSField]
            public double EndMass => deltaVStageInfo.endMass;

            [KSField]
            public double FuelMass => deltaVStageInfo.fuelMass;

            [KSField]
            public double DryMass => deltaVStageInfo.dryMass;

            [KSField]
            public DeltaVEngineInfoAdapter[] Engines => deltaVStageInfo.enginesInStage.Select(e => new DeltaVEngineInfoAdapter(vesselAdapter, e)).ToArray();

            [KSField]
            public DeltaVEngineInfoAdapter[] ActiveEngines => deltaVStageInfo.enginesActiveInStage.Select(e => new DeltaVEngineInfoAdapter(vesselAdapter, e)).ToArray();

        }
    }
}
