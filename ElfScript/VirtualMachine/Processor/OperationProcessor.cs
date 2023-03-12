using ElfScript.Errors;
using ElfScript.IL;
using ElfScript.VirtualMachine.Memory;

namespace ElfScript.VirtualMachine.Processor {
    using static OperationCodeType;

    internal sealed partial class OperationProcessor {

        public int V1 { get; private set; } = -1;
        public int V2 { get; private set; } = -1;

        public bool IsExecuting { get; private set; } = false;
        public int ProgramPointer { get; private set; } = -1;

        private RegisterSet _registers => _stateMachine.Registers;
        private OperationCode[] _program = Array.Empty<OperationCode>();
        private readonly Dictionary<OperationCodeType,Action> _operations;
        private readonly StateMachine _stateMachine;

        private readonly Dictionary<Type,Action<Register,Address>> _memoryToRegisterConverters;

        internal void SetProgramPointer(int value) {
            if(value < 0 || value >= _program.Length) {
                throw ErrorFactory.IllegalJumpDestination();
            }
            ProgramPointer = value;
        }

        private async Task OperationLoop(Func<Task>? onInterrupt = null) {
            while(ProgramPointer < _program.Length) {
                OperationCode operation = _program[ProgramPointer];
                ProgramPointer += 1;
                V1 = operation.V1;
                V2 = operation.V2;
                if(operation.Type == Flow_Interrupt) {
                    if(onInterrupt is null) {
                        continue;
                    }
                    await onInterrupt.Invoke();
                    continue;
                }
                if(!_operations.TryGetValue(operation.Type,out Action? action)) {
                    throw new NotImplementedException($"Operation code '{operation.Type}' is not implemented.");
                }
                action.Invoke();
            }
        }

        public async Task ExecuteProgram(OperationCode[] program,Func<Task>? onInterrupt = null) {
            if(IsExecuting) {
                throw new InvalidOperationException("A program is already executing.");
            }
            IsExecuting = true;
            _program = program;
            await OperationLoop(onInterrupt);
            _program = Array.Empty<OperationCode>();
            V1 = -1;
            V2 = -1;
            IsExecuting = false;
        }
    }
}
