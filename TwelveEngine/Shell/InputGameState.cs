using TwelveEngine.Input.Routing;

namespace TwelveEngine.Shell {
    public class InputGameState:GameState {

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
            Mouse.Update(inputEnabled,GameIsActive);
            Impulse.Update(inputEnabled);
        }
    }
}
