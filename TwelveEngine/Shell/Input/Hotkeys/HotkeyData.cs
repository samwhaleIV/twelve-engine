using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Shell.Input {
    public readonly struct HotkeyData {
        public HotkeyData(KeyboardState keyboardState,GameTime gameTime) {
            KeyboardState = keyboardState;
            GameTime = gameTime;
        }
        public readonly GameTime GameTime;
        public readonly KeyboardState KeyboardState;
    }
}
