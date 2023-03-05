namespace ElfScript.Expressions.Variable {
    internal sealed class SetVariableExpression:Expression {

        public string VariableName { get; private init; }
        public Expression ValueExpression { get; private init; }

        public SetVariableExpression(string variableName,Expression valueExpression) {
            VariableName = variableName;
            ValueExpression = valueExpression;
        }

        public override Value Evaluate(StateMachine stateMachine) {
            var value = ValueExpression.Evaluate(stateMachine);
            stateMachine.SetValue(VariableName,value.ID);
            return value;
        }
    }
}
