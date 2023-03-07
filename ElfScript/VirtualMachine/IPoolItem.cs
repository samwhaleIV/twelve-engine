namespace ElfScript.VirtualMachine
{
    internal interface IPoolItem
    {
        public void SetLeaseID(int ID);
        public int GetLeaseID();
        public void Reset();
    }
}
