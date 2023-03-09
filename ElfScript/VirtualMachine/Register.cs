namespace ElfScript.VirtualMachine {
    internal sealed class Register {

        public Type Type { get; private set; } = Type.Boolean;
        public bool Boolean { get; private set; } = false;
        public int Integer { get; private set; } = 0;
        public float Decimal { get; private set; } = float.NaN;
        public char Character { get; private set; } = char.MinValue;
        public string String { get; private set; } = string.Empty;
        public FunctionPointer Function { get; private set; } = FunctionPointer.Null;

        public void Set(StaticRegister staticRegister) {
            Type = staticRegister.Type;
            Boolean = staticRegister.Boolean;
            Integer = staticRegister.Integer;
            Decimal = staticRegister.Decimal;
            Character = staticRegister.Character;
            String = staticRegister.String;
        }

        public void Set(Register staticRegister) {
            Type = staticRegister.Type;
            Boolean = staticRegister.Boolean;
            Integer = staticRegister.Integer;
            Decimal = staticRegister.Decimal;
            Character = staticRegister.Character;
            String = staticRegister.String;
        }

        public void Set(bool value) {
            Type = Type.Boolean;
            Boolean = value;
        }

        public void Set(int value) {
            Type = Type.Integer;
            Integer = value;
        }

        public void Set(float value) {
            Type = Type.Decimal;
            Decimal = value;
        }

        public void Set(char value) {
            Type = Type.Character;
            Character = value;
        }

        public void Set(string value) {
            Type = Type.String;
            String = value;
        }

        public void Set(FunctionPointer value) {
            Type = Type.Function;
            Function = value;
        }
    }
}
