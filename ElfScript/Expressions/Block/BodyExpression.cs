namespace ElfScript.Expressions.Block {
    internal class BodyExpression:Expression {

        public Expression[] Expressions { get; private init; }

        public BodyExpression(Expression[] expressions) {
            Expressions = expressions;
            AsyncEnabled = true;
        }

        public override Value Evaluate(StateMachine stateMachine) {
            stateMachine.CreateStackFrame();
            foreach(var expression in Expressions) {
                expression.Evaluate(stateMachine);
            }
            stateMachine.PopStackFrame();
            return stateMachine.GetReturnRegisterValue();
        }

        public override async Task<Value> EvaluateAsync(StateMachine stateMachine) {
            stateMachine.CreateStackFrame();
            foreach(var expression in Expressions) {
                if(expression.AsyncEnabled) {
                    await expression.EvaluateAsync(stateMachine);
                    continue;
                }
                expression.Evaluate(stateMachine);
            }
            stateMachine.PopStackFrame();
            return stateMachine.GetReturnRegisterValue();
        }
    }
}
