using System;
using System.Collections.Generic;
using System.Linq;

namespace KontrolSystem.Parsing {
    public interface IResult<out T> {
        T Value {
            get;
        }

        IInput Remaining {
            get;
        }

        bool WasSuccessful {
            get;
        }

        IEnumerable<string> Expected {
            get;
        }

        Position Position {
            get;
        }

        IResult<U> Map<U>(Func<T, U> f);

        IResult<U> Select<U>(Func<IResult<T>, IResult<U>> next);
    }

    public static class Result {
        public static IResult<T> success<T>(IInput remaining, T value) => new Success<T>(remaining, value);

        public static IResult<T> failure<T>(IInput input, string expected) => new Failure<T>(input, expected.Yield());

        public static IResult<T> failure<T>(IInput input, IEnumerable<string> expected) => new Failure<T>(input, expected);

        internal struct Success<T> : IResult<T> {
            private T _value;
            private IInput remaining;

            internal Success(IInput remaining, T value) {
                this.remaining = remaining;
                _value = value;
            }

            public T Value => _value;

            public IInput Remaining => remaining;

            public bool WasSuccessful => true;

            public IEnumerable<string> Expected => Enumerable.Empty<string>();

            public Position Position => remaining.Position;

            public IResult<U> Map<U>(Func<T, U> f) => new Success<U>(remaining, f(_value));

            public IResult<U> Select<U>(Func<IResult<T>, IResult<U>> next) => next(this);
        }

        internal struct Failure<T> : IResult<T> {
            private IEnumerable<string> expected;

            private IInput input;

            internal Failure(IInput input, IEnumerable<string> expected) {
                this.input = input;
                this.expected = expected;
            }

            public T Value => throw new InvalidOperationException("Failure has no value");

            public IInput Remaining => input;

            public bool WasSuccessful => false;

            public IEnumerable<string> Expected => expected;

            public Position Position => input.Position;

            public IResult<U> Map<U>(Func<T, U> f) => new Failure<U>(input, expected);

            public IResult<U> Select<U>(Func<IResult<T>, IResult<U>> next) => new Failure<U>(input, expected);
        }
    }
}
