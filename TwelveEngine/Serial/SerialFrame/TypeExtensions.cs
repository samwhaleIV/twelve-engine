using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using TwelveEngine.Shell.Input;

namespace TwelveEngine.Serial {
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
        public void Set(Vector3 vector) {
            Set(vector.X);
            Set(vector.Y);
            Set(vector.Z);
        }
        public void Set(Color color) {
            Set(color.R);
            Set(color.G);
            Set(color.B);
            Set(color.A);
        }

        public void Set(Rectangle rectangle) {
            Set(rectangle.Location);
            Set(rectangle.Size);
        }

        public Vector2 GetVector2() {
            float x = GetFloat(), y = GetFloat();
            return new Vector2(x,y);
        }

        public Point GetPoint() {
            int x = GetInt(), y = GetInt();
            return new Point(x,y);
        }

        public Vector3 GetVector3() {
            float x = GetFloat(), y = GetFloat(), z = GetFloat();
            return new Vector3(x,y,z);
        }

        public Color GetColor() {
            var r = GetByte();
            var g = GetByte();
            var b = GetByte();
            var a = GetByte();
            return new Color(r,g,b,a);
        }

        public Rectangle GetRectangle() {
            return new Rectangle(GetPoint(),GetPoint());
        }
    }
}
