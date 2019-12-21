using System.Collections.Generic;
using KontrolSystem.TO2.Binding;

namespace KontrolSystem.TO2.Runtime {
    [KSModule("core::logging",
        Description = "Provides basic logging. In KSP all log messages will apear in the debug console as well as the `KSP.log` file."
    )]
    public class CoreLogging {
        protected readonly ITO2Logger logger;

        public CoreLogging(IContext context, Dictionary<string, object> modules) => logger = context.Logger;

        [KSFunction(
            Description = "Write a debug-level `message`."
        )]
        public void debug(string message) => logger.Debug(message);

        [KSFunction(
            Description = "Write an info-level `message`."
        )]
        public void info(string message) => logger.Info(message);

        [KSFunction(
            Description = "Write a warning-level `message`."
        )]
        public void warning(string message) => logger.Warning(message);

        [KSFunction(
            Description = "Write an error-level `message`."
        )]
        public void error(string message) => logger.Error(message);
    }
}
