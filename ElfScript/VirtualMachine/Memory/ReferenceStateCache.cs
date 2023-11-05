using ElfScript.Errors;

namespace ElfScript.VirtualMachine.Memory {
    internal sealed class ReferenceStateCache {

        private readonly HashSet<Address> _strongReferences = new(), _weakReferences = new();
        private readonly Dictionary<Address,ValueReference> _referenceTable = new();

        private readonly Stack<Value> _deletionBuffer = new();

        public void SweepWeakReferences(VirtualMemory memory) {
            while(_weakReferences.Count > 0) {
                foreach(Address address in _weakReferences) {
                    var type = _referenceTable[address].Type;
                    _deletionBuffer.Push(new(address,type));
                }
                while(_deletionBuffer.TryPop(out Value value)) {
                    memory.Delete(value);
                    _weakReferences.Remove(value.Address);
                }
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

        public bool TryGet(Address address,out Type referenceType) {
            var result = _referenceTable.TryGetValue(address,out ValueReference reference);
            referenceType = reference.Type;
            return result;
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
            _weakReferences.Add(address);
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
            if(!TryGet(address,out Type referenceType)) {
                throw ErrorFactory.MemoryReferenceError(address);
            }
            return new(address,referenceType);
        }

        public int StrongReferenceCount => _strongReferences.Count;
        public int WeakReferenceCount => _weakReferences.Count; 
    }
}
