namespace TwelveEngine.Shell {
    public struct TransitionData {
        public Func<GameState> Generator { get; set; }
        public GameState State { get; set; }
        public TimeSpan Duration { get; set; }
        public StateData Data { get; set; }
    }
}
