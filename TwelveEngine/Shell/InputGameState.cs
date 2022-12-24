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

        protected bool InputEnabled => Game.IsActive && !IsTransitioning;

        protected void UpdateInputs() {
            mouseHandler.Update(Game.MouseState,InputEnabled);
            if(!InputEnabled) {
                return;
            }
            inputHandler.Update(Game.KeyboardState,Game.GamePadState);
            timeoutManager.Update(Now);
        }
    }
}
