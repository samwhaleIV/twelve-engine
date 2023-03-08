namespace ElfScript.VirtualMachine.Memory {
    internal sealed class VirtualTable:VirtualCollection<string> {
        public VirtualTable(VirtualMemory memory) : base(memory) { }

        private readonly Dictionary<string,Value> _items = new();

        public override Value Get(string index) {
            return _items[index];
        }

        protected override void ClearItems() {
            _items.Clear();
        }

        public override IEnumerable<Value> GetItems() {
            return _items.Values;
        }

        protected override void SetValue(string index,Value value) {
            _items[index] = value;
        }

        protected override bool TryGetValue(string index,out Value value) {
            return _items.TryGetValue(index,out value);
        }

        protected override void RemoveValue(string index) {
            _items.Remove(index);
        }

        public sealed class Pool:Pool<VirtualTable> {

            private readonly VirtualMemory _memory;
            public Pool(VirtualMemory memory) => _memory = memory;

            protected override VirtualTable CreateNew() {
                return new VirtualTable(_memory);
            }

            protected override void Reset(VirtualTable item) {
                item.Reset();
            }
        }
    }
}
