using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass("Window")]
        public interface IWindow<T> {
            [KSField(IncludeSetter = true)] T State { get; set; }
            
            [KSField] bool Closed { get; }

            [KSMethod]
            void Close();
        }
    }
}
