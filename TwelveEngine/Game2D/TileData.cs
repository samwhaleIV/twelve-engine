namespace TwelveEngine.Game2D {
    public readonly struct TileData {
        public Rectangle Source { get; init; }
        public Color Color { get; init; }
        public SpriteEffects SpriteEffects { get; init; }
        public static readonly TileData None = new TileData() {
            Source = new Rectangle(0,0,1,1),
            Color = Color.White,
            SpriteEffects = SpriteEffects.None
        };
    }
}
