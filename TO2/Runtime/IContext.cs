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
}
