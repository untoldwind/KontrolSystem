using System;
using System.Text;

namespace KontrolSystem.TO2.Runtime {
    public static class ArrayMethods {
        public static U[] Map<T, U>(T[] source, Func<T, U> mapper) {
            U[] result = new U[source.Length];

            for (int i = 0; i < source.Length; i++)
                result[i] = mapper(source[i]);

            return result;
        }

        public static U[] MapWithIndex<T, U>(T[] source, Func<T, long, U> mapper) {
            U[] result = new U[source.Length];

            for (int i = 0; i < source.Length; i++)
                result[i] = mapper(source[i], i);

            return result;
        }

        public static void Set<T>(T[] array, long index, T element) => array[index] = element;

        public static Option<T> Find<T>(T[] source, Func<T, bool> predicate) {
            for (int i = 0; i < source.Length; i++) {
                if(predicate(source[i])) return new Option<T>(source[i]);
            }

            return new Option<T>();
        }
        
        public static string ArrayToString<T>(T[] array) {
            StringBuilder builder = new StringBuilder("[");

            for (int i = 0; i < array.Length; i++) {
                if (i > 0) builder.Append(", ");
                builder.Append(array[i]);
            }

            builder.Append("]");
            return builder.ToString();
        }
    }
}
