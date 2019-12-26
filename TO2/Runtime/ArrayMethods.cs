using System;

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
                result[i] = mapper(source[i], (long)i);

            return result;
        }

        public static void Set<U>(U[] array, long index, U element) => array[index] = element;
    }
}
