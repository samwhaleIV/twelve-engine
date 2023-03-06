using ElfScript.Errors;

namespace ElfScript.VirtualMachine.Collections {
    public sealed class TypeContainer<T>:Dictionary<Address,T>, ITypeContainer {
        public event Action<T>? OnDeleted;
        public void Delete(Address address) {
            if(!TryGetValue(address,out T? value)) {
                throw ErrorFactory.InternalMemoryReferenceError(address);
            }
            Remove(address);
            OnDeleted?.Invoke(value);
        }
    }
}
