using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPVessel {
    public partial class KSPVesselModule {
        [KSClass("Experiment")]
        public class ModuleExperimentAdapter :  PartModuleAdapter{
            private readonly ModuleScienceExperiment experiment;

            public ModuleExperimentAdapter(VesselAdapter vesselAdapter, ModuleScienceExperiment experiment) : base(vesselAdapter, experiment) =>
                this.experiment = experiment;

            [KSField] public bool Deployed => experiment.Deployed;

            [KSField] public bool Inoperable => experiment.Inoperable;

            [KSField] public bool HasData => experiment.GetData().Any();

            [KSField] public bool Rerunnable => experiment.IsRerunnable();

            [KSMethod]
            public void DumpData() => Array.ForEach(experiment.GetData(), data => experiment.DumpData(data));

            [KSMethod]
            public Result<object, string> DeployExperiment() {
                if(HasData) return Result.Err<object, string>($"Experiment {experiment.moduleName} already has data");
                
                if(Inoperable) return Result.Err<object, string>($"Expreiment {experiment.moduleName} is inoperable");
                
                Deploy();
                
                return Result.Ok<object, string>(null);
            }
            
            [KSMethod]
            public void Deploy() {
                var gatherDataMethod = experiment.GetType()
                    .GetMethod("gatherData", BindingFlags.NonPublic | BindingFlags.Instance);

                experiment.DeployExperiment();
                object result = gatherDataMethod!.Invoke(experiment, new object[] {false});
                
                experiment.StartCoroutine(result as IEnumerator);
            }

            [KSMethod]
            public void TransmitData() {
                
            }
        }
    }
}
