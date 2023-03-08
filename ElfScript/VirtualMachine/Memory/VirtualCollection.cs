using ElfScript.Errors;

namespace ElfScript.VirtualMachine.Memory {
    internal abstract class VirtualCollection<TIndex>:IPoolItem,IVirtualCollection where TIndex : notnull {

        private readonly VirtualMemory _memory;
        public VirtualCollection(VirtualMemory memory) => _memory = memory;

        private int _leaseID;
        public int GetLeaseID() => _leaseID;
        public void SetLeaseID(int ID) => _leaseID = ID;

        public Address Address { get; set; } = Address.Null;

        protected abstract void SetValue(TIndex index,Value value);
        protected abstract bool TryGetValue(TIndex index,out Value value);
        protected abstract void RemoveValue(TIndex index);

        public abstract Value Get(TIndex index);

        private static bool IsCollection(Type type) {
            return type == Type.List || type == Type.Table;
        }

        private readonly Stack<Address> _referenceCycleStack = new();

        private bool CreatesCircularReference(Address childCollection) {
            _referenceCycleStack.Push(childCollection);
            bool circularReferenceDetected = false;
            Address selfAddress = Address;
            while(_referenceCycleStack.TryPop(out Address address)) {
                if(address == selfAddress) {
                    circularReferenceDetected = true;
                    break;
                }
                IVirtualCollection collection = _memory.GetCollection(address);
                foreach(Address childAddress in collection.GetCollectionItems()) {
                    _referenceCycleStack.Push(childAddress);
                }
            }
            _referenceCycleStack.Clear();
            return circularReferenceDetected;
        }

        private readonly HashSet<Address> _collectionAddresses = new();

        public void Set(TIndex index,Value value) {
            Type type = value.Type;
            Address address = value.Address;
            if(TryGetValue(index,out var oldValue) && oldValue.Address != value.Address) {
                _memory.Dereference(oldValue.Address);
            }
            SetValue(index,value);
            if(IsCollection(type)) {
                _collectionAddresses.Add(address);
            }
            if(IsCollection(type) && CreatesCircularReference(value.Address)) {
                /* If this error is ignored, immortal garbage generation can occur, but normal program flow can resume normally. */
                throw ErrorFactory.CircularReferenceInCollection();
            }
            _memory.Reference(address);
        }

        public void Remove(TIndex index) {
            if(!TryGetValue(index,out var oldValue)) {
                throw ErrorFactory.CollectionElementDoesNotExist(index);
            }
            RemoveValue(index);
            if(IsCollection(oldValue.Type)) {
                _collectionAddresses.Remove(oldValue.Address);
            }
            _memory.Dereference(oldValue.Address);
        }

        public abstract IEnumerable<Value> GetItems();
        protected abstract void ClearItems();

        public void Reset() {
            foreach(var item in GetItems()) {
                _memory.Dereference(item.Address);
            }
            _collectionAddresses.Clear();
            Address = Address.Null;
            ClearItems();
        }

        public IEnumerable<Address> GetCollectionItems() {
            return _collectionAddresses;
        }
    }
}
