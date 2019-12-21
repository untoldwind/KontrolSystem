using System.Collections.Generic;
using KontrolSystem.KSP.Runtime;
using KontrolSystem.TO2.Runtime;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.KSP.Runtime.KSPConsole {
    [KSModule("ksp::console", Description =
        @"Provides functions to interact with the in-game KontrolSystem Console. As of now the console is output- and monochrome-only, this might change in the future.
        
          Additionally there is support for displaying popup messages on the HUD.
         "
    )]
    public partial class KSPConsoleModule {
        IKSPContext context;

        public KSPConsoleModule(IContext _context, Dictionary<string, object> modules) => context = _context as IKSPContext;


        [KSFunction(
            Description = "Clear the console of all its content and move cursor to (0, 0)."
        )]
        public void Clear() => context.ConsoleBuffer?.Clear();

        [KSFunction(
            Description = "Print a message at the current cursor position (and move cursor forward)"
        )]
        public void Print(string message) => context.ConsoleBuffer?.Print(message);

        [KSFunction(
            Description = "Print a message at the current cursor position and move cursor to the beginning of the next line."
        )]
        public void PrintLine(string message) => context.ConsoleBuffer?.PrintLine(message);

        [KSFunction(
            Description = "Shortcut for `move_cursor(row, col)` followed by `print(message)`"
        )]
        public void PrintAt(long row, long column, string message) {
            context.ConsoleBuffer?.MoveCursor((int)row, (int)column);
            context.ConsoleBuffer?.Print(message);
        }

        [KSFunction(
            Description = "Move the cursor to a give `row` and `column`."
        )]
        public void MoveCursor(long row, long column) => context.ConsoleBuffer?.MoveCursor((int)row, (int)column);

        [KSFunction(
            Description = "Show a message on the HUD to inform the player that something extremely cool (or extremely uncool) has happed."
        )]
        public void hud_text(string message, long seconds, long size, long styleSelect, RgbaColor color) {
            ScreenMessageStyle style;
            string htmlColour = color.ToHexNotation();

            switch (styleSelect) {
            case 1: style = ScreenMessageStyle.UPPER_LEFT; break;
            case 2: style = ScreenMessageStyle.UPPER_CENTER; break;
            case 3: style = ScreenMessageStyle.UPPER_RIGHT; break;
            case 4:
            default: style = ScreenMessageStyle.UPPER_CENTER; break;
            }

            ScreenMessages.PostScreenMessage($"<color={htmlColour}><size={size}>{message}</size></color>", seconds, style);
        }

        [KSFunction(
            Description = "Create a new color from `red`, `green`, `blue` and `alpha` (0.0 - 1.0)."
        )]
        public RgbaColor Color(double red, double green, double blue, double alpha) => new RgbaColor(red, green, blue, alpha);

    }
}
