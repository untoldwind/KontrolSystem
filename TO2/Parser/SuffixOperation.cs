using System.Collections.Generic;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser {
    public interface ISuffixOperation { }

    public struct IndexGetSuffix : ISuffixOperation {
        public readonly IndexSpec indexSpec;

        public IndexGetSuffix(IndexSpec _indexSpec) => indexSpec = _indexSpec;
    }

    public struct FieldGetSuffix : ISuffixOperation {
        public readonly string fieldName;

        public FieldGetSuffix(string _fieldName) => fieldName = _fieldName;
    }

    public struct MethodCallSuffix : ISuffixOperation {
        public readonly string methodName;
        public readonly List<Expression> arguments;

        public MethodCallSuffix(string _methodName, List<Expression> _arguments) {
            methodName = _methodName;
            arguments = _arguments;
        }
    }

    public struct OperatorSuffix : ISuffixOperation {
        public readonly Operator op;

        public OperatorSuffix(Operator _op) => op = _op;
    }
}
