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

            [KSMethod]
            IContainer<T> Width(double width);

            [KSMethod]
            IContainer<T> MinWidth(double width);

            [KSMethod]
            IContainer<T> MaxWidth(double width);

            [KSMethod]
            IContainer<T> ExpandWidth();

            [KSMethod]
            IContainer<T> Height(double height);

            [KSMethod]
            IContainer<T> MinHeight(double height);

            [KSMethod]
            IContainer<T> MaxHeight(double height);

            [KSMethod]
            IContainer<T> ExpandHeight();
        }
    }
}
