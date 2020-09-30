using System.Collections.Generic;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Runtime {
    [KSModule("core::logging",
        Description =
            "Provides basic logging. In KSP all log messages will apear in the debug console as well as the `KSP.log` file."
    )]
    public class CoreLogging {
        public static ITO2Logger Logger => ContextHolder.CurrentContext.Value?.Logger;

        [KSFunction(
            Description = "Write a debug-level `message`."
        )]
        public static void debug(string message) => Logger?.Debug(message);

        [KSFunction(
            Description = "Write an info-level `message`."
        )]
        public static void info(string message) => Logger?.Info(message);

        [KSFunction(
            Description = "Write a warning-level `message`."
        )]
        public static void warning(string message) => Logger?.Warning(message);

        [KSFunction(
            Description = "Write an error-level `message`."
        )]
        public static void error(string message) => Logger?.Error(message);
    }
}
