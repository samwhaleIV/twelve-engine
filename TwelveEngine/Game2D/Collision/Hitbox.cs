namespace TwelveEngine.Game2D {
    public struct Hitbox {
        public Hitbox(float x,float y,float width,float height) {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
        public float X;
        public float Y;
        public float Width;
        public float Height;
    }
}
