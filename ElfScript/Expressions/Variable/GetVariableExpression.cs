using ElfScript.VirtualMachine;

namespace ElfScript.Expressions.Variable {
    internal sealed class GetVariableExpression:Expression {

        public string VariableName { get; private init; }

        public GetVariableExpression(string variableName) {
            VariableName = variableName;
        }

        public override Value Evaluate(StateMachine stateMachine) {
            return stateMachine.GetValue(VariableName);
        }
    }
}
