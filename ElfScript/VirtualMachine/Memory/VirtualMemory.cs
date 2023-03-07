using ElfScript.Errors;
using ElfScript.VirtualMachine.Memory;
using ElfScript.VirtualMachine.Memory.Collections;

namespace ElfScript.VirtualMachine {
    internal sealed partial class VirtualMemory {

        private readonly Dictionary<Type,ITypeContainer> _typeContainers;
        public VirtualMemory() {
            _typeContainers = new() {
                { Type.String, _strings },
                { Type.Number, _numbers },
                { Type.Boolean, _booleans },
                { Type.List, _lists },
                { Type.Table, _tables },
                { Type.Function, _functions }
            };

            _virtualListPool = new(this);
            _virtualTablePool = new(this);

            _lists.OnDeleted += DeleteList;
            _tables.OnDeleted += DeleteTable;
        }

        private void DeleteTable(Address address) {
            var table = _tables[address];
            table.Reset();
            _virtualTablePool.Return(table);
        }

        private void DeleteList(Address address) {
            var list = _lists[address];
            list.Reset();
            _virtualListPool.Return(list);
        }

        private readonly AddressGenerator _addressGenerator = new();
        private readonly ReferenceStateCache _references = new();

        public void Delete(Value value) {
            if(!_references.Contains(value.Address)) {
                throw ErrorFactory.MemoryReferenceError(value.Address);
            }
            _typeContainers[value.Type].Delete(value.Address);
        }

        public Value Get(Address address) {
            if(!_references.Contains(address)) {
                throw ErrorFactory.MemoryReferenceError(address);
            }
            return _references.GetValue(address);
        }

        public T Get<T>(Address address,Type type,TypeContainer<T> container) {
            if(!_references.TryGet(address,out ValueReference reference)) {
                throw ErrorFactory.MemoryReferenceError(address);
            } else if(reference.Type != Type.String) {
                throw ErrorFactory.MemoryTypeError(address,type,reference.Type);
            }
            return container[address];
        }

        public Value Set<T>(Address address,T variableValue,Type type,TypeContainer<T> container) {
            if(_references.TryGet(address,out var reference) && reference.Type != type) {
                _typeContainers[type].Delete(address);
            }
            container[address] = variableValue;
            _references.Set(address,type);
            return new(address,type);
        }

        public Value Create<T>(T variableValue,Type type,TypeContainer<T> container) {
            var address = _addressGenerator.GetNext();
            container[address] = variableValue;
            _references.Set(address,type);
            return new(address,type);
        }

        public void Reference(Address address) {
            _references.IncreaseReferenceCounter(address);
        }

        public void Dereference(Address address) {
            _references.DecreaseReferenceCounter(address);
        }

        public void Sweep() {
            _references.SweepWeakReferences(this);
        }
    }
}
