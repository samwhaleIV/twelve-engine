namespace TwelveEngine {
    public class DynamicGameState<TData>:GameState {
        protected TData Data { get; private set; }
        internal void SetData(TData data) => Data = data;
    }
}
