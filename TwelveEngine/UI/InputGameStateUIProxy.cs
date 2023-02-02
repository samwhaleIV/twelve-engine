using TwelveEngine.Shell;

namespace TwelveEngine.UI {
    public abstract class InputGameStateUIProxy<TState,TElement>:InteractionAgent<TElement> where TElement:InteractionElement<TElement> where TState:InputGameState {
        public TState State { get; private set; }

        public InputGameStateUIProxy(TState state) {
            State = state;
            BindInputEvents(State);
            state.OnUpdate += SetCustomCursor;
        }

        private void SetCustomCursor() => CustomCursor.State = CursorState;
        protected override bool GetContextTransitioning() => State.IsTransitioning;
        protected override TimeSpan GetCurrentTime() => State.Now;
        protected override bool GetLeftMouseButtonIsCaptured() => State.Mouse.CapturingLeft;
        protected override bool GetRightMouseButtonIsCaptured() => State.Mouse.CapturingRight;
    }
}
