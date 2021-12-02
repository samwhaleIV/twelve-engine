namespace TwelveEngine.PuzzleGame.Components {
    public sealed class NOT:Component {
        protected override void UpdateSignal() {
            SignalState = Input?.SignalState.NOT() ?? SignalState.Neutral;
        }
    }
}
