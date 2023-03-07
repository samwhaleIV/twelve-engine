using ElfScript.VirtualMachine.Collections;
using ElfScript.VirtualMachine.Collections.Runtime;

namespace ElfScript.VirtualMachine {
    internal sealed partial class VirtualMemory {

        private readonly TypeContainer<string> _strings = new();
        private readonly TypeContainer<int> _numbers = new();
        private readonly TypeContainer<bool> _booleans = new();

        private readonly TypeContainer<VirtualList> _lists = new();
        private readonly TypeContainer<VirtualTable> _tables = new();
        private readonly TypeContainer<FunctionPointer> _functions = new();

        private readonly VirtualList.Pool _virtualListPool;
        private readonly VirtualTable.Pool _virtualTablePool;

        public void SetString(Address address,string value) => Set(address,value,Type.String,_strings);
        public void SetNumber(Address address,int value) => Set(address,value,Type.Number,_numbers);
        public void SetBoolean(Address address,bool value) => Set(address,value,Type.Boolean,_booleans);
        public void SetFunction(Address address,FunctionPointer value) => Set(address,value,Type.Function,_functions);
        public void SetTable(Address address,VirtualTable value) => Set(address,value,Type.Table,_tables);
        public void SetList(Address address,VirtualList value) => Set(address,value,Type.List,_lists);

        public string GetString(Address address) => Get(address,Type.String,_strings);
        public int GetNumber(Address address) => Get(address,Type.Number,_numbers);
        public bool GetBoolean(Address address) => Get(address,Type.Boolean,_booleans);
        public FunctionPointer GetFunction(Address address) => Get(address,Type.Function,_functions);
        public VirtualTable GetTable(Address address) => Get(address,Type.Table,_tables);
        public VirtualList GetList(Address address) => Get(address,Type.List,_lists);

        public Value CreateString(string value) => Set(_addressGenerator.GetNext(),value,Type.String,_strings);
        public Value CreateNumber(int value) => Set(_addressGenerator.GetNext(),value,Type.Number,_numbers);
        public Value CreateBoolean(bool value) => Set(_addressGenerator.GetNext(),value,Type.Boolean,_booleans);
        public Value CreateFunction(FunctionPointer value) => Set(_addressGenerator.GetNext(),value,Type.Function,_functions);
        public Value CreateTable() => Set(_addressGenerator.GetNext(),_virtualTablePool.Lease(),Type.Table,_tables);
        public Value CreateList() => Set(_addressGenerator.GetNext(),_virtualListPool.Lease(),Type.List,_lists);
    }
}
