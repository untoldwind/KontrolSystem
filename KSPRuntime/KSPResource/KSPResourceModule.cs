using System;
using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.KSP.Runtime.KSPResource {
    [KSModule("ksp::resource", Description =
        @"Provides functions to handle resources (like fuel, ore, etc)"
    )]
    public partial class KSPResourceModule {
        [KSFunction(
            "find_resource"
        )]
        public static Result<ResourceDefinitionAdapter, string> FindResource(string resourceName) {
            foreach (var resourceDefinition in PartResourceLibrary.Instance.resourceDefinitions) {
                if (string.Equals(resourceDefinition.name, resourceName, StringComparison.InvariantCultureIgnoreCase)) {
                    return new Result<ResourceDefinitionAdapter, string>(true,
                        new ResourceDefinitionAdapter(resourceDefinition), null);
                }
            }
            return new Result<ResourceDefinitionAdapter, string>(false, null, $"No resource ${resourceName} found");
        }
    }
}
