namespace ElfScript.Expressions.System {
    internal sealed class WriteConsoleLineExpression:Expression {

        public Expression Expression { get; private init; }

        public WriteConsoleLineExpression(Expression expression) {
            Expression = expression;
        }

        public override Value Evaluate(StateMachine stateMachine) {
            var value = Expression.Evaluate(stateMachine);
            switch(value.Type) {
                case Type.List:
                    Console.WriteLine("<List>");
                    break;
                case Type.Number:
                    Console.WriteLine(stateMachine.Memory.GetNumber(value.ID));
                    break;
                case Type.Function:
                    Console.WriteLine("<Function Expression>");
                    break;
                case Type.String:
                    Console.WriteLine(stateMachine.Memory.GetString(value.ID));
                    break;
                case Type.Table:
                    Console.WriteLine("<Table>");
                    break;
                case Type.Boolean:
                    Console.WriteLine("<Boolean>");
                    break;
                default:
                    Console.WriteLine("<Unknown Type>");
                    break;
            }
            return value;
        }
    }
}
