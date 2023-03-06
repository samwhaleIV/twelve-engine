namespace ElfScript.VirtualMachine {
    public readonly struct Address {
        public readonly ulong ID;
        public Address(ulong ID) => this.ID = ID;
        public static readonly Address Null = new(ulong.MinValue);
    }
}
