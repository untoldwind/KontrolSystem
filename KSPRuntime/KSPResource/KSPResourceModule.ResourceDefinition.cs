using KontrolSystem.KSP.Runtime.KSPVessel;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPResource {
    public partial class KSPResourceModule {
        [KSClass("ResourceDefinition")]
        public class ResourceDefinitionAdapter {
            internal readonly PartResourceDefinition resourceDefinition;

            public ResourceDefinitionAdapter(PartResourceDefinition resourceDefinition) =>
                this.resourceDefinition = resourceDefinition;

            [KSField] public string Name => resourceDefinition.name;

            [KSField] public string DisplayName => resourceDefinition.displayName;

            [KSField] public double Density => resourceDefinition.density;

            [KSField] public double Volume => resourceDefinition.volume;

            [KSField] public double UnitCost => resourceDefinition.unitCost;

            [KSField] public string TransferMode => resourceDefinition.resourceTransferMode.ToString();

            [KSMethod]
            public ResourceTransfer StartResourceTransfer(KSPVesselModule.PartAdapter[] transferFrom,
                KSPVesselModule.PartAdapter[] transferTo, double amount) {
                return new ResourceTransfer(KSPContext.CurrentContext, this, transferFrom, transferTo, amount);
            }
        }
    }
}
