using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;
using KontrolSystem.TO2.Runtime;

namespace KontrolSystem.TO2.AST {
    public class ConstDeclaration : Node, IModuleItem {
        public readonly bool isPublic;
        public readonly string name;
        public readonly string description;
        public readonly TO2Type type;
        public readonly Expression expression;

        public ConstDeclaration(bool _isPublic, string _name, string _description, TO2Type _type, Expression _expression, Position start = new Position(), Position end = new Position()) : base(start, end) {
            isPublic = _isPublic;
            name = _name;
            description = _description;
            type = _type;
            expression = _expression;
            expression.SetTypeHint(context => type.UnderlyingType(context.ModuleContext));
        }

        public IEnumerable<StructuralError> TryDeclareTypes(ModuleContext context) => Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportConstants(ModuleContext context) => Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportFunctions(ModuleContext context) => Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryImportTypes(ModuleContext context) => Enumerable.Empty<StructuralError>();

        public IEnumerable<StructuralError> TryVerifyFunctions(ModuleContext context) => Enumerable.Empty<StructuralError>();
    }
}
