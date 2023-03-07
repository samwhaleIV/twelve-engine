using ElfScript.Errors;

namespace ElfScript.VirtualMachine.Operations {
    internal sealed class FunctionDeclaration:Operation {

        public string[] Parameters { get; init; } = Array.Empty<string>();
        public Operation[] Operations { get; init; } = Array.Empty<Operation>();

        protected override Task Execute(StateMachine stateMachine) {
            throw ErrorFactory.CannotExecuteFunctionDeclaration();
        }

        public async Task Invoke(StateMachine stateMachine,Operation[] arguments) {
            stateMachine.CreateStackFrame();
            if(arguments.Length != Parameters.Length) {
                throw ErrorFactory.FunctionParameterMismatch(Parameters.Length,arguments.Length);
            }
            for(int i = 0;i<arguments.Length;i++) {
                await stateMachine.Execute(arguments[i]);
                stateMachine.SetVariable(Parameters[i],stateMachine.ValueRegister.Address);
            }
            foreach(var operation in Operations) {
                await stateMachine.Execute(operation);
                if(stateMachine.ShouldExitBlock) {
                    stateMachine.PopStackFrame();
                    return;
                }
            }
            stateMachine.PopStackFrame();
        }
    }
}
