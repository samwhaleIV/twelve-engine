using Microsoft.Xna.Framework.Input;
using TwelveEngine.Shell.Automation;

namespace TwelveEngine.Input {
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

        public static bool MouseChanged { get; private set; }
        public static bool KeyboardChanged { get; private set; }
        public static bool GamePadChanged { get; private set; }

        public static ImpulseInputType RecentImpulseType { get; private set; } = ImpulseInputType.Unknown;

        /* A little bit expensive... but powerful. */
        public static void Update(bool gameIsActive) {
            /* States are filtered by the automation agent. State cannot be changed while the game is paused or waiting. */
            KeyboardState keyboardState = GetKeyboardState();
            MouseState mouseState = GetMouseState();
            GamePadState gamePadState = GetGamepadState();

            MouseChanged = mouseState != LastMouse;
            KeyboardChanged = keyboardState != LastKeyboard;
            GamePadChanged = gamePadState != LastGamePad;

            LastMouse = mouseState;
            LastKeyboard = keyboardState;
            LastGamePad = gamePadState;

            Keyboard = keyboardState;
            Mouse = mouseState;
            GamePad = gamePadState;

            if(!gameIsActive) {
                return;
            }
            if(GamePadChanged) {
                RecentImpulseType = ImpulseInputType.GamePad;
            } else if(KeyboardChanged) {
                RecentImpulseType = ImpulseInputType.Keyboard;
            }
            if(KeyboardChanged || GamePadChanged) {
                LastInputEventWasFromMouse = false;
            }
            if(MouseChanged) {
                LastInputEventWasFromMouse = true;
            }
        }
    }
}
