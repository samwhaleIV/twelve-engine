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
        public Keys Interact = Keys.Enter;

        public KeyBinds() => UpdateCache();

        public void Export(SerialFrame frame) {
            frame.Set((int)Up);
            frame.Set((int)Down);
            frame.Set((int)Left);
            frame.Set((int)Right);
            frame.Set((int)Interact);
        }

        public void Import(SerialFrame frame) {
            Up = (Keys)frame.GetInt();
            Down = (Keys)frame.GetInt();
            Left = (Keys)frame.GetInt();
            Right = (Keys)frame.GetInt();
            Interact = (Keys)frame.GetInt();
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
