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

        protected void UpdateInputDevices() {
            Mouse.Update(InputStateCache.Mouse,InputEnabled,GameIsActive);
            if(!InputEnabled) {
                return;
            }
            Impulse.Update(InputStateCache.Keyboard,InputStateCache.GamePad);
            timeoutManager.Update(Now);
        }
    }
}
