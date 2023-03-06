using ElfScript.VirtualMachine;

namespace ElfScript.Expressions.Variable.Types {
    internal sealed class NumberExpression:Expression {

        public int Number { get; private init; }

        public NumberExpression(int number) {
            Number = number;
        }

        public override Value Evaluate(StateMachine stateMachine) {
            return stateMachine.Memory.CreateNumber(Number);
        }
    }
}
