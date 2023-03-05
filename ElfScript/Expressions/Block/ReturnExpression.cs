namespace ElfScript.Expressions.Block {
    internal sealed class ReturnExpression:Expression {
        public Expression ReturnValueExpression { get; private init; }
        public ReturnExpression(Expression expression) {
            ReturnValueExpression = expression;
        }
        public override Value Evaluate(StateMachine stateMachine) {
            var returnValue = ReturnValueExpression.Evaluate(stateMachine);
            stateMachine.SetReturnRegisterValue(returnValue);
            return returnValue;
        }
    }
}
