using System;
using TwelveEngine.Shell.Input;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Shell {
    public class InputGameState:GameState {

        private readonly TimeoutManager timeoutManager = new();
        private readonly MouseHandler mouseHandler = new();
        private readonly InputHandler inputHandler = new();
        private readonly InputGuide inputGuide = new();

        public InputHandler Input => inputHandler;
        public MouseHandler Mouse => mouseHandler;
        public InputGuide InputGuide => inputGuide;

        public InputGameState() => OnLoad += InputGameState_OnLoad;

        private void InputGameState_OnLoad() {
            inputHandler.Load(Game.KeyBinds);
            inputGuide.Load(Game,Input);
        }

        public bool ClearTimeout(int ID) {
            return timeoutManager.Remove(ID);
        }

        public int SetTimeout(Action action,TimeSpan delay) {
            return timeoutManager.Add(action,delay,Game.Time.TotalGameTime);
        }

        protected void UpdateImpulseInput() {
            inputHandler.Update(Game.KeyboardState,Game.GamePadState);
        }

        protected void UpdateMouseInput() {
            mouseHandler.Update(Game.MouseState);
        }

        protected void UpdateTimeoutInput() {
            timeoutManager.Update(Now);
        }

        protected void UpdateInputs() {
            if(!Game.IsActive) {
                return;
            }
            UpdateMouseInput();
            UpdateImpulseInput();
            UpdateTimeoutInput();
        }
    }
}
