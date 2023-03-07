namespace ElfScript.VirtualMachine.Operations {
    internal sealed class Block:Operation {

        public Operation[] Operations { get; init; } = Array.Empty<Operation>();

        protected override async Task Execute(StateMachine stateMachine) {
            stateMachine.CreateStackFrame();
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
