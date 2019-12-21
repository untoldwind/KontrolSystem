using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KontrolSystem.TO2.Generator;

namespace KontrolSystem.TO2.AST {
    public interface IOperatorCollection {
        IOperatorEmitter GetMatching(ModuleContext context, Operator op, TO2Type otherType);
    }

    public class OperatorCollection : IEnumerable<IOperatorEmitter>, IOperatorCollection {
        private readonly Dictionary<Operator, List<IOperatorEmitter>> collection = new Dictionary<Operator, List<IOperatorEmitter>>();

        public void Add(Operator op, IOperatorEmitter operatorEmitter) {
            if (collection.ContainsKey(op))
                collection[op].Add(operatorEmitter);
            else
                collection.Add(op, new List<IOperatorEmitter> { operatorEmitter });
        }
        public IOperatorEmitter GetMatching(ModuleContext context, Operator op, TO2Type otherType) {
            if (!collection.ContainsKey(op)) return null;
            return collection[op].Find(o => o.Accepts(context, otherType));
        }

        public IEnumerator<IOperatorEmitter> GetEnumerator() => collection.Values.SelectMany(o => o).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
