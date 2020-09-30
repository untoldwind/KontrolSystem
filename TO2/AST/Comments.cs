using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class LineComment : IBlockItem, IModuleItem {
        private Position start;
        private Position end;
        private readonly string comment;

        public LineComment(string comment, Position start, Position end) {
            this.comment = comment;
            this.start = start;
            this.end = end;
        }

        public bool IsComment => true;

        public Position Start => start;

        public Position End => end;

        public void SetVariableContainer(IVariableContainer container) {
        }

        public void SetTypeHint(TypeHint typeHint) {
        }

        public TO2Type ResultType(IBlockContext context) => BuildinType.Unit;

        public void EmitCode(IBlockContext context, bool dropResult) {
        }

        public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) =>
            Enumerable.Empty<StructuralError>();
    }
}
