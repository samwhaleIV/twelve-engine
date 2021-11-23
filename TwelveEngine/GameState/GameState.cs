using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using TwelveEngine.Input;

namespace TwelveEngine {
    public abstract class GameState:ISerializable {

        private readonly KeyboardHandler keyboardHandler = new KeyboardHandler();

        private void keyUp(Keys key) {
            KeyUp(this,key);
        }
        private void keyDown(Keys key) {
            KeyDown(this,key);
        }

        public event EventHandler<Keys> KeyUp;
        public event EventHandler<Keys> KeyDown;

        internal GameManager Game = null;

        internal virtual void Load(GameManager game) {
            keyboardHandler.KeyDown = keyDown;
            keyboardHandler.KeyUp = keyUp;
        }
        internal abstract void Unload();

        internal abstract void Draw(GameTime gameTime);
        internal virtual void Update(GameTime gameTime) {
            keyboardHandler.Update(Game);
        }

        public abstract void Export(SerialFrame frame);
        public abstract void Import(SerialFrame frame);
    }
}
