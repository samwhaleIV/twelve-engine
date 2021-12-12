namespace TwelveEngine.Game2D{
    public readonly struct CollisionType {
        public CollisionType(
            int x,int y,int width,int height,float tileSize
        ) {
            X = x / tileSize;
            Y = y / tileSize;
            Width = width / tileSize;
            Height = height / tileSize;
        }
        public readonly float X;
        public readonly float Y;
        public readonly float Width;
        public readonly float Height;
    }
}
