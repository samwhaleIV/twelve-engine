namespace ElfScript {
    internal abstract class Pool<T> where T:IPoolItem {

        private readonly Dictionary<int,T> _active = new();
        private readonly Stack<T> _inactive = new();

        private int _IDCounter = 0;

        protected abstract void Reset(T item);
        protected internal abstract T CreateNew();

        public T Lease() {
            if(_IDCounter == int.MaxValue) {
                _IDCounter = int.MinValue;
            }
            int ID = _IDCounter++;
            T item = _inactive.Count < 1 ? CreateNew() : _inactive.Pop();
            _active[ID] = item;
            item.SetLeaseID(ID);
            return item;
        }

        public void Return(IPoolItem poolItem) {
            int leaseID = poolItem.GetLeaseID();
            if(!_active.TryGetValue(leaseID,out T? value)) {
                throw new InvalidOperationException($"ID '{leaseID}' not contained in active pool.");
            }
            Reset(value);
            _active.Remove(leaseID);
            _inactive.Push(value);
        }
    }
}
