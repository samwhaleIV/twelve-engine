namespace TwelveEngine.Game2D{
    public struct CollisionType {
        public CollisionType(
            int x,int y,int width,int height,float tileSize
        ) {
            this.X = x / tileSize;
            this.Y = y / tileSize;
            this.Width = width / tileSize;
            this.Height = height / tileSize;
        }
        public float X;
        public float Y;
        public float Width;
        public float Height;
    }
}
