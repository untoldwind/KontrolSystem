using System;
using KontrolSystem.KSP.Runtime.KSPControl;
using KontrolSystem.KSP.Runtime.KSPOrbit;
using KontrolSystem.KSP.Runtime.KSPMath;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;
using System.Linq;
using KSP.UI.Screens;
using UnityEngine;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Stage")]
        public class VesselStageAdapter {
            private readonly Vessel vessel;

            internal VesselStageAdapter(Vessel vessel) => this.vessel = vessel;

            [KSField] public long Number => StageManager.CurrentStage;

            [KSField] public bool Ready => vessel.isActiveVessel && StageManager.CanSeparate;

            [KSMethod]
            public Future<bool> Next() {
                if (!vessel.isActiveVessel || !StageManager.CanSeparate) return new Future.Success<bool>(false);

                StageManager.ActivateNextStage();
                return new Future.Success<bool>(true);
            }
        }
    }
}
