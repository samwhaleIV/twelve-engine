using ElfScript.Errors;
using ElfScript.Expressions.Block;

namespace ElfScript {
    internal sealed class VirtualMemory {

        /* All saved data goes through virtual memory. Dangling expression values come through here, but unless they are pinned, they are deleted on CleanUp() */

        private Dictionary<int,string> Strings { get; init; } = new();
        private Dictionary<int,int> Numbers { get; init; } = new();
        private Dictionary<int,bool> Booleans { get; init; } = new();

        private Dictionary<int,List<Value>> Lists { get; init; } = new();
        private Dictionary<int,Dictionary<string,Value>> Tables { get; init; } = new();

        private Dictionary<int,FunctionExpression> Functions { get; init; } = new();

        private readonly Dictionary<int,Value> _values = new();
        private readonly HashSet<int> _pinnedValues = new();

        private int _IDCounter = Value.None.ID + 1;

        private int GetID() {
            return _IDCounter++;
        }

        public void CleanUp() {
            foreach(var ID in _values.Keys) {
                if(_pinnedValues.Contains(ID)) {
                    continue;
                }
                Delete(ID);
            }
        }

        public void AddPin(int ID) {
            _pinnedValues.Add(ID);
        }

        public void RemovePin(int ID) {
            _pinnedValues.Remove(ID);
        }

        public string GetString(int ID) {
            if(!_values.TryGetValue(ID,out var value)) {
                throw ErrorFactory.InternalMemoryReferenceError(ID,Type.String);
            } else if(value.Type != Type.String) {
                throw ErrorFactory.InternalMemoryTypeError(ID,Type.String,value.Type);
            }
            return Strings[ID];
        }

        public int GetNumber(int ID) {
            if(!_values.TryGetValue(ID,out var value)) {
                throw ErrorFactory.InternalMemoryReferenceError(ID,Type.Number);
            } else if(value.Type != Type.Number) {
                throw ErrorFactory.InternalMemoryTypeError(ID,Type.Number,value.Type);
            }
            return Numbers[ID];
        }

        public bool GetBoolean(int ID) {
            if(!_values.TryGetValue(ID,out var value)) {
                throw ErrorFactory.InternalMemoryReferenceError(ID,Type.Boolean);
            } else if(value.Type != Type.Boolean) {
                throw ErrorFactory.InternalMemoryTypeError(ID,Type.Boolean,value.Type);
            }
            return Booleans[ID];
        }

        public List<Value> GetList(int ID) {
            if(!_values.TryGetValue(ID,out var value)) {
                throw ErrorFactory.InternalMemoryReferenceError(ID,Type.List);
            } else if(value.Type != Type.List) {
                throw ErrorFactory.InternalMemoryTypeError(ID,Type.List,value.Type);
            }
            return Lists[ID];
        }

        public Dictionary<string,Value> GetTable(int ID) {
            if(!_values.TryGetValue(ID,out var value)) {
                throw ErrorFactory.InternalMemoryReferenceError(ID,Type.Table);
            } else if(value.Type != Type.Table) {
                throw ErrorFactory.InternalMemoryTypeError(ID,Type.Table,value.Type);
            }
            return Tables[ID];
        }

        public FunctionExpression GetFunction(int ID) {
            if(!_values.TryGetValue(ID,out var value)) {
                throw ErrorFactory.InternalMemoryReferenceError(ID,Type.Function);
            } else if(value.Type != Type.Function) {
                throw ErrorFactory.InternalMemoryTypeError(ID,Type.Function,value.Type);
            }
            return Functions[ID];
        }

        public void SetString(int ID,string value) {
            Delete(ID);
            Strings[ID] = value;
            _values[ID] = new Value(ID,Type.String);
        }

        public void SetNumber(int ID,int value) {
            Delete(ID);
            Numbers[ID] = value;
            _values[ID] = new Value(ID,Type.Number);
        }

        public void SetBoolean(int ID) {
            Delete(ID);
            Booleans[ID] = true;
        }

        public void SetList(int ID,List<Value> list) {
            //todo... cleanup/reuse tables with pools
            Delete(ID);
            Lists[ID] = list;
            _values[ID] = new Value(ID,Type.List);
        }

        public void SetTable(int ID,Dictionary<string,Value> table) {
            //todo... cleanup/reuse tables with pools
            Delete(ID);
            Tables[ID] = table;
            _values[ID] = new Value(ID,Type.Table);
        }

        public void SetFunction(int ID,FunctionExpression functionExpression) {
            Delete(ID);
            Functions[ID] = functionExpression;
            _values[ID] = new Value(ID,Type.Function);
        }

        public Value Get(int ID) {
            return _values[ID];
        }

        public void Delete(int ID) {
            if(!_values.TryGetValue(ID,out var value)) {
                return;
            }
            if(_pinnedValues.Contains(ID)) {
                throw ErrorFactory.IllegalDeletionOfPinnedValue();
            }
            switch(value.Type) {
                case Type.Boolean:
                    Booleans.Remove(ID);
                    break;
                case Type.Number:
                    Numbers.Remove(ID);
                    break;
                case Type.String:
                    Strings.Remove(ID);
                    break;
                case Type.Table:
                    Tables.Remove(ID);
                    //todo... cleanup/reuse tables with pools
                    break;
                case Type.List:
                    Lists.Remove(ID);
                    //todo... cleanup/reuse lists with pools
                    break;
                case Type.Function:
                    break;
            }
            _values.Remove(ID);
        }

        public Value CreateString(string stringValue) {
            var ID = GetID();
            Strings[ID] = stringValue;
            var value = new Value(ID,Type.String);
            _values[ID] = value;
            return value;
        }

        public Value CreateNumber(int numberValue) {
            var ID = GetID();
            Numbers[ID] = numberValue;
            var value = new Value(ID,Type.Number);
            _values[ID] = value;
            return value;
        }

        public Value CreateBoolean(bool booleanValue) {
            var ID = GetID();
            Booleans[ID] = booleanValue;
            var value = new Value(ID,Type.Boolean);
            _values[ID] = value;
            return value;
        }

        public Value CreateList(List<Value> list) {
            //todo... cleanup/reuse tables with pools
            var ID = GetID();
            Lists[ID] = list;
            var value = new Value(ID,Type.List);
            _values[ID] = value;
            return value;
        }

        public Value CreateTable(Dictionary<string,Value> table) {
            //todo... cleanup/reuse tables with pools
            var ID = GetID();
            Tables[ID] = table;
            var value = new Value(ID,Type.Table);
            _values[ID] = value;
            return value;
        }

        public Value CreateFunction(FunctionExpression functionExpression) {
            var ID = GetID();
            Functions[ID] = functionExpression;
            var value = new Value(ID,Type.Function);
            _values[ID] = value;
            return value;
        }
    }
}
