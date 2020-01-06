using System;
using System.Threading;

namespace KontrolSystem.TO2.Runtime {
    public interface IContext {
        ITO2Logger Logger {
            get;
        }

        void CheckTimeout();

        void ResetTimeout();
    }

    public class EmptyContext : IContext {
        private ConsoleLogger logger = new ConsoleLogger();

        public ITO2Logger Logger => logger;

        public void CheckTimeout() { }

        public void ResetTimeout() { }
    }

    public static class ContextHolder {
        public static readonly ThreadLocal<IContext> CurrentContext = new ThreadLocal<IContext>();

        public static void CheckTimeout() {
            IContext context = CurrentContext.Value;
            if(context != null) context.CheckTimeout(); else throw new ArgumentException("Running out of context");
        }
    }
}
