using System;

namespace KontrolSystem.Parsing {
    public struct Position : IEquatable<Position> {
        public readonly string sourceName;

        public readonly int position;

        public readonly int line;

        public readonly int column;

        public Position(string _sourceName, int _position = 0, int _line = 1, int _column = 1) {
            sourceName = _sourceName;
            position = _position;
            line = _line;
            column = _column;
        }

        public override bool Equals(object obj) => Equals((Position)obj);

        public bool Equals(Position other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return position == other.position && line == other.line && column == other.column;
        }

        public static bool operator ==(Position left, Position right) => Equals(left, right);

        public static bool operator !=(Position left, Position right) => !Equals(left, right);

        public override int GetHashCode() {
            var h = 31;
            h = h * 13 + position;
            h = h * 13 + line;
            h = h * 13 + column;
            return h;
        }

        public override string ToString() => $"{sourceName}({line}, {column})";
    }
}
