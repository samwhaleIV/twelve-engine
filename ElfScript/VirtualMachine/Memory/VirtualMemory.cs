using ElfScript.Errors;
using ElfScript.VirtualMachine.Memory;
using System.Text;

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
            if(!_references.TryGet(address,out Type referenceType)) {
                throw ErrorFactory.MemoryReferenceError(address);
            } else if(referenceType != type) {
                throw ErrorFactory.MemoryTypeError(address,type,referenceType);
            }
            return container[address];
        }

        public Value Set<T>(Address address,T variableValue,Type type,TypeContainer<T> container) {
            if(_references.TryGet(address,out Type referenceType) && referenceType != type) {
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

        public Value CreateTable() {
            Address address = _addressGenerator.GetNext();
            VirtualTable table = _virtualTablePool.Lease();
            table.Address = address;
            return Set(address,table,Type.Table,_tables);
        }

        public Value CreateList() {
            Address address = _addressGenerator.GetNext();
            VirtualList list = _virtualListPool.Lease();
            list.Address = address;
            return Set(address,list,Type.List,_lists);
        }

        public IVirtualCollection GetCollection(Address address) {
            if(!_references.TryGet(address,out Type referenceType)) {
                throw ErrorFactory.MemoryReferenceError(address);
            }
            if(referenceType == Type.List) {
                return Get(address,Type.List,_lists);
            } else {
                return Get(address,Type.Table,_tables);
            }
        }

        public string GetDiagnostics() {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"{{ Weak References: {_references.WeakReferenceCount}, Strong References: {_references.StrongReferenceCount} }}");

            return stringBuilder.ToString();
        }
    }
}
