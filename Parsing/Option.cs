using System;

namespace KontrolSystem.Parsing {
    public interface IOption<out T> {
        bool IsEmpty { get; }

        bool IsDefined { get; }

        T Value { get; }

        IOption<U> Map<U>(Func<T, U> convert);
    }

    public static class Option {
        public static IOption<T> some<T>(T value) => new Some<T>(value);

        public static IOption<T> none<T>() => new None<T>();

        public static T GetOrElse<T>(this IOption<T> option, T defaultValue) =>
            option.IsEmpty ? defaultValue : option.Value;

        internal struct Some<T> : IOption<T> {
            private T _value;

            internal Some(T value) => _value = value;

            public bool IsEmpty => false;

            public bool IsDefined => true;

            public T Value => _value;

            public IOption<U> Map<U>(Func<T, U> convert) => new Some<U>(convert(_value));
        }

        internal struct None<T> : IOption<T> {
            public bool IsEmpty => true;

            public bool IsDefined => false;

            public T Value => throw new InvalidOperationException("None has no value");

            public IOption<U> Map<U>(Func<T, U> convert) => new None<U>();
        }
    }
}
