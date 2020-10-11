using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPOrbit {
    public partial class KSPOrbitModule {
        [KSClass("Targetable")]
        public interface IKSPTargetable {
            [KSField] string Name { get; }

            [KSField] IOrbit Orbit { get; }

            ITargetable Underlying { get; }
        }
    }
}
