namespace ElfScript.Errors {
    internal static class ErrorFactory {
        public static ElfScriptException ValueTypeError(string variableName,Type expectedType) {
            return new($"{nameof(ValueTypeError)}: '{variableName}' is not of type '{expectedType}'.");
        }
        public static ElfScriptException VariableReferenceError(string variableName) {
            return new($"{nameof(VariableReferenceError)}: '{variableName}' was not found.");
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
        public static ElfScriptException IndexValueTypeError(Type expectedType) {
            return new($"{nameof(IndexValueTypeError)}: Index value is not of type '{expectedType}'.");
        }
        public static ElfScriptException StackUnderflow() {
            return new($"{nameof(StackUnderflow)}: Cannot pop stack frame from top level.");
        }
        public static ElfScriptException OperatorNotImplemented(Type a,Type b,Operator operatorType) {
            return new($"{nameof(OperatorNotImplemented)}: Operator '{operatorType}' is not implemented between type '{a}' and '{b}'");
        }
        public static ElfScriptException IllegalDeletionOfPinnedValue() {
            return new($"{nameof(IllegalDeletionOfPinnedValue)}: Cannot delete a pinned memory value. You must first unpin the value.");
        }
        public static ElfScriptException InternalMemoryReferenceError(int ID,Type expectedType) {
            return new($"{nameof(InternalMemoryReferenceError)}: Value for ID:{ID} not found, expected type is {expectedType}.");
        }
        public static ElfScriptException InternalMemoryTypeError(int ID,Type expectedType,Type currentType) {
            return new($"{nameof(InternalMemoryTypeError)}: Value for ID:{ID} has incorrect type. Expected type is {expectedType}, found type {currentType}.");
        }
    }
}
