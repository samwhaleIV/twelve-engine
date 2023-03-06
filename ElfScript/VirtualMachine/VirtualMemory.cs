using ElfScript.Errors;
using ElfScript.Expressions.Block;
using ElfScript.VirtualMachine.Collections;
using ElfScript.VirtualMachine.Collections.Runtime;

namespace ElfScript.VirtualMachine
{
    internal sealed partial class VirtualMemory {

        /* All saved data goes through virtual memory. Dangling expression values come through here, but unless they are pinned, they are deleted on CleanUp() */
        private readonly TypeContainer<string> _strings = new();
        private readonly TypeContainer<int> _numbers = new();
        private readonly TypeContainer<bool> _booleans = new();

        private readonly TypeContainer<VirtualList> _lists = new();
        private readonly TypeContainer<VirtualTable> _tables = new();
        private readonly TypeContainer<FunctionExpression> _functions = new();

        private readonly Dictionary<Type,ITypeContainer> _typeContainers;

        public VirtualMemory() => _typeContainers = new() {
            { Type.String, _strings }, { Type.Number, _numbers }, { Type.Boolean, _booleans },
            { Type.List, _lists }, { Type.Table, _tables }, { Type.Function, _functions }
        };

        private readonly Dictionary<Address,Value> _values = new();
        private readonly HashSet<Address> _pinnedValues = new();

        private readonly AddressGenerator _addressGenerator = new();

        private readonly VirtualListPool _virtualListPool = new();
        private readonly VirtualTablePool _virtualTablePool = new();

        public void CleanUp() {
            foreach(Address address in _values.Keys) {
                if(_pinnedValues.Contains(address)) {
                    continue;
                }
                Delete(address);
            }
        }

        public void AddPin(Address address) => _pinnedValues.Add(address);
        public void RemovePin(Address address) => _pinnedValues.Remove(address);

        public Value Get(Address address) {
            if(!_values.ContainsKey(address)) {
                throw ErrorFactory.InternalMemoryReferenceError(address);
            }
            return _values[address];
        }

        public T Get<T>(Address address,Type type,TypeContainer<T> container) {
            if(!_values.TryGetValue(address,out Value value)) {
                throw ErrorFactory.InternalMemoryReferenceError(address);
            } else if(value.Type != Type.String) {
                throw ErrorFactory.InternalMemoryTypeError(address,type,value.Type);
            }
            return container[address];
        }

        public Value Set<T>(Address address,T variableValue,Type type,TypeContainer<T> container) {
            if(_values.ContainsKey(address)) {
                Delete(address);
            }
            container[address] = variableValue;
            var value = new Value(address,type);
            _values[address] = value;
            return value;
        }

        public Value Create<T>(T variableValue,Type type,TypeContainer<T> container) {
            var address = _addressGenerator.GetNext();
            container[address] = variableValue;
            Value value = new(address,type);
            _values[address] = value;
            return value;
        }

        public void Delete(Address address) {
            if(!_values.ContainsKey(address)) {
                throw ErrorFactory.InternalMemoryReferenceError(address);
            }
            if(_pinnedValues.Contains(address)) {
                throw ErrorFactory.IllegalDeletionOfPinnedValue();
            }
            _typeContainers[_values[address].Type].Delete(address);
            _values.Remove(address);
        }
    }
}
