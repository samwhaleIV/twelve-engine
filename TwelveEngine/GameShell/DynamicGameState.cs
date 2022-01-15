namespace TwelveEngine {
    public class DynamicGameState<TData>:GameState {
        private readonly TData data;
        protected TData Data => data;
        public DynamicGameState(TData data) => this.data = data;
    }
}
