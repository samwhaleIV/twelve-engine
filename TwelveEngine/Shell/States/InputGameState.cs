using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Shell.Timeout;
using TwelveEngine.Shell.Input;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Shell.States {
    public class InputGameState:GameState {

        private readonly TimeoutManager timeoutManager = new TimeoutManager();
        private readonly MouseHandler mouseHandler = new MouseHandler();

        private InputHandler inputHandler;
        private InputGuide inputGuide;

        public InputHandler Input => inputHandler;
        public MouseHandler Mouse => mouseHandler;
        public InputGuide InputGuide => inputGuide;

        public InputGameState() => OnLoad += InputGameState_OnLoad;

        private void InputGameState_OnLoad() {
            inputGuide = new InputGuide(inputHandler,Game);
            inputHandler = new InputHandler(Game.KeyBinds);
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

        protected void UpdateTimeoutInput(GameTime gameTime) {
            timeoutManager.Update(gameTime.TotalGameTime);
        }

        protected void UpdateInputs(GameTime gameTime) {
            UpdateMouseInput();
            UpdateImpulseInput();
            UpdateTimeoutInput(gameTime);
        }
    }
}
