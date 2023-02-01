using TwelveEngine.Input.Routing;

namespace TwelveEngine.Shell {
    public class InputGameState:GameState {

        private readonly TimeoutManager timeoutManager = new();

        public ImpulseEventHandler Impulse { get; private set; } = new();
        public MouseEventHandler Mouse { get; private set; } = new();

        private static ImpulseEventHandler oldInputHandler;
        private static MouseEventHandler oldMouseHandler;

        public InputGameState() {
            OnLoad += InputGameState_OnLoad;
            OnUnload += InputGameState_OnUnload;
            OnUpdate += UpdateInputs;
        }

        private void InputGameState_OnUnload() {
            oldInputHandler = Impulse;
            oldMouseHandler = Mouse;
        }

        private void InputGameState_OnLoad() {
            if(HasFlag(StateFlags.CarryKeyboardInput)) {
                Impulse.Import(oldInputHandler);
            }
            if(HasFlag(StateFlags.CarryMouseInput)) {
                Mouse.Import(oldMouseHandler);
            }
            oldInputHandler = null;
            oldMouseHandler = null;
        }

        public bool ClearTimeout(int ID) {
            return timeoutManager.Remove(ID);
        }

        public int SetTimeout(Action action,TimeSpan delay) {
            return timeoutManager.Add(action,delay,Now);
        }

        protected bool InputEnabled {
            get {
                return GameIsActive && !IsTransitioning;
            }
        }

        private bool _inputActivated = false;

        public event Action OnInputActivated;

        private void UpdateInputs() {
            if(!IsTransitioning && !_inputActivated) {
                _inputActivated = true;
                OnInputActivated?.Invoke();
            }
            bool inputEnabled = InputEnabled && _inputActivated;
            Mouse.Update(InputStateCache.Mouse,inputEnabled,GameIsActive);
            Impulse.Update(inputEnabled);
            timeoutManager.Update(Now);
        }
    }
}
