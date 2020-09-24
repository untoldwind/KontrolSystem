using System.Collections.Generic;
using KontrolSystem.TO2.AST;

namespace KontrolSystem.TO2.Parser {
    public interface ISuffixOperation { }

    public struct IndexGetSuffix : ISuffixOperation {
        public readonly IndexSpec indexSpec;

        public IndexGetSuffix(IndexSpec indexSpec) => this.indexSpec = indexSpec;
    }

    public struct FieldGetSuffix : ISuffixOperation {
        public readonly string fieldName;

        public FieldGetSuffix(string fieldName) => this.fieldName = fieldName;
    }

    public struct MethodCallSuffix : ISuffixOperation {
        public readonly string methodName;
        public readonly List<Expression> arguments;

        public MethodCallSuffix(string methodName, List<Expression> arguments) {
            this.methodName = methodName;
            this.arguments = arguments;
        }
    }

    public struct OperatorSuffix : ISuffixOperation {
        public readonly Operator op;

        public OperatorSuffix(Operator _op) => op = _op;
    }
}
