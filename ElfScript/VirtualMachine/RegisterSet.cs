using ElfScript.IL;

namespace ElfScript.VirtualMachine {
    internal sealed class RegisterSet {
        private readonly Dictionary<RegisterCode,Register> _registers;

        public RegisterSet() => _registers = new() {
            { RegisterCode.LeftHand, new() },
            { RegisterCode.RightHand, new() },
            { RegisterCode.Result, new() },
            { RegisterCode.Conditional, new() },
            { RegisterCode.Jump, new() },
            { RegisterCode.R5, new() },
            { RegisterCode.R6, new() },
            { RegisterCode.R7, new() },
        };   

        public Register LeftHand => _registers[RegisterCode.LeftHand];
        public Register RightHand => _registers[RegisterCode.RightHand];
        public Register Result => _registers[RegisterCode.Result];
        public Register Conditional => _registers[RegisterCode.Conditional];
        public Register Jump => _registers[RegisterCode.Jump];
        public Register R5 => _registers[RegisterCode.R5];
        public Register R6 => _registers[RegisterCode.R6];
        public Register R7 => _registers[RegisterCode.R7];

        public Register Get(int register) => _registers[(RegisterCode)register];
    }
}
