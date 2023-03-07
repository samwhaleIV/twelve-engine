namespace ElfScript.VirtualMachine.Operations {
    internal sealed class Return:Operation {

        public Operation? GetValue { get; init; } = null;

        protected override async Task Execute(StateMachine stateMachine) {
            if(GetValue is not null) {
                await stateMachine.Execute(GetValue);
            }
            stateMachine.ShouldExitBlock = true;
        }
    }
}
