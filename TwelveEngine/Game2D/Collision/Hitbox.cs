namespace TwelveEngine.Game2D {
    public struct Hitbox {
        public Hitbox(
            float x, float y,
            float width, float height
        ) {
            X = x; Y = y;
            Width = width; Height = height;
        }

        public float X; public float Y;
        public float Width; public float Height;

        public bool Collides(Hitbox target) =>
            X <= target.X + target.Width &&
            X + Width >= target.X &&
            Y <= target.Y + target.Height &&
            Height + Y >= target.Y;
    }
}
