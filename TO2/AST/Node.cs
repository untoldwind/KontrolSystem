using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public abstract class Node {
        private readonly Position start;
        private readonly Position end;

        protected Node(Position start, Position end) {
            this.start = start;
            this.end = end;
        }

        public Position Start => start;

        public Position End => end;
    }
}
