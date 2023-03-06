namespace ElfScript.VirtualMachine {
    public sealed class AddressGenerator {
        private ulong _address = Address.Null.ID + 1;
        public Address GetNext() => new(_address++);
    }
}
