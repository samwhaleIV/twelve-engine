namespace ElfScript.Expressions.System {
    internal sealed class ReadConsoleLineExpression:Expression {
        public override Value Evaluate(StateMachine stateMachine) {
            var input = Console.ReadLine() ?? string.Empty;
            var value = stateMachine.Memory.CreateString(input);
            return value;
        }
    }
}
