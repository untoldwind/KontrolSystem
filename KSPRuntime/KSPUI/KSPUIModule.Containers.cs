using System;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass("Container")]
        public interface IContainer<T> {
            [KSMethod]
            ILabel Label(string label);

            [KSMethod]
            IButton Button(string label, Func<T, T> onClick);

            [KSMethod]
            ITextField TextField(string value, Func<T, string, T> onUpdate);

            [KSMethod]
            IContainer<T> VerticalLayout();

            [KSMethod]
            IContainer<T> HorizontalLayout();
        }
    }
}
