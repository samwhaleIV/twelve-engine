using ElfScript.Errors;

namespace ElfScript.Expressions.Block {
    internal sealed class FunctionExpression:Expression {

        public string[] ParameterNames { get; private init; }
        public Expression[] FunctionBody { get; private init; }

        public FunctionExpression(string[] parameterNames,Expression[] functionBody) {
            ParameterNames = parameterNames;
            FunctionBody = functionBody;
        }

        public Value EvaluateFunction(StateMachine stateMachine,ReadOnlySpan<Value> parameters) {
            if(parameters.Length != ParameterNames.Length) {
                throw ErrorFactory.ParameterMismatch(ParameterNames.Length,parameters.Length);
            }
            stateMachine.CreateStackFrame();
            for(int i = 0;i < parameters.Length;i++) {
                var value = parameters[i];
                stateMachine.SetValue(ParameterNames[i],value.ID);
            }
            for(int i = 0;i < FunctionBody.Length;i++) {
                FunctionBody[i].Evaluate(stateMachine);
            }
            stateMachine.PopStackFrame();
            return stateMachine.GetReturnRegisterValue();
        }

        public override Value Evaluate(StateMachine stateMachine) {
            throw ErrorFactory.IllegalFunctionExecution();
        }
    }
}
