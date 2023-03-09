namespace ElfScript.VirtualMachine {
    internal readonly struct FunctionPointer {

        public FunctionPointer() {
            OpCodeIndex = -1;
        }

        public readonly int OpCodeIndex { get; init; }

        public static readonly FunctionPointer Null = new();
    }
}
