using ElfScript.Errors;
using ElfScript.VirtualMachine;

namespace ElfScript.Expressions.Block {
    internal sealed class FunctionExpression:Expression {

        public string[] ParameterNames { get; private init; }
        public Expression[] FunctionBody { get; private init; }

        public FunctionExpression(string[] parameterNames,Expression[] functionBody) {
            ParameterNames = parameterNames;
            FunctionBody = functionBody;
        }

        public Value Invoke(StateMachine stateMachine,ReadOnlySpan<Value> parameters) {
            if(parameters.Length != ParameterNames.Length) {
                throw ErrorFactory.ParameterMismatch(ParameterNames.Length,parameters.Length);
            }
            stateMachine.CreateStackFrame();
            for(int i = 0;i < parameters.Length;i++) {
                var value = parameters[i];
                stateMachine.SetValue(ParameterNames[i],value.Address);
            }
            for(int i = 0;i < FunctionBody.Length;i++) {
                FunctionBody[i].Evaluate(stateMachine);
            }
            stateMachine.PopStackFrame();
            return stateMachine.GetReturnRegisterValue();
        }

        public async Task<Value> InvokeAsync(StateMachine stateMachine,Value[] parameters) {
            if(parameters.Length != ParameterNames.Length) {
                throw ErrorFactory.ParameterMismatch(ParameterNames.Length,parameters.Length);
            }
            stateMachine.CreateStackFrame();
            for(int i = 0;i < parameters.Length;i++) {
                var value = parameters[i];
                stateMachine.SetValue(ParameterNames[i],value.Address);
            }
            for(int i = 0;i<FunctionBody.Length;i++) {
                var bodyExpression = FunctionBody[i];
                if(bodyExpression.AsyncEnabled) {
                    await bodyExpression.EvaluateAsync(stateMachine);
                    continue;
                }
                bodyExpression.Evaluate(stateMachine);
            }
            stateMachine.PopStackFrame();
            return stateMachine.GetReturnRegisterValue();
        }

        public override Value Evaluate(StateMachine stateMachine) {
            return stateMachine.Memory.CreateFunction(this);
        }
    }
}
