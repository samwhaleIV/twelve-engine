namespace ElfScript.VirtualMachine.Collections.Runtime {
    internal sealed class VirtualList:VirtualCollection<int> {
        public VirtualList(VirtualMemory memory) : base(memory) { }

        private readonly List<Value> _items = new();

        public override Value Get(int index) {
            return _items[index];
        }

        protected override void ClearItems() {
            _items.Clear();
        }

        protected override IEnumerable<Value> GetItems() {
            return _items;
        }

        protected override void SetValue(int index,Value value) {
            _items[index] = value;
        }

        protected override bool TryGetValue(int index,out Value value) {
            var oldValue = _items[index];
            if(oldValue.Address == Address.Null) {
                value = default;
                return false;
            }
            value = oldValue;
            return true;
        }

        protected override void RemoveValue(int index) {
            _items[index] = default;
        }

        public sealed class Pool:Pool<VirtualList> {

            private readonly VirtualMemory _memory;
            public Pool(VirtualMemory memory) => _memory = memory;

            protected override VirtualList CreateNew() {
                return new VirtualList(_memory);
            }

            protected override void Reset(VirtualList item) {
                item.Reset();
            }
        }
    }
}
