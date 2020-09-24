namespace KontrolSystem.TO2.Runtime {
    public interface IAnyResult {
        bool Success { get; }

        string ErrorString { get; }
    }

    public struct Result<T, E> : IAnyResult {
        public readonly bool success;
        public readonly T value;
        public readonly E error;

        public Result(T value) {
            success = true;
            this.value = value;
            error = default(E);
        }

        public Result(bool failure, E error) {
            success = false;
            value = default(T);
            this.error = error;
        }

        public bool Success => success;

        public string ErrorString => error?.ToString();
    }

    public static class Result {
        public static Result<T, E> Ok<T, E>(T value) => new Result<T, E>(value);

        public static Result<T, E> Err<T, E>(E error) => new Result<T, E>(true, error);
    }
}
