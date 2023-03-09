using ElfScript.VirtualMachine.Memory;

namespace ElfScript.Errors {
    internal static class ErrorFactory {
        public static ElfScriptException VariableReferenceError(int variableID) {
            return new($"{nameof(VariableReferenceError)}: '{variableID}' was not found.");
        }
        public static ElfScriptException StackUnderflow() {
            return new($"{nameof(StackUnderflow)}: Cannot pop stack frame from top level.");
        }
        public static ElfScriptException MemoryReferenceError(Address address) {
            return new($"{nameof(MemoryReferenceError)}: Value for address '{address}' not found.");
        }
        public static ElfScriptException MemoryTypeError(Address address,Type expectedType,Type currentType) {
            return new($"{nameof(MemoryTypeError)}: Value for address '{address}' has incorrect type. Expected type is {expectedType}, found type {currentType}.");
        }
        public static ElfScriptException CollectionElementDoesNotExist<TIndex>(TIndex index) where TIndex : notnull {
            return new($"{nameof(CollectionElementDoesNotExist)}: Cannot delete collection element at index '{index}' because the item does not exist.");
        }
        public static ElfScriptException CircularReferenceInCollection() {
            return new($"{nameof(CircularReferenceInCollection)}: Circular reference detected in collection layout.");
        }
        public static ElfScriptException IncorrectJumpRegisterType() {
            return new($"{nameof(IncorrectJumpRegisterType)}: The jump register does not contain a valid function pointer.");
        }
        public static ElfScriptException IllegalJumpDestination() {
            return new($"{nameof(IllegalJumpDestination)}: Illegal jump destination.");
        }
    }
}
