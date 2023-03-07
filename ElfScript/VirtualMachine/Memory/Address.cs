namespace ElfScript.VirtualMachine.Memory {
    public readonly struct Address:IEquatable<Address> {
        private readonly int _value;
        public Address(int ID) => _value = ID;
        public static readonly Address Null = new(int.MinValue);

        public bool Equals(Address other) => other._value == _value;

        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(Address a,Address b) => a._value == b._value;
        public static bool operator !=(Address a,Address b) => a._value != b._value;

#pragma warning disable CS8765
        public override bool Equals(object obj) => obj is Address address && address._value == _value;
#pragma warning restore CS8765
    }
}
