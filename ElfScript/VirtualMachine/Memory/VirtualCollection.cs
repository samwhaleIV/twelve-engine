using ElfScript.Errors;

namespace ElfScript.VirtualMachine.Memory {
    internal abstract class VirtualCollection<TIndex>:IPoolItem where TIndex : notnull {

        private readonly VirtualMemory _memory;
        public VirtualCollection(VirtualMemory memory) => _memory = memory;

        private int _leaseID;
        public int GetLeaseID() => _leaseID;
        public void SetLeaseID(int ID) => _leaseID = ID;

        protected abstract void SetValue(TIndex index,Value value);
        protected abstract bool TryGetValue(TIndex index,out Value value);
        protected abstract void RemoveValue(TIndex index);

        public abstract Value Get(TIndex index);

        public void Set(TIndex index,Value value) {
            if(TryGetValue(index,out var oldValue) && oldValue.Address != value.Address) {
                _memory.Dereference(oldValue.Address);
            }
            SetValue(index,value);
            _memory.Reference(value.Address);
        }

        public void Remove(TIndex index) {
            if(!TryGetValue(index,out var oldValue)) {
                throw ErrorFactory.CollectionElementDoesNotExist(index);
            }
            RemoveValue(index);
            _memory.Dereference(oldValue.Address);
        }

        protected abstract IEnumerable<Value> GetItems();
        protected abstract void ClearItems();

        public void Reset() {
            foreach(var item in GetItems()) {
                _memory.Dereference(item.Address);
            }
            ClearItems();
        }
    }
}
