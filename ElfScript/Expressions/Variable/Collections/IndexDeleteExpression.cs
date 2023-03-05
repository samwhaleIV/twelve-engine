using ElfScript.Errors;

namespace ElfScript.Expressions.Variable.Collections {
    internal sealed class IndexDeleteExpression:Expression {

        public string VariableName { get; private init; }

        public Expression IndexExpression { get; private init; }

        public IndexDeleteExpression(string name,Expression indexExpression) {
            VariableName = name;
            IndexExpression = indexExpression;
        }

        public override Value Evaluate(StateMachine stateMachine) {
            var value = stateMachine.GetValue(VariableName);
            if(value.Type != Type.Table) {
                throw ErrorFactory.InvalidIndexDeleteOperation(VariableName);
            }
            var table = stateMachine.Memory.GetTable(value.ID);
            var index = IndexExpression.Evaluate(stateMachine);
            if(index.Type != Type.String) {
                throw ErrorFactory.IndexTypeError(Type.String);
            }
            table.Remove(stateMachine.Memory.GetString(index.ID));
            return Value.None;
        }
    }
}
