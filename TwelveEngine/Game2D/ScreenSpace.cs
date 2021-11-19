namespace TwelveEngine.Game2D {
    public struct ScreenSpace {
        public float X;
        public float Y;
        public float Width;
        public float Height;
        public float TileSize;
        public (float x,float y) getCenter() {
            return (X + Width / 2, Y + Height / 2);
        }
    }
}
