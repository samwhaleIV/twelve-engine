using ElfScript.IR;

namespace ElfScript.Expressions.Block {
    internal class BodyExpression:Expression {

        public Expression[] Expressions { get; private init; }

        public BodyExpression(Expression[] expressions) {
            Expressions = expressions;
        }

        public override Value Evaluate(StateMachine stateMachine) {
            stateMachine.CreateStackFrame();
            foreach(var expression in Expressions) {
                expression.Evaluate(stateMachine);
            }
            stateMachine.PopStackFrame();
            return stateMachine.GetReturnRegisterValue();
        }
    }
}
