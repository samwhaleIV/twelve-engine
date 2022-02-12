using System;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Shell.Input {
    public sealed class KeyWatcher {

        private readonly Keys key;
        public Keys Key => key;

        private bool isPressed = false;
        public bool IsPressed => isPressed;

        private readonly Action action;

        public KeyWatcher(Keys key,Action action) {
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

