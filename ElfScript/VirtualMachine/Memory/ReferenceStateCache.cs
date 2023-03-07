using ElfScript.Errors;

namespace ElfScript.VirtualMachine.Memory {
    internal sealed class ReferenceStateCache {

        private readonly HashSet<Address> _strongReferences = new(), _weakReferences = new();
        private readonly Dictionary<Address,ValueReference> _referenceTable = new();

        private readonly Stack<Value> _collectionDeletionBuffer = new(), _deletionBuffer = new();

        private static bool IsCollection(Type type) => type == Type.Table || type == Type.List;

        public void SweepWeakReferences(VirtualMemory memory) {
            foreach(Address address in _weakReferences) {
                ValueReference reference = _referenceTable[address];
                if(IsCollection(reference.Type)) {
                    _collectionDeletionBuffer.Push(new(address,reference.Type));
                    continue;
                }
                _deletionBuffer.Push(new(address,reference.Type));
            }
            while(_deletionBuffer.TryPop(out var reference)) {
                while(_collectionDeletionBuffer.TryPop(out var collectionReference)) {
                    memory.Delete(collectionReference);
                }
                memory.Delete(reference);
            }
        }

        private void UpgradeReference(Address address) {
            /* Reference count going from 0 -> 1 */
            _weakReferences.Remove(address);
            _strongReferences.Add(address);
        }

        private void DowngradeReference(Address address) {
            /* Reference count going from 1 -> 0 */
            _strongReferences.Remove(address);
            _weakReferences.Add(address);
        }

        public bool Contains(Address address) {
            return _referenceTable.ContainsKey(address);
        }

        public bool TryGet(Address address,out ValueReference reference) {
            return _referenceTable.TryGetValue(address,out reference);
        }

        public void Set(Address address,Type type) {
            int referenceCount = 0;
            if(_referenceTable.TryGetValue(address,out ValueReference reference)) {
                referenceCount = reference.ReferenceCount;
            }
            reference = new(type) {
                ReferenceCount = referenceCount
            };
            _referenceTable[address] = reference;
        }

        public void IncreaseReferenceCounter(Address address) {
            ValueReference reference = _referenceTable[address];
            if(reference.ReferenceCount == 0) {
                UpgradeReference(address);
            }
            reference.ReferenceCount += 1;
            _referenceTable[address] = reference;
        }

        public void DecreaseReferenceCounter(Address address) {
            ValueReference reference = _referenceTable[address];
            if(reference.ReferenceCount == 1) {
                DowngradeReference(address);
            }
            reference.ReferenceCount -= 1;
            _referenceTable[address] = reference;
        }

        public Value GetValue(Address address) {
            if(!TryGet(address,out ValueReference reference)) {
                throw ErrorFactory.MemoryReferenceError(address);
            }
            return new(address,reference.Type);
        }
    }
}
