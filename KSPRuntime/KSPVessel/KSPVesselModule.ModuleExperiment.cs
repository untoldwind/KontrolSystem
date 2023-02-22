using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Experiment")]
        public class ModuleExperimentAdapter : PartModuleAdapter {
            private readonly ModuleScienceExperiment experiment;

            public ModuleExperimentAdapter(VesselAdapter vesselAdapter, ModuleScienceExperiment experiment) : base(
                vesselAdapter, experiment) {
                this.experiment = experiment;
            }

            [KSField] public bool Deployed => experiment.Deployed;

            [KSField] public bool Inoperable => experiment.Inoperable;

            [KSField] public bool HasData => experiment.GetData().Any();

            [KSField] public bool Rerunnable => experiment.IsRerunnable();

            [KSMethod]
            public void DumpData() => Array.ForEach(experiment.GetData(), experiment.DumpData);

            [KSMethod]
            public Result<object, string> DeployExperiment() {
                if (HasData) return Result.Err<object, string>($"Experiment {experiment.moduleName} already has data");

                if (Inoperable) return Result.Err<object, string>($"Experiment {experiment.moduleName} is inoperable");

                Deploy();

                return Result.Ok<object, string>(null);
            }

            [KSMethod]
            public void Deploy() {
                var gatherDataMethod = experiment.GetType()
                    .GetMethod("gatherData", BindingFlags.NonPublic | BindingFlags.Instance);

                experiment.DeployExperiment();
                object result = gatherDataMethod!.Invoke(experiment, new object[] { false });

                experiment.StartCoroutine(result as IEnumerator);
            }

            [KSMethod]
            public Result<object, string> TransmitData() {
                ScienceData[] data = experiment.GetData();
                ScienceData scienceData;
                for (int i = 0; i < data.Length; ++i) {
                    scienceData = data[i];

                    ExperimentResultDialogPage page = new ExperimentResultDialogPage(
                        partModule.part, scienceData, scienceData.baseTransmitValue, scienceData.transmitBonus,
                        false, "", false,
                        new ScienceLabSearch(vesselAdapter.vessel, scienceData),
                        null, null, null, null);
                }

                IScienceDataTransmitter bestTransmitter = ScienceUtil.GetBestTransmitter(vesselAdapter.vessel);

                if (bestTransmitter != null) {
                    bestTransmitter.TransmitData(data.ToList());
                    Array.ForEach(experiment.GetData(), experiment.DumpData);
                    if (experiment.useCooldown)
                        experiment.cooldownToGo = experiment.cooldownTimer;

                    return Result.Ok<object, string>(null);
                }

                return Result.Err<object, string>("No transmitters available on this vessel or no data to transmit.");
            }
        }
    }
}
