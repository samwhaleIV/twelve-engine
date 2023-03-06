namespace ElfScript.VirtualMachine.Collections.Runtime {
    internal sealed class VirtualListPool:Pool<VirtualList> {
        protected override void Reset(VirtualList item) {
            item.Clear();
        }
        protected internal override VirtualList CreateNew() {
            return new VirtualList();
        }
    }
}
