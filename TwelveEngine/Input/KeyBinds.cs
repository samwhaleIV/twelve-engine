using Microsoft.Xna.Framework.Input;

namespace TwelveEngine {
    public enum KeyBind {
        Up,
        Down,
        Left,
        Right,
        Interact
    }
}
namespace TwelveEngine.Input {
    public sealed class KeyBinds:ISerializable {
        public Keys Up = Keys.W;
        public Keys Down = Keys.S;
        public Keys Left = Keys.A;
        public Keys Right = Keys.D;
        public Keys Interact = Keys.E;

        public KeyBinds() {
            UpdateCache();
        }

        public void Export(SerialFrame frame) {
            frame.Set("Up",(int)Up);
            frame.Set("Down",(int)Down);
            frame.Set("Left",(int)Left);
            frame.Set("Right",(int)Right);
            frame.Set("Interact",(int)Interact);
        }

        public void Import(SerialFrame frame) {
            Up = (Keys)frame.GetInt("Up");
            Down = (Keys)frame.GetInt("Down");
            Left = (Keys)frame.GetInt("Left");
            Right = (Keys)frame.GetInt("Right");
            Interact = (Keys)frame.GetInt("Interact");
            UpdateCache();
        }

        private Keys[] cache = null;
        public void UpdateCache() {
            cache = new Keys[] {
                Up,Down,Left,Right,Interact
            };
        }
        public Keys Get(KeyBind type) {
            return cache[(int)type];
        }
    }
}
