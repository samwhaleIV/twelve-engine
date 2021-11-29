namespace TwelveEngine.PuzzleGame.Components {
    public sealed class NOT:Component {
        public override void UpdateSignal() {
            SignalState = Input?.SignalState.NOT() ?? SignalState.Neutral;
        }
    }
}
