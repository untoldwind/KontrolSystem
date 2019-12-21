using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public abstract class Node {
        private readonly Position _start;
        private readonly Position _end;

        public Node(Position start, Position end) {
            _start = start;
            _end = end;
        }

        public Position Start => _start;

        public Position End => _end;
    }
}
