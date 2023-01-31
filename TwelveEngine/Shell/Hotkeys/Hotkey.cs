using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Shell.Hotkeys {
    public sealed class Hotkey {

        private readonly Keys key;
        public Keys Key => key;

        private bool isPressed = false;
        public bool IsPressed => isPressed;

        private readonly Action action;

        public Hotkey(Keys key,Action action) {
            this.key = key;
            this.action = action;
        }

        public void Update(KeyboardState keyboardState) {
            if(keyboardState.IsKeyDown(key)) {
                if(isPressed) return;
                action.Invoke();
                isPressed = true;
            } else {
                isPressed = false;
            }
        }
    }
}

