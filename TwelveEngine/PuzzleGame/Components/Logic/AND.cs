namespace TwelveEngine.PuzzleGame.Components {
    public sealed class AND:Gate {
        public override void UpdateSignal() {
            if(Input == null || SecondInput == null) {
                SignalState = SignalState.Neutral;
            } else {
                SignalState = Input.SignalState.AND(SecondInput.SignalState);
            }
        }
    }
}
