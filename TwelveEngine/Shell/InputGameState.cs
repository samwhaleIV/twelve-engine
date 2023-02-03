using TwelveEngine.Input.Routing;

namespace TwelveEngine.Shell {
    public class InputGameState:GameState {

        public ImpulseEventHandler Impulse { get; private set; } = new();
        public MouseEventHandler Mouse { get; private set; } = new();

        private static ImpulseEventHandler oldInputHandler;
        private static MouseEventHandler oldMouseHandler;

        public InputGameState() {
            OnLoad.Add(ImportOldHandlers,EventPriority.First);
            OnUpdate.Add(UpdateInputs,EventPriority.First);
            OnUnload.Add(SetOldHandlers);
        }

        private void SetOldHandlers() {
            oldInputHandler = Impulse;
            oldMouseHandler = Mouse;
        }

        private void ImportOldHandlers() {
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
