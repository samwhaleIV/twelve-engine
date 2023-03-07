namespace ElfScript.VirtualMachine.Collections {
    public sealed class TypeContainer<T>:Dictionary<Address,T>,ITypeContainer {
        public event Action<Address>? OnDeleted;
        public void Delete(Address address) {
            OnDeleted?.Invoke(address);
            Remove(address);
        }
    }
}
