namespace TwelveEngine.Effects {
    public sealed class ScrollingBackground:InfinityBackground {

        public static ScrollingBackground GetCheckered() {
            return new ScrollingBackground() {
                TileScale = 4f,
                Scale = 2f,
                Bulge = -0.75f,
                ScrollTime = TimeSpan.FromSeconds(30),
                ColorA = Color.FromNonPremultiplied(new Vector4(new Vector3(0.41f),1)),
                ColorB = Color.FromNonPremultiplied(new Vector4(new Vector3(0.66f),1))
            };
        }

        public Vector2 Direction { get; set; } = new(1,0);

        public TimeSpan ScrollTime { get; set; } = TimeSpan.FromSeconds(10);

        public void Update(TimeSpan now) {
            Vector2 direction = Direction;
            float t = (float)(now / ScrollTime);
            Position = direction * t;
        }
    }
}
