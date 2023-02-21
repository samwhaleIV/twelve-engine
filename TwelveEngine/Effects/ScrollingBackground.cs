namespace TwelveEngine.Effects {
    public sealed class ScrollingBackground:InfinityBackground {

        public Vector2 Direction { get; set; } = new(1,0);

        public TimeSpan ScrollTime { get; set; } = TimeSpan.FromSeconds(10);

        public void Update(TimeSpan now) {
            Vector2 direction = Direction;
            float t = (float)(now / ScrollTime);
            Position = direction * t;
        }
    }
}
