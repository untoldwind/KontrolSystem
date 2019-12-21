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
            private IInput _remainging;

            internal Success(IInput remainging, T value) {
                _remainging = remainging;
                _value = value;
            }

            public T Value => _value;

            public IInput Remaining => _remainging;

            public bool WasSuccessful => true;

            public IEnumerable<string> Expected => Enumerable.Empty<string>();

            public Position Position => _remainging.Position;

            public IResult<U> Map<U>(Func<T, U> f) => new Success<U>(_remainging, f(_value));

            public IResult<U> Select<U>(Func<IResult<T>, IResult<U>> next) => next(this);
        }

        internal struct Failure<T> : IResult<T> {
            private IEnumerable<string> _expected;

            private IInput _input;

            internal Failure(IInput input, IEnumerable<string> expected) {
                _input = input;
                _expected = expected;
            }

            public T Value => throw new InvalidOperationException("Failure has no value");

            public IInput Remaining => _input;

            public bool WasSuccessful => false;

            public IEnumerable<string> Expected => _expected;

            public Position Position => _input.Position;

            public IResult<U> Map<U>(Func<T, U> f) => new Failure<U>(_input, _expected);

            public IResult<U> Select<U>(Func<IResult<T>, IResult<U>> next) => new Failure<U>(_input, _expected);
        }
    }
}
