using ElfScript.Errors;

namespace ElfScript.Expressions.Variable.Dynamic {
    internal sealed class OperatorExpression:Expression {

        public Expression ExpressionA { get; private init; }
        public Expression ExpressionB { get; private init; }
        public Operator Operator { get; private init; }

        public OperatorExpression(Expression expressionA,Expression expressionB,Operator operatorType) {
            ExpressionA = expressionA;
            ExpressionB = expressionB;
            Operator = operatorType;
        }

        public override Value Evaluate(StateMachine stateMachine) {
            var valueA = ExpressionA.Evaluate(stateMachine);
            var valueB = ExpressionB.Evaluate(stateMachine);
            if(!(valueA.Type == valueB.Type && Operator == Operator.Add)) {
                throw ErrorFactory.OperatorNotImplemented(valueA.Type,valueB.Type,Operator);
            }
            int a = stateMachine.Memory.GetNumber(valueA.ID), b = stateMachine.Memory.GetNumber(valueB.ID);
            return stateMachine.Memory.CreateNumber(a + b);
        }
    }
}
