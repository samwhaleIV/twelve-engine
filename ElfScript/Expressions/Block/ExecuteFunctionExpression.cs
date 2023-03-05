using ElfScript.Errors;

namespace ElfScript.Expressions.Block {
    internal sealed class ExecuteFunctionExpression:Expression {

        public string FunctionVariableName { get; private set; }
        public Expression[] FunctionParameterExpressions { get; private set; }

        public ExecuteFunctionExpression(string functionVariableName,Expression[] functionParameterExpressions) {
            FunctionVariableName = functionVariableName;
            FunctionParameterExpressions = functionParameterExpressions;
            AsyncEnabled = true;
        }

        public override Value Evaluate(StateMachine stateMachine) {
            var value = stateMachine.GetValue(FunctionVariableName);
            if(value.Type != Type.Function) {
                throw ErrorFactory.ValueTypeError(FunctionVariableName,Type.Function);
            }
            var functionExpression = stateMachine.Memory.GetFunction(value.ID);
            /* Warning! Dynamically allocated stack memory... */
            Span<Value> parameterValues = stackalloc Value[FunctionParameterExpressions.Length];
            for(int i = 0;i < parameterValues.Length;i++) {
                parameterValues[i] = FunctionParameterExpressions[i].Evaluate(stateMachine);
            }
            value = functionExpression.Invoke(stateMachine,parameterValues);
            return value;
        }

        public override async Task<Value> EvaluateAsync(StateMachine stateMachine) {
            var value = stateMachine.GetValue(FunctionVariableName);
            if(value.Type != Type.Function) {
                throw ErrorFactory.ValueTypeError(FunctionVariableName,Type.Function);
            }
            var functionExpression = stateMachine.Memory.GetFunction(value.ID);
            var parameterValues = new Value[FunctionParameterExpressions.Length];
            for(int i = 0;i < parameterValues.Length;i++) {
                var parameterExpression = FunctionParameterExpressions[i];
                if(parameterExpression.AsyncEnabled) {
                    parameterValues[i] = await parameterExpression.EvaluateAsync(stateMachine);
                    continue;
                }
                parameterValues[i] = parameterExpression.Evaluate(stateMachine);
            }
            value = await functionExpression.InvokeAsync(stateMachine,parameterValues);
            return value;
        }
    }
}
