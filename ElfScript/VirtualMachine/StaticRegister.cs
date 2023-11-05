namespace ElfScript.VirtualMachine {
    internal readonly struct StaticRegister {

        public Type Type { get; private init; } = Type.Boolean;
        public bool Boolean { get; private init; } = false;
        public int Integer { get; private init; } = 0;
        public float Decimal { get; private init; } = float.NaN;
        public char Character { get; private init; } = char.MinValue;
        public string String { get; private init; } = string.Empty;
        public FunctionPointer Function { get; private init; } = FunctionPointer.Null;

        public StaticRegister(bool value) {
            Type = Type.Boolean;
            Boolean = value;
        }

        public StaticRegister(int value) {
            Type = Type.Integer;
            Integer = value;
        }

        public StaticRegister(float value) {
            Type = Type.Decimal;
            Decimal = value;
        }

        public StaticRegister(char value) {
            Type = Type.Character;
            Character = value;
        }

        public StaticRegister(string value) {
            Type = Type.String;
            String = value;
        }

        public StaticRegister(FunctionPointer value) {
            Type = Type.Function;
            Function = value;
        }
    }
}
