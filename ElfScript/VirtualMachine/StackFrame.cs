using ElfScript.VirtualMachine.Memory;

namespace ElfScript.VirtualMachine {
    internal sealed class StackFrame:Dictionary<int,Address>, IPoolItem {
        private int _leaseID;
        public int GetLeaseID() => _leaseID;
        public void Reset() => Clear();
        public void SetLeaseID(int ID) => _leaseID = ID;
        public int ReturnIndex { get; set; } = -1;
    }
}
