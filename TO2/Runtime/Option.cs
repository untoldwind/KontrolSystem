using System;

namespace KontrolSystem.TO2.Runtime {
    public interface IAnyOption {
        bool Defined { get; }
    }

    public struct Option<T> : IAnyOption {
        public readonly bool defined;
        public readonly T value;

        public Option(T _value) {
            defined = true;
            value = _value;
        }

        public bool Defined => defined;

        public Option<U> Map<U>(Func<T, U> mapper) => defined ? new Option<U>(mapper(value)) : new Option<U>();

        public Result<T, E> OkOr<E>(E error) => defined ? new Result<T, E>(value) : new Result<T, E>(false, error);

        public T GetValueOrDefault(T defaultValue) => defined ? value : defaultValue;
    }

    public static class Option {
        public static Option<T> Some<T>(T value) => new Option<T>(value);

        public static Option<T> None<T>() => new Option<T>();
    }
}
