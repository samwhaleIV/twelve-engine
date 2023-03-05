namespace ElfScript {
    internal abstract class Expression {
        public abstract Value Evaluate(StateMachine stateMachine);
    }
}
