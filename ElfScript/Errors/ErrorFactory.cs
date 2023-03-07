using ElfScript.VirtualMachine.Memory;

namespace ElfScript.Errors {
    internal static class ErrorFactory {
        public static ElfScriptException ValueTypeError(Type expectedType) {
            return new($"{nameof(ValueTypeError)}: Value is not of type '{expectedType}'.");
        }
        public static ElfScriptException ValueTypeError(string variableName,Type expectedType) {
            return new($"{nameof(ValueTypeError)}: '{variableName}' is not of type '{expectedType}'.");
        }
        public static ElfScriptException VariableReferenceError(string variableName) {
            return new($"{nameof(VariableReferenceError)}: '{variableName}' was not found.");
        }
        public static ElfScriptException FunctionParameterMismatch(int expectedCount,int count) {
            return new($"{nameof(FunctionParameterMismatch)}: Function defined with arity {expectedCount}, but invoked with arity {count}.");
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
        public static ElfScriptException MemoryReferenceError(Address address) {
            return new($"{nameof(MemoryReferenceError)}: Value for address '{address}' not found.");
        }
        public static ElfScriptException MemoryTypeError(Address address,Type expectedType,Type currentType) {
            return new($"{nameof(MemoryTypeError)}: Value for address '{address}' has incorrect type. Expected type is {expectedType}, found type {currentType}.");
        }
        public static ElfScriptException ExpressionIsNotAsync() {
            return new($"{nameof(ExpressionIsNotAsync)}: Cannot execute expression asynchronously, the expression is synchronous.");
        }
        public static ElfScriptException CollectionElementDoesNotExist<TIndex>(TIndex index) where TIndex : notnull {
            return new($"{nameof(CollectionElementDoesNotExist)}: Cannot delete collection element at index '{index}' because the item does not exist.");
        }
        public static ElfScriptException SweepCycleLimit() {
            return new($"{nameof(SweepCycleLimit)}: The garbage collection tried really hard but it could not clean up your mess. How deep are your objects?");
        }
        public static ElfScriptException OperationResultNullReference() {
            return new($"{nameof(OperationResultNullReference)}: Cannot dereference operation result register because no value has been set.");
        }
        public static ElfScriptException NullOperation() {
            return new($"{nameof(NullOperation)}: Cannot execute null operation.");
        }
        public static ElfScriptException IllegalExecutionBypass() {
            return new($"{nameof(IllegalExecutionBypass)}: Operations are not allowed to bypass the state machine's operation executor.");
        }
        public static ElfScriptException IllegalHandshakeDeletion() {
            return new($"{nameof(IllegalHandshakeDeletion)}: Only the state machine cthat created the handshake token can end its lifetime.");
        }
        public static ElfScriptException CannotExecuteFunctionDeclaration() {
            return new($"{nameof(CannotExecuteFunctionDeclaration)}: Cannot execute a function declaration. It must be invoked with a function operation.");
        }
    }
}
