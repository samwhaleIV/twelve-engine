namespace ElfScript.VirtualMachine {
    internal struct ValueReference {
        public readonly Type Type { get; private init; }
        public int ReferenceCount { get; set; }
        public ValueReference(Type type) { Type = type; ReferenceCount = 0; }
    }
}
