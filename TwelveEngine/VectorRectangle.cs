using Microsoft.Xna.Framework;

namespace TwelveEngine {
    public struct VectorRectangle {
        private float x, y, width, height;

        public VectorRectangle(float x,float y,float width,float height) {
            this.x = x;
            this.y = y;

            this.width = width;
            this.height = height;
        }

        public VectorRectangle(Vector2 position,Vector2 size) {
            x = position.X;
            y = position.Y;

            width = size.X;
            height = size.Y;
        }

        public VectorRectangle(Vector2 position,float width,float height) {
            x = position.X;
            y = position.Y;

            this.width = width;
            this.height = height;
        }

        public VectorRectangle(float x,float y,Vector2 size) {
            this.x = x;
            this.y = y;

            width = size.X;
            height = size.Y;
        }

        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }

        public float Width { get => width; set => width = value; }
        public float Height { get => height; set => height = value; }

        public Vector2 Size {
            get => new Vector2(width,height);
            set {
                width = value.X;
                height = value.Y;
            }
        }

        public Vector2 Position {
            get => new Vector2(x,y);
            set {
                x = value.X;
                y = value.Y;
            }
        }

        public Vector2 Center => Position + Size * 0.5f;

        public float Top => Y;
        public float Bottom => Y + Height;

        public float Left => X;
        public float Right => X + Width;

        public Vector2 TopLeft => new Vector2(x,y);
        public Vector2 BottomRight => new Vector2(x+width,y+height);

        public static VectorRectangle Zero => new VectorRectangle(0,0,0,0);
        public static VectorRectangle One => new VectorRectangle(0,0,1,1);
    }
}
