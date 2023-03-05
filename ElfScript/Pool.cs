namespace ElfScript {

    internal abstract class Pool<T> {

        private readonly Dictionary<int,T> _active = new();
        private readonly Stack<T> _inactive = new();

        private int _IDCounter = 0;

        protected abstract void Reset(T item);
        protected internal abstract T CreateNew();

        public int Lease(out T item) {
            if(_IDCounter == int.MaxValue) {
                _IDCounter = int.MinValue;
            }
            int ID = _IDCounter++;
            if(_inactive.Count <= 0) {
                item = CreateNew();
            } else {
                item = _inactive.Pop();
            }
            _active[ID] = item;
            return ID;
        }

        public void Return(int ID) {
            if(!_active.TryGetValue(ID,out T? value)) {
                throw new InvalidOperationException($"ID '{ID}' not contained in active pool.");
            }
            Reset(value);
            _active.Remove(ID);
            _inactive.Push(value);
        }
    }
}
