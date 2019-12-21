namespace KontrolSystem.TO2.Runtime {
    public class Cell<T> {
        private readonly object cellLock;
        private T element;

        public Cell(T value) {
            element = value;
            cellLock = new object();
        }

        public T Value {
            get {
                lock (cellLock) {
                    return element;
                }
            }
            set {
                lock (cellLock) {
                    element = value;
                }
            }
        }
    }
}
