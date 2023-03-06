using ElfScript.VirtualMachine;

namespace ElfScript.Expressions.Variable.Types {
    internal sealed class StringExpression:Expression {

        public string String { get; private init; }

        public StringExpression(string stringValue) {
            String = stringValue;
        }

        public override Value Evaluate(StateMachine stateMachine) {
            return stateMachine.Memory.CreateString(String);
        }
    }
}
