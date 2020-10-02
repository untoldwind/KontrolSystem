using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPConsole {
    [KSModule("ksp::console", Description =
        @"Provides functions to interact with the in-game KontrolSystem Console. As of now the console is output- and monochrome-only, this might change in the future.
        
          Additionally there is support for displaying popup messages on the HUD.
         "
    )]
    public partial class KSPConsoleModule {
        [KSConstant("RED", Description = "Color red")]
        public static readonly RgbaColor RedColor = new RgbaColor(1.0, 0.0, 0.0, 1.0);

        [KSConstant("YELLOW", Description = "Color yellow")]
        public static readonly RgbaColor YellowColor = new RgbaColor(1.0, 1.0, 0.0, 1.0);

        [KSConstant("GREEN", Description = "Color green")]
        public static readonly RgbaColor GreenColor = new RgbaColor(0.0, 1.0, 0.0, 1.0);

        [KSConstant("CYAN", Description = "Color cyan")]
        public static readonly RgbaColor CyanColor = new RgbaColor(0.0, 1.0, 1.0, 1.0);

        [KSConstant("BLUE", Description = "Color blue")]
        public static readonly RgbaColor BlueColor = new RgbaColor(0.0, 0.0, 1.0, 1.0);

        [KSFunction(
            Description = "Clear the console of all its content and move cursor to (0, 0)."
        )]
        public static void Clear() => KSPContext.CurrentContext.ConsoleBuffer?.Clear();

        [KSFunction(
            Description = "Print a message at the current cursor position (and move cursor forward)"
        )]
        public static void Print(string message) => KSPContext.CurrentContext.ConsoleBuffer?.Print(message);

        [KSFunction(
            Description =
                "Print a message at the current cursor position and move cursor to the beginning of the next line."
        )]
        public static void PrintLine(string message) => KSPContext.CurrentContext.ConsoleBuffer?.PrintLine(message);

        [KSFunction(
            Description = "Shortcut for `move_cursor(row, col)` followed by `print(message)`"
        )]
        public static void PrintAt(long row, long column, string message) {
            KSPContext.CurrentContext.ConsoleBuffer?.MoveCursor((int) row, (int) column);
            KSPContext.CurrentContext.ConsoleBuffer?.Print(message);
        }

        [KSFunction(
            Description = "Move the cursor to a give `row` and `column`."
        )]
        public static void MoveCursor(long row, long column) =>
            KSPContext.CurrentContext.ConsoleBuffer?.MoveCursor((int) row, (int) column);

        [KSFunction(
            Description =
                "Show a message on the HUD to inform the player that something extremely cool (or extremely uncool) has happed."
        )]
        public static void hud_text(string message, long seconds, long size, long styleSelect, RgbaColor color) {
            ScreenMessageStyle style;
            string htmlColour = color.ToHexNotation();

            switch (styleSelect) {
            case 1:
                style = ScreenMessageStyle.UPPER_LEFT;
                break;
            case 2:
                style = ScreenMessageStyle.UPPER_CENTER;
                break;
            case 3:
                style = ScreenMessageStyle.UPPER_RIGHT;
                break;
            default:
                style = ScreenMessageStyle.UPPER_CENTER;
                break;
            }

            ScreenMessages.PostScreenMessage($"<color={htmlColour}><size={size}>{message}</size></color>", seconds,
                style);
        }

        [KSFunction(
            Description = "Create a new color from `red`, `green`, `blue` and `alpha` (0.0 - 1.0)."
        )]
        public static RgbaColor Color(double red, double green, double blue, double alpha) =>
            new RgbaColor(red, green, blue, alpha);
    }
}
