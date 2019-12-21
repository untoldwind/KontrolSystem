namespace KontrolSystem.TO2.Runtime {
    public interface IAnyFutureResult {
        bool IsReady { get; }

        object ValueObject { get; }
    }

    public struct FutureResult<T> : IAnyFutureResult {
        public readonly bool ready;
        public readonly T value;

        public FutureResult(T _value) {
            ready = true;
            value = _value;
        }

        public bool IsReady => ready;

        public object ValueObject => value;
    }

    public interface IAnyFuture {
        IAnyFutureResult Poll();
    }

    public abstract class Future<T> : IAnyFuture {
        public IAnyFutureResult Poll() => PollValue();

        public abstract FutureResult<T> PollValue();
    }

    public static class Future {
        public class Success<T> : Future<T> {
            private readonly T value;

            public Success(T _value) => value = _value;

            public override FutureResult<T> PollValue() => new FutureResult<T>(value);
        }
    }
}
