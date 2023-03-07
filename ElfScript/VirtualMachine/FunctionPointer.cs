namespace ElfScript.VirtualMachine {
    public readonly struct FunctionPointer:IEquatable<FunctionPointer> {
        private readonly int _value;
        public FunctionPointer(int ID) => _value = ID;
        public static readonly FunctionPointer Null = new(int.MinValue);

        public bool Equals(FunctionPointer other) => other._value == _value;

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(FunctionPointer a,FunctionPointer b) => a._value == b._value;
        public static bool operator !=(FunctionPointer a,FunctionPointer b) => a._value != b._value;

#pragma warning disable CS8765
        public override bool Equals(object obj) => obj is FunctionPointer FunctionPointer && FunctionPointer._value == _value;
#pragma warning restore CS8765
    }
}
