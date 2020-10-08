using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass("WindowHandle")]
        public interface IWindowHandle<T> {
            [KSField] T State { get; set; }

            [KSField] bool Closed { get; }

            [KSMethod]
            void Close();
        }
    }
}
