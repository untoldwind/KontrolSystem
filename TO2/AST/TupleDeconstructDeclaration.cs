using System.Collections.Generic;
using System.Reflection.Emit;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public class TupleDeconstructDeclaration : Node, IBlockItem {
        public readonly List<DeclarationParameter> declarations;
        public readonly bool isConst;
        public readonly Expression expression;
        private IVariableContainer variableContainer;

        public TupleDeconstructDeclaration(List<DeclarationParameter> _declarations, bool _isConst, Expression _experssion, Position start = new Position(), Position end = new Position()) : base(start, end) {
            declarations = _declarations;
            isConst = _isConst;
            expression = _experssion;
            expression.SetTypeHint(context => new TupleType(declarations.Select(d => d.type ?? BuildinType.Unit).ToList()));
        }

        public bool IsComment => false;

        public void SetVariableContainer(IVariableContainer container) {
            expression.SetVariableContainer(container);
            variableContainer = container;
        }

        public void SetTypeHint(TypeHint _typeHint) { }

        public TO2Type ResultType(IBlockContext context) => BuildinType.Unit;

        public void EmitCode(IBlockContext context, bool dropResult) {
            RealizedType valueType = expression.ResultType(context).UnderlyingType(context.ModuleContext);

            switch (valueType) {
            case TupleType tupleType:
                EmitCodeTuple(context, dropResult, tupleType);
                return;
            case RecordType recordType:
                EmitCodeRecord(context, dropResult, recordType);
                return;
            default:
                context.AddError(new StructuralError(
                            StructuralError.ErrorType.InvalidType,
                            $"Expected right side to be a tuple or record, but got {valueType}",
                            Start,
                            End
                        ));
                return;
            }
        }

        private void EmitCodeTuple(IBlockContext context, bool dropResult, TupleType tupleType) {
            if (tupleType.itemTypes.Count != declarations.Count) {
                context.AddError(new StructuralError(
                                       StructuralError.ErrorType.InvalidType,
                                       $"Expected right side to be a tuple with {declarations.Count} elements, but got {tupleType}",
                                       Start,
                                       End
                                   ));
                return;
            }

            expression.EmitCode(context, false);

            if (context.HasErrors) return;

            for (int i = 0; i < declarations.Count; i++) {
                DeclarationParameter declaration = declarations[i];

                if (declaration.IsPlaceholder) continue;

                TO2Type variableType = declaration.IsInferred ? tupleType.itemTypes[i] : declaration.type;

                if (context.FindVariable(declaration.name) != null) {
                    context.AddError(new StructuralError(
                                           StructuralError.ErrorType.DublicateVariableName,
                                           $"Variable '{declaration.name}' already declared in this scope",
                                           Start,
                                           End
                                       ));
                    return;
                }
                if (!variableType.IsAssignableFrom(context.ModuleContext, tupleType.itemTypes[i])) {
                    context.AddError(new StructuralError(
                                           StructuralError.ErrorType.IncompatibleTypes,
                                           $"Expected element {i} of {tupleType} to be of type {variableType}",
                                           Start,
                                           End
                                       ));
                    return;
                }

                IBlockVariable variable = context.DeclaredVariable(declaration.name, isConst, variableType.UnderlyingType(context.ModuleContext));

                context.IL.Emit(OpCodes.Dup);
                tupleType.FindField(context.ModuleContext, $"_{i + 1}").Create(context.ModuleContext).EmitLoad(context);

                variable.EmitStore(context);
            }
            context.IL.Emit(OpCodes.Pop);
        }

        private void EmitCodeRecord(IBlockContext context, bool dropResult, RecordType recordType) {
            expression.EmitCode(context, false);

            if (context.HasErrors) return;

            for (int i = 0; i < declarations.Count; i++) {
                DeclarationParameter declaration = declarations[i];

                if (declaration.IsPlaceholder) continue;

                if (!recordType.ItemTypes.ContainsKey(declaration.name)) {
                    context.AddError(new StructuralError(
                                           StructuralError.ErrorType.IncompatibleTypes,
                                           $"{recordType} does not have a field '{declaration.name}'",
                                           Start,
                                           End
                                       ));
                    return;
                }

                if (declaration.IsPlaceholder) continue;

                TO2Type variableType = declaration.IsInferred ? recordType.ItemTypes[declaration.name] : declaration.type;

                if (context.FindVariable(declaration.name) != null) {
                    context.AddError(new StructuralError(
                                           StructuralError.ErrorType.DublicateVariableName,
                                           $"Variable '{declaration.name}' already declared in this scope",
                                           Start,
                                           End
                                       ));
                    return;
                }
                if (!variableType.IsAssignableFrom(context.ModuleContext, recordType.ItemTypes[declaration.name])) {
                    context.AddError(new StructuralError(
                                           StructuralError.ErrorType.IncompatibleTypes,
                                           $"Expected element {declaration.name} of {recordType} to be of type {variableType}",
                                           Start,
                                           End
                                       ));
                    return;
                }

                IBlockVariable variable = context.DeclaredVariable(declaration.name, isConst, variableType.UnderlyingType(context.ModuleContext));

                context.IL.Emit(OpCodes.Dup);
                recordType.FindField(context.ModuleContext, declaration.name).Create(context.ModuleContext).EmitLoad(context);

                variable.EmitStore(context);
            }
            context.IL.Emit(OpCodes.Pop);
        }

        public IEnumerable<IVariableRef> Refs => declarations.SelectMany((declaration, itemIdx) => declaration.IsPlaceholder ? Enumerable.Empty<IVariableRef>() : new TupleVariableRef(itemIdx, declaration, expression).Yield());
    }

    public class TupleVariableRef : IVariableRef {
        private readonly int itemIdx;
        private readonly DeclarationParameter declaration;
        private readonly Expression expression;
        private bool lookingUp = false;

        internal TupleVariableRef(int _itemIdx, DeclarationParameter _declaration, Expression _experssion) {
            itemIdx = _itemIdx;
            declaration = _declaration;
            expression = _experssion;
        }

        public string Name => declaration.name;

        public TO2Type VariableType(IBlockContext context) {
            if (!declaration.IsInferred) return declaration.type;

            if (lookingUp) return null;
            lookingUp = true; // Somewhat ugly workaround if there is a cycle in inferred variables that should produce a correct error message
            RealizedType inferred = expression.ResultType(context).UnderlyingType(context.ModuleContext);
            lookingUp = false;
            switch (inferred) {
            case TupleType tupleType:
                if (itemIdx >= tupleType.itemTypes.Count) return BuildinType.Unit;

                return tupleType.itemTypes[itemIdx];
            case RecordType recordType:
                return recordType.ItemTypes.Get(declaration.name) ?? BuildinType.Unit;
            default:
                return BuildinType.Unit;
            }
        }
    }
}
