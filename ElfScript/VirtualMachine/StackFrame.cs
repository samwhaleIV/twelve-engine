namespace ElfScript.VirtualMachine {
    internal sealed class StackFrame:Dictionary<string,Address>,IPoolItem {
        private int _leaseID;
        public int GetLeaseID() => _leaseID;
        public void Reset() => Clear();
        public void SetLeaseID(int ID) => _leaseID = ID;
    }
}
