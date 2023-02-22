using TwelveEngine.Input;
using TwelveEngine.Input.Routing;

namespace TwelveEngine.Shell {
    public class InputGameState:GameState {

        public ImpulseEventHandler Impulse { get; private init; } = new();
        public MouseEventHandler Mouse { get; private init; } = new();

        private static ImpulseEventHandler oldInputHandler;
        private static MouseEventHandler oldMouseHandler;

        public VirtualMouseProvider VirtualMouseProvider { get; private init; }

        public InputGameState() {
            OnLoad.Add(ImportOldHandlers,EventPriority.First);
            OnUpdate.Add(UpdateInputs,EventPriority.First);
            OnUnload.Add(SetOldHandlers);
            VirtualMouseProvider = new(this);
            OnRender.Add(RenderVirtualMouse,EventPriority.Last);
        }

        private void RenderVirtualMouse() {
            VirtualMouseProvider.TryRenderVirtualMouse(SpriteBatch,CustomCursor.State);
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

            VirtualMouseProvider.Update();
            Mouse.Update(inputEnabled,GameIsActive);
            Impulse.Update(inputEnabled);
        }

        internal override bool GetCustomCursorHiddenState() {
            bool? hiddenState = VirtualMouseProvider.HiddenState;
            if(hiddenState.HasValue) {
                return hiddenState.Value;
            }
            return base.GetCustomCursorHiddenState();
        }
    }
}
