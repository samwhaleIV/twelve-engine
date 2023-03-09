namespace ElfScript.VirtualMachine {
    internal readonly struct FunctionPointer {

        public FunctionPointer() {
            Value = -1;
        }

        public readonly int Value { get; init; }

        public static readonly FunctionPointer Null = new();
    }
}
