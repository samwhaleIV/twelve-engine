using ElfScript.VirtualMachine;
using ElfScript.Errors;

namespace ElfScript.Expressions.Variable.Collections {
    internal sealed class IndexSetExpression:Expression {

        public string VariableName { get; private init; }

        public Expression IndexExpression { get; private init; }
        public Expression ValueExpression { get; private init; }

        public IndexSetExpression(string name,Expression indexExpression,Expression valueExpression) {
            VariableName = name;
            IndexExpression = indexExpression;
            ValueExpression = valueExpression;
        }

        private Value EvaluateList(StateMachine stateMachine,Address address) {
            var list = stateMachine.Memory.GetList(address);
            var indexValue = IndexExpression.Evaluate(stateMachine);
            if(indexValue.Type != Type.Number) {
                throw ErrorFactory.IndexValueTypeError(Type.Number);
            }
            var index = stateMachine.Memory.GetNumber(indexValue.Address);
            var value = ValueExpression.Evaluate(stateMachine);
            list[index] = value;
            return value;
        }

        private Value EvaluateTable(StateMachine stateMachine,Address address) {
            var table = stateMachine.Memory.GetTable(address);
            var indexValue = IndexExpression.Evaluate(stateMachine);
            if(indexValue.Type != Type.String) {
                throw ErrorFactory.IndexValueTypeError(Type.String);
            }
            var value = ValueExpression.Evaluate(stateMachine);
            var index = stateMachine.Memory.GetString(indexValue.Address);
            table[index] = value;
            return value;
        }

        public override Value Evaluate(StateMachine stateMachine) {
            var value = stateMachine.GetValue(VariableName);
            if(value.Type == Type.List) {
                return EvaluateList(stateMachine,value.Address);
            } else if(value.Type == Type.Table) {
                return EvaluateTable(stateMachine,value.Address);
            } else {
                throw ErrorFactory.InvalidIndexOperation(VariableName);
            }
        }
    }
}
