namespace TwelveEngine.PuzzleGame.Components {
    public sealed class OR:Gate {
        public override void UpdateSignal() {
            if(Input == null || SecondInput == null) {
                SignalState = SignalState.Neutral;
            } else {
                SignalState = Input.SignalState.OR(SecondInput.SignalState);
            }
        }
    }
}
