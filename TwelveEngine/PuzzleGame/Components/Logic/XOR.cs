namespace TwelveEngine.PuzzleGame.Components {
    public sealed class XOR:Gate {
        public override void UpdateSignal() {
            if(Input == null || SecondInput == null) {
                SignalState = SignalState.Neutral;
            } else {
                SignalState = Input.SignalState.XOR(SecondInput.SignalState);
            }
        }
    }
}
