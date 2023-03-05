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

        private Value ExecuteList(StateMachine stateMachine,int valueID) {
            var list = stateMachine.Memory.GetList(valueID);
            var indexValue = IndexExpression.Evaluate(stateMachine);
            if(indexValue.Type != Type.Number) {
                throw ErrorFactory.IndexTypeError(Type.Number);
            }
            var index = stateMachine.Memory.GetNumber(indexValue.ID);
            var value = ValueExpression.Evaluate(stateMachine);
            list[index] = value;
            return value;
        }

        private Value ExecuteTable(StateMachine stateMachine,int valueID) {
            var table = stateMachine.Memory.GetTable(valueID);
            var indexValue = IndexExpression.Evaluate(stateMachine);
            if(indexValue.Type != Type.String) {
                throw ErrorFactory.IndexTypeError(Type.String);
            }
            var value = ValueExpression.Evaluate(stateMachine);
            var index = stateMachine.Memory.GetString(indexValue.ID);
            table[index] = value;
            return value;
        }

        public override Value Evaluate(StateMachine stateMachine) {
            var value = stateMachine.GetValue(VariableName);
            if(value.Type == Type.List) {
                return ExecuteList(stateMachine,value.ID);
            } else if(value.Type == Type.Table) {
                return ExecuteTable(stateMachine,value.ID);
            } else {
                throw ErrorFactory.InvalidIndexOperation(VariableName);
            }
        }
    }
}
