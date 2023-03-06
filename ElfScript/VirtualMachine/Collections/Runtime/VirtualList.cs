﻿namespace ElfScript.VirtualMachine.Collections.Runtime {
    internal sealed class VirtualList:List<Value>, IPoolItem {
        private int _leaseID;
        public int GetLeaseID() => _leaseID;
        public void SetLeaseID(int ID) => _leaseID = ID;
    }
}
