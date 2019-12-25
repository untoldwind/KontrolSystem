using System.Collections.Generic;
using KontrolSystem.Parsing;

namespace KontrolSystem.TO2 {
    public struct StructuralError {
        public enum ErrorType {
            ArgumentMismatch,
            CoreGeneration,
            DublicateConstantName,
            DublicateFunctionName,
            DuplicateTypeName,
            DublicateVariableName,
            IncompatibleTypes,
            InvalidScope,
            InvalidImport,
            InvalidOperator,
            InvalidType,
            NoIndexAccess,
            NoSuchField,
            NoSuchFunction,
            NoSuchMethod,
            NoSuchModule,
            NoSuchVariable
        };

        public readonly ErrorType errorType;
        public readonly string message;
        public readonly Position start;
        public readonly Position end;

        public StructuralError(ErrorType _errorType, string _message, Position _start, Position _end) {
            errorType = _errorType;
            message = _message;
            start = _start;
            end = _end;
        }

        public override string ToString() => $"{start}: ERROR {errorType}: {message}";
    }

    public class CompilationErrorException : System.Exception {
        public List<StructuralError> errors;

        public CompilationErrorException(List<StructuralError> _errors) : base($"{_errors.Count} structural errors") {
            errors = _errors;
        }
    }
}
