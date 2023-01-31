using Microsoft.Xna.Framework.Input;
using TwelveEngine.Shell.Automation;

namespace TwelveEngine.Shell {
    public static class InputStateCache {

        public static KeyboardState Keyboard { get; private set; }
        public static MouseState Mouse { get; private set; }
        public static GamePadState GamePad { get; private set; }

        public static KeyboardState LastKeyboard { get; private set; }
        public static MouseState LastMouse { get; private set; }
        public static GamePadState LastGamePad { get; private set; }

        public static bool LastInputEventWasFromMouse { get; private set; } = false;

        private static KeyboardState GetKeyboardState() {
            return AutomationAgent.FilterKeyboardState(Microsoft.Xna.Framework.Input.Keyboard.GetState());
        }

        private static MouseState GetMouseState() {
            return AutomationAgent.FilterMouseState(Microsoft.Xna.Framework.Input.Mouse.GetState());
        }

        private static GamePadState GetGamepadState() {
            return Microsoft.Xna.Framework.Input.GamePad.GetState(Config.GetInt(Config.Keys.GamePadIndex),GamePadDeadZone.Circular);
        }

        /* A little bit expensive... but powerful. */
        public static void Update(bool gameIsActive) {
            /* States are filtered by the automation agent. State cannot be changed while the game is paused or waiting. */
            KeyboardState keyboardState = GetKeyboardState();
            MouseState mouseState = GetMouseState();
            GamePadState gamePadState = GetGamepadState();

            /* Priority for keyboard or gamepad events, even if the mouse data changed in the same frame. */
            if(gameIsActive && mouseState != LastMouse) {
                LastInputEventWasFromMouse = true;
                //Console.WriteLine("MOUSE ACTIVE");
            }
            if(gameIsActive && keyboardState != LastKeyboard || gamePadState != LastGamePad) {
                LastInputEventWasFromMouse = false;
                //Console.WriteLine("KEYBOARD ACTIVE");
            }

            LastMouse = mouseState;
            LastKeyboard = keyboardState;
            LastGamePad = gamePadState;

            Keyboard = keyboardState;
            Mouse = mouseState;
            GamePad = gamePadState;
        }
    }
}
