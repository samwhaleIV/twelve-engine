using ElfScript.VirtualMachine.Operations;

namespace ElfScript.VirtualMachine {
    internal abstract class Operation {
        protected abstract Task Execute(StateMachine stateMachine);
        public async Task Execute(ExecutionHandshake handshake) => await Execute(handshake.StateMachine);
        public static readonly Operation Null = new NullOperation();
    }
}
