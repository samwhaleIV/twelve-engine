using System;
using TwelveEngine.Input;

namespace TwelveEngine.Shell {
    public class InputGameState:GameState {

        private readonly TimeoutManager timeoutManager = new();

        public InputHandler Input { get; private set; } = new InputHandler();
        public MouseHandler Mouse { get; private set; } = new MouseHandler();

        private static InputHandler oldInputHandler;
        private static MouseHandler oldMouseHandler;

        public InputGameState() {
            OnLoad += InputGameState_OnLoad;
            OnUnload += InputGameState_OnUnload;
        }

        private void InputGameState_OnUnload() {
            oldInputHandler = Input;
            oldMouseHandler = Mouse;
        }

        private void InputGameState_OnLoad() {
            if(HasFlag(StateFlags.CarryKeyboardInput)) {
                Input.Import(oldInputHandler);
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
            return timeoutManager.Add(action,delay,Game.Time.TotalGameTime);
        }

        protected bool InputEnabled {
            get {
                return Game.IsActive && !IsTransitioning;
            }
        }

        protected void UpdateInputs() {
            Mouse.Update(Game.MouseState,InputEnabled,Game.IsActive);
            if(!InputEnabled) {
                return;
            }
            Input.Update(Game.KeyboardState,Game.GamePadState);
            timeoutManager.Update(Now);
        }
    }
}
