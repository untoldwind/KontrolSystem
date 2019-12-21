using System.Collections.Generic;
using System.Linq;
using KontrolSystem.Parsing;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public interface IBlockItem {
        Position Start {
            get;
        }

        Position End {
            get;
        }

        bool IsComment {
            get;
        }

        void SetVariableContainer(IVariableContainer variableContainer);

        void SetTypeHint(TypeHint typeHint);

        TO2Type ResultType(IBlockContext context);

        void EmitCode(IBlockContext context, bool dropResult);
    }

    public interface IVariableRef {
        string Name {
            get;
        }

        TO2Type VariableType(IBlockContext context);
    }

    public class Block : Expression, IVariableContainer {
        public readonly List<IBlockItem> items;
        public readonly Dictionary<string, IVariableRef> variables;
        private IVariableContainer parentContainer;

        public Block(List<IBlockItem> _items, Position start = new Position(), Position end = new Position()) : base(start, end) {
            items = _items;
            variables = new Dictionary<string, IVariableRef>();
            foreach (IBlockItem item in items) {
                item.SetVariableContainer(this);
                switch (item) {
                case VariableDeclaration variable:
                    if (!variables.ContainsKey(variable.declaration.name))
                        variables.Add(variable.declaration.name, variable);
                    break;
                case TupleDeconstructDeclaration tuple:
                    foreach (IVariableRef r in tuple.Refs) {
                        if (!variables.ContainsKey(r.Name))
                            variables.Add(r.Name, r);
                    }
                    break;
                }
            }
        }

        public override void SetVariableContainer(IVariableContainer container) => parentContainer = container;

        public override void SetTypeHint(TypeHint typeHint) => items.LastOrDefault()?.SetTypeHint(typeHint);

        public IVariableContainer ParentContainer => parentContainer;

        public TO2Type FindVariableLocal(IBlockContext context, string name) => variables.Get(name)?.VariableType(context);

        public override TO2Type ResultType(IBlockContext context) => items.Where(item => !item.IsComment).LastOrDefault()?.ResultType(context) ?? BuildinType.Unit;

        public override void EmitCode(IBlockContext context, bool dropResult) {
            bool childScope = parentContainer is Block;
            IBlockContext effectiveContext = context;

            if (childScope) {
                context.IL.BeginScope();
                effectiveContext = context.CreateChildContext();
            }

            List<IBlockItem> nonComments = items.Where(item => !item.IsComment).ToList();
            int len = nonComments.Count;
            if (len > 0) {
                for (int i = 0; i < len - 1; i++) {
                    IBlockItem item = nonComments[i];
                    item.EmitCode(effectiveContext, true);
                }
                nonComments[len - 1].EmitCode(effectiveContext, dropResult);
            }

            if (childScope) context.IL.EndScope();
        }
    }
}
