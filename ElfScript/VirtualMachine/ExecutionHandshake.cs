using ElfScript.Errors;

namespace ElfScript.VirtualMachine {
    /// <summary>
    /// Used as an extreme safe guard in preventing state machine operations executing children directly; they must be routed through the state machine's executor.<br/>
    /// This is a runtime constraint to make sure that the garbage collector runs in nested operations.<br/>
    /// </summary>
    internal sealed class ExecutionHandshake {
        private readonly static HashSet<StateMachine> _states = new();

        public StateMachine StateMachine { get; private init; }

        private ExecutionHandshake(StateMachine stateMachine) => StateMachine = stateMachine;

        public static ExecutionHandshake Create(StateMachine stateMachine) {
            if(_states.Contains(stateMachine)) {
                throw ErrorFactory.IllegalExecutionBypass();
            }
            _states.Add(stateMachine);
            return new(stateMachine);
        }

        public static void Delete(ExecutionHandshake handshake) {
            _states.Remove(handshake.StateMachine);
        }
    }
}
