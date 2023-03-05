namespace ElfScript.Errors {
    internal static class ErrorFactory {
        public static ElfScriptException TypeError(string variableName,Type expectedType) {
            return new($"{nameof(TypeError)}: '{variableName}' is not of type '{expectedType}'.");
        }
        public static ElfScriptException ReferenceError(string variableName) {
            return new($"{nameof(ReferenceError)}: '{variableName}' was not found.");
        }
        public static ElfScriptException ParameterMismatch(int expectedCount,int count) {
            return new($"{nameof(ParameterMismatch)}: Function defined with arity {expectedCount}, but invoked with arity {count}.");
        }
        public static ElfScriptException InvalidIndexOperation(string variableName) {
            return new($"{nameof(InvalidIndexOperation)}: {variableName} is not a collection type.");
        }
        public static ElfScriptException InvalidIndexDeleteOperation(string variableName) {
            return new($"{nameof(InvalidIndexDeleteOperation)}: Cannot delete subvalue of '{variableName}' because it is not a table.");
        }
        public static ElfScriptException IndexTypeError(Type expectedType) {
            return new($"{nameof(TypeError)}: Index value is not of type '{expectedType}'.");
        }
        public static ElfScriptException StackUnderflow() {
            return new($"{nameof(StackUnderflow)}: Cannot pop stack frame from top level.");
        }
        public static ElfScriptException OperatorNotImplemented(Type a,Type b,Operator operatorType) {
            return new($"{nameof(OperatorNotImplemented)}: Operator '{operatorType}' is not implemented between type '{a}' and '{b}'");
        }
        public static ElfScriptException IllegalFunctionExecution() {
            return new($"{nameof(IllegalFunctionExecution)}: A function block cannot be evaluated as a standard expression.");
        }
        public static ElfScriptException IllegalDeletionOfPinnedValue() {
            return new($"{nameof(IllegalDeletionOfPinnedValue)}: Cannot delete a pinned memory value. You must first unpin the value.");
        }
    }
}
