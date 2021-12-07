using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {
    public sealed class KeyWatcher {

        private readonly Keys key;
        public Keys Key => key;

        private bool isPressed = false;
        public bool IsPressed => isPressed;

        private readonly Action<KeyboardState,GameTime> action;

        public KeyWatcher(Keys key,Action<KeyboardState,GameTime> action) {
            this.key = key;
            this.action = action;
        }
        public KeyWatcher(Keys key,Action action) {
            this.key = key;
            this.action = (keyboardState,gameTime) => action.Invoke();
        }
        public KeyWatcher(Keys key,Action<KeyboardState> action) {
            this.key = key;
            this.action = (keyboardState,gameTime) => action.Invoke(keyboardState);
        }
        public KeyWatcher(Keys key,Action<GameTime> action) {
            this.key = key;
            this.action = (keyboardState,gameTime) => action.Invoke(gameTime);
        }

        public void Process(KeyboardState keyboardState,GameTime gameTime) {
            if(keyboardState.IsKeyDown(key)) {
                if(isPressed) return;
                action.Invoke(keyboardState,gameTime);
                isPressed = true;
            } else {
                isPressed = false;
            }
        }
    }
}
