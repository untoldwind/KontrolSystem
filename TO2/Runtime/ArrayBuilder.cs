using System.Collections.Generic;

namespace KontrolSystem.TO2.Runtime {
    public class ArrayBuilder<T> {
        private readonly List<T> elements;

        public ArrayBuilder(long capacity) => elements = new List<T>((int)capacity);

        public long Length => elements.Count;

        public ArrayBuilder<T> Append(T element) {
            elements.Add(element);
            return this;
        }

        public T[] Result() => elements.ToArray();
    }
}
