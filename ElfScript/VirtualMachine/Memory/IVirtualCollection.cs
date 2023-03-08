namespace ElfScript.VirtualMachine.Memory {
    internal interface IVirtualCollection {
        public IEnumerable<Address> GetCollectionItems();
    }
}
