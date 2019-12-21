using System;
namespace KontrolSystem.Parsing {
    /// <summary>
    /// Entire input for parsing is available as string.
    /// </summary>
    public struct StringInput : IInput {
        private string _source;
        private Position _position;

        public StringInput(string source, string sourceName = "<inline>") {
            _source = source;
            _position = new Position(sourceName);
        }

        private StringInput(string source, Position position) {
            _source = source;
            _position = position;
        }

        public char Current => _source[_position.position];

        public int Available => _source.Length - _position.position;

        public Position Position => _position;

        public int FindNext(Predicate<char> predicate) {
            for (int p = _position.position; p < _source.Length; p++) {
                if (predicate(_source[p])) return p - _position.position;
            }
            return -1;
        }

        public string Take(int count) {
            if (count == 0) return "";
            if (count + _position.position > _source.Length) throw new InvalidOperationException("Advance beyond eof");

            return _source.Substring(_position.position, count);
        }

        public IInput Advance(int count) {
            if (count == 0) return this;
            if (count + _position.position > _source.Length) throw new InvalidOperationException("Advance beyond eof");

            int line = _position.line;
            int column = _position.column;

            for (int p = _position.position; p < _position.position + count; p++) {
                column++;
                if (_source[p] == '\n') {
                    line++;
                    column = 1;
                }
            }

            return new StringInput(_source, new Position(_position.sourceName, _position.position + count, line, column));
        }

        override public string ToString() => _source.Substring(_position.position);
    }
}
