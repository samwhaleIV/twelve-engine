using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace TwelveEngine {
    public sealed partial class SerialFrame {
        public void Set(Direction direction) => Set((byte)direction);
        public Direction GetDirection() => (Direction)GetByte();

        public void Set(Keys key) => Set((byte)key);
        public Keys GetKey() => (Keys)GetByte();

        public void Set(Impulse bind) => Set((int)bind);
        public Impulse GetBind() => (Impulse)GetInt();

        public void Set(Vector2 vector) {
            Set(vector.X);
            Set(vector.Y);
        }
        public void Set(Point point) {
            Set(point.X);
            Set(point.Y);
        }

        public Vector2 GetVector2() {
            float x = GetFloat(), y = GetFloat();
            return new Vector2(x,y);
        }
        public Point GetPoint() {
            int x = GetInt(), y = GetInt();
            return new Point(x,y);
        }
    }
}
