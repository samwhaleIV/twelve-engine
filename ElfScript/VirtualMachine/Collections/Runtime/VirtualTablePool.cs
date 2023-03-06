namespace ElfScript.VirtualMachine.Collections.Runtime {
    internal sealed class VirtualTablePool:Pool<VirtualTable> {
        protected override void Reset(VirtualTable item) {
            item.Clear();
        }
        protected internal override VirtualTable CreateNew() {
            return new VirtualTable();
        }
    }
}
