using ElfScript.Errors;

namespace ElfScript.VirtualMachine.Operations {
    internal sealed class ExecuteFunction:Operation {

        public Operation GetFunction { get; init; } = Null;
        public Operation[] Arguments { get; init; } = Array.Empty<Operation>();

        protected override async Task Execute(StateMachine stateMachine) {
            await stateMachine.Execute(GetFunction);
            Value value = stateMachine.ValueRegister;
            if(value.Type != Type.Function) {
                throw ErrorFactory.ValueTypeError(Type.Function);
            }
            FunctionDeclaration function = stateMachine.Memory.GetFunction(value.Address);
            await stateMachine.Execute(function);
        }
    }
}
