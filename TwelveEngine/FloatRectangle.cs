namespace TwelveEngine {
    public struct FloatRectangle {
        private float x, y, width, height;

        public FloatRectangle(int x,int y,int width,int height) {
            this.x = x;
            this.y = y;

            this.width = width;
            this.height = height;
        }

        public FloatRectangle(int x,int y,float width,float height) {
            this.x = x;
            this.y = y;

            this.width = width;
            this.height = height;
        }

        public FloatRectangle(float x,float y,int width,int height) {
            this.x = x;
            this.y = y;

            this.width = width;
            this.height = height;
        }

        public FloatRectangle(float x,float y,float width,float height) {
            this.x = x;
            this.y = y;

            this.width = width;
            this.height = height;
        }

        public FloatRectangle(int x,int y,int width,int height,Texture2D uvSource) {
            this.x = (float)x / uvSource.Width;
            this.y = (float)y / uvSource.Height;
            this.width = (float)width / uvSource.Width;
            this.height = (float)height / uvSource.Height;
        }

        public FloatRectangle(Vector2 position,Vector2 size) {
            x = position.X;
            y = position.Y;

            width = size.X;
            height = size.Y;
        }

        public FloatRectangle(Vector2 position,float width,float height) {
            x = position.X;
            y = position.Y;

            this.width = width;
            this.height = height;
        }

        public FloatRectangle(float x,float y,Vector2 size) {
            this.x = x;
            this.y = y;

            width = size.X;
            height = size.Y;
        }

        public FloatRectangle(Rectangle rectangle) {
            x = rectangle.X;
            y = rectangle.Y;

            width = rectangle.Width;
            height = rectangle.Height;
        }

        public FloatRectangle(Viewport viewport) {
            x = viewport.X;
            y = viewport.Y;

            width = viewport.Width;
            height = viewport.Height;
        }

        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }

        public float Width { get => width; set => width = value; }
        public float Height { get => height; set => height = value; }

        public Vector2 Size {
            get => new(width,height);
            set {
                width = value.X;
                height = value.Y;
            }
        }

        public Vector2 Position {
            get => new(x,y);
            set {
                x = value.X;
                y = value.Y;
            }
        }

        public Vector2 Center => new(x + width * 0.5f,y + height * 0.5f);
        public float CenterX => x + width * 0.5f;
        public float CenterY => y + height * 0.5f;

        public float Top => y;
        public float Bottom => y + height;

        public float Left => x;
        public float Right => x + width;

        public Vector2 TopLeft => new(x,y);
        public Vector2 BottomRight => new(x+width,y+height);

        public Vector2 BottomLeft => new(x,y+height);
        public Vector2 TopRight => new(x+width,y);

        public static readonly FloatRectangle One = new(0,0,1,1);
        public static readonly FloatRectangle Empty = new(0,0,0,0);

        public bool Contains(Point point) {
            return x <= point.X && point.X < x + width && y <= point.Y && point.Y < y + height;
        }

        public bool Contains(Vector2 vector) {
            return x <= vector.X && vector.X < x + width && y <= vector.Y && vector.Y < y + height;
        }

        public bool Contains(int x,int y) {
            return this.x <= x && x < this.x + width && this.y <= y && y < this.y + height;
        }

        public bool Contains(float x,float y) {
            return this.x <= x && x < this.x + width && this.y <= y && y < this.y + height;
        }

        public static explicit operator Rectangle(FloatRectangle vectorRectangle) {
            return new Rectangle((int)vectorRectangle.x,(int)vectorRectangle.y,(int)vectorRectangle.width,(int)vectorRectangle.height);
        }

        public Rectangle ToRectangle() {
            return new Rectangle((int)x,(int)y,(int)width,(int)height);
        }

        public override int GetHashCode() {
            return HashCode.Combine(x,y,width,height);
        }

        public override bool Equals(object self) {
            return self is FloatRectangle other && Equals(other);
        }

        public bool Equals(FloatRectangle vectorRectangle) {
            return x == vectorRectangle.X &&
                   y == vectorRectangle.Y &&
                   width == vectorRectangle.Width &&
                   height == vectorRectangle.Height;
        }

        public static bool operator ==(FloatRectangle a,FloatRectangle b) {
            return a.Equals(b);
        }
        public static bool operator !=(FloatRectangle a,FloatRectangle b) {
            return !a.Equals(b);
        }

        public float AspectRatio => width / height;
    }
}
