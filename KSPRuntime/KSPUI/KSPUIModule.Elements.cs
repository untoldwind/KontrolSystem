using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPUI {
    public partial class KSPUIModule {
        [KSClass("Label")]
        public interface ILabel {
            [KSMethod]
            ILabel Width(double width);

            [KSMethod]
            ILabel MinWidth(double width);

            [KSMethod]
            ILabel MaxWidth(double width);

            [KSMethod]
            ILabel ExpandWidth();
            
            [KSMethod]
            ILabel Height(double height);

            [KSMethod]
            ILabel MinHeight(double height);

            [KSMethod]
            ILabel MaxHeight(double height);
            
            [KSMethod]
            ILabel ExpandHeight();
        }

        [KSClass("Button")]
        public interface IButton {
            [KSMethod]
            IButton Width(double width);

            [KSMethod]
            IButton MinWidth(double width);

            [KSMethod]
            IButton MaxWidth(double width);

            [KSMethod]
            IButton ExpandWidth();
            
            [KSMethod]
            IButton Height(double height);

            [KSMethod]
            IButton MinHeight(double height);

            [KSMethod]
            IButton MaxHeight(double height);
            
            [KSMethod]
            IButton ExpandHeight();
        }

        [KSClass("TextField")]
        public interface ITextField {
            [KSMethod]
            ITextField MaxLength(long maxLength);
            
            [KSMethod]
            ITextField Width(double width);

            [KSMethod]
            ITextField MinWidth(double width);

            [KSMethod]
            ITextField MaxWidth(double width);

            [KSMethod]
            ITextField ExpandWidth();
            
            [KSMethod]
            ITextField Height(double height);

            [KSMethod]
            ITextField MinHeight(double height);

            [KSMethod]
            ITextField MaxHeight(double height);
            
            [KSMethod]
            ITextField ExpandHeight();
        }
    }
}
