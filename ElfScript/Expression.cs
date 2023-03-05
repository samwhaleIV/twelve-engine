using ElfScript.Errors;

namespace ElfScript {
    internal abstract class Expression {
        public abstract Value Evaluate(StateMachine stateMachine);

        public bool AsyncEnabled { get; protected init; }

        public virtual Task<Value> EvaluateAsync(StateMachine stateMachine) {
            if(!AsyncEnabled) {
                throw ErrorFactory.ExpressionIsNotAsync();
            } else {
                throw ErrorFactory.AsyncNotImplemented();
            }
        }
    }
}
