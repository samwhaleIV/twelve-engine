namespace ElfScript.VirtualMachine {
    public sealed class AddressGenerator {
        private int _address = 1;
        public Address GetNext() => new(_address += 1);
    }
}
