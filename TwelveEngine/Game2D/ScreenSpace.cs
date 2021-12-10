namespace TwelveEngine.Game2D {
    public readonly struct ScreenSpace {
        public ScreenSpace(float x,float y,float width,float height,int tileSize) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            TileSize = tileSize;
        }

        public readonly float X;
        public readonly float Y;
        public readonly float Width;
        public readonly float Height;
        public readonly int TileSize;

        public (float x,float y) getCenter() {
            return (X + Width / 2, Y + Height / 2);
        }
    }
}
