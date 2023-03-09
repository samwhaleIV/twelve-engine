using ElfScript.VirtualMachine.Memory;

namespace ElfScript.VirtualMachine {
    internal sealed partial class VirtualMemory {
        private readonly TypeContainer<string> _strings = new();
        private readonly TypeContainer<int> _integers = new();
        private readonly TypeContainer<bool> _booleans = new();
        private readonly TypeContainer<float> _decimals = new();
        private readonly TypeContainer<char> _characters = new();
        private readonly TypeContainer<VirtualList> _lists = new();
        private readonly TypeContainer<VirtualTable> _tables = new();
        private readonly TypeContainer<FunctionPointer> _functions = new();

        private readonly VirtualList.Pool _virtualListPool;
        private readonly VirtualTable.Pool _virtualTablePool;

        public Value Set(Address address,string value) => Set(address,value,Type.String,_strings);
        public Value Set(Address address,int value) => Set(address,value,Type.Integer,_integers);
        public Value Set(Address address,bool value) => Set(address,value,Type.Boolean,_booleans);
        public Value Set(Address address,VirtualTable value) => Set(address,value,Type.Table,_tables);
        public Value Set(Address address,VirtualList value) => Set(address,value,Type.List,_lists);
        public Value Set(Address address,float value) => Set(address,value,Type.Decimal,_decimals);
        public Value Set(Address address,char value) => Set(address,value,Type.Character,_characters);
        public Value Set(Address address,FunctionPointer value) => Set(address,value,Type.Function,_functions);

        public string GetString(Address address) => Get(address,Type.String,_strings);
        public int GetInteger(Address address) => Get(address,Type.Integer,_integers);
        public bool GetBoolean(Address address) => Get(address,Type.Boolean,_booleans);
        public char GetCharacter(Address address) => Get(address,Type.Character,_characters);
        public float GetDecimal(Address address) => Get(address,Type.Decimal,_decimals);
        public VirtualTable GetTable(Address address) => Get(address,Type.Table,_tables);
        public VirtualList GetList(Address address) => Get(address,Type.List,_lists);
        public FunctionPointer GetFunction(Address address) => Get(address,Type.Function,_functions);

        public Value Create(string value) => Set(_addressGenerator.GetNext(),value,Type.String,_strings);
        public Value Create(int value) => Set(_addressGenerator.GetNext(),value,Type.Integer,_integers);
        public Value Create(bool value) => Set(_addressGenerator.GetNext(),value,Type.Boolean,_booleans);
        public Value Create(float value) => Set(_addressGenerator.GetNext(),value,Type.Decimal,_decimals);
        public Value Create(char value) => Set(_addressGenerator.GetNext(),value,Type.Character,_characters);
        public Value Create(FunctionPointer value) => Set(_addressGenerator.GetNext(),value,Type.Function,_functions);

        public Value Create(StaticRegister value) => value.Type switch {
            Type.Boolean => Create(value.Boolean),
            Type.Integer => Create(value.Integer),
            Type.String => Create(value.String),
            Type.Function => Create(value.Function),
            Type.Character => Create(value.Character),
            Type.Decimal => Create(value.Decimal),
            _ => throw new NotImplementedException()
        };

        public Value Set(Address address,StaticRegister value) => value.Type switch {
            Type.Boolean => Set(address,value.Boolean),
            Type.Integer => Set(address,value.Integer),
            Type.String => Set(address,value.String),
            Type.Function => Set(address,value.Function),
            Type.Character => Set(address,value.Character),
            Type.Decimal => Set(address,value.Decimal),
            _ => throw new NotImplementedException()
        };

        public Value Create(Register value) => value.Type switch {
            Type.Boolean => Create(value.Boolean),
            Type.Integer => Create(value.Integer),
            Type.String => Create(value.String),
            Type.Function => Create(value.Function),
            Type.Character => Create(value.Character),
            Type.Decimal => Create(value.Decimal),
            _ => throw new NotImplementedException()
        };

        public Value Set(Address address,Register value) => value.Type switch {
            Type.Boolean => Set(address,value.Boolean),
            Type.Integer => Set(address,value.Integer),
            Type.String => Set(address,value.String),
            Type.Function => Set(address,value.Function),
            Type.Character => Set(address,value.Character),
            Type.Decimal => Set(address,value.Decimal),
            _ => throw new NotImplementedException()
        };
    }
}
