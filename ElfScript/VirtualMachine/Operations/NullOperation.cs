using ElfScript.Errors;

namespace ElfScript.VirtualMachine.Operations {
    internal sealed class NullOperation:Operation {
        protected override Task<Value> Execute(StateMachine stateMachine) {
            throw ErrorFactory.NullOperation();
        }
    }
}
