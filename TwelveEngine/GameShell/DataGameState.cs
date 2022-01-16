namespace TwelveEngine {
    public class DataGameState<TData>:GameState {
        protected TData Data { get; private set; }
        internal void SetData(TData data) => Data = data;
    }
}
