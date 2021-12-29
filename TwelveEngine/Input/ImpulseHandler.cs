using System;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {
    public enum InputMethod {
        Unknown, Keyboard, Gamepad
    }

    public sealed class ImpulseHandler {

        public ImpulseHandler(KeyBinds keyBinds) {
            this.keyBinds = keyBinds;
        }
        private readonly KeyBinds keyBinds;

        private InputMethod inputMethod = InputMethod.Unknown;
        public InputMethod InputMethod => inputMethod;

        public void Update(KeyboardState keyboardState,GamePadState gamePadState) {

        }

        public bool IsKeyDown(Impulse impulse) {
            return false; //todo
        }
        public bool IsKeyUp(Impulse impulse) {
            return true; //todo
        }

        public event Action<InputMethod> OnInputMethodChanged;

        public event Action OnAcceptDown;
        public event Action OnAcceptUp;

        public event Action OnEscapeDown;
        public event Action OnEscapeUp;

        public event Action<Direction> OnDirectionUp;
        public event Action<Direction> OnDirectionDown;
    }
}
