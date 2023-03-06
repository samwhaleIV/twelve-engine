using ElfScript.VirtualMachine;

namespace ElfScript.Expressions.Variable {
    internal sealed class DeleteVariableExpression:Expression {

        public string VariableName { get; private init; }

        public DeleteVariableExpression(string variableName) {
            VariableName = variableName;
        }

        public override Value Evaluate(StateMachine stateMachine) {
            stateMachine.DeleteValue(VariableName);
            return Value.None;
        }
    }
}
