namespace TwelveEngine.Porthole.PuzzleGame.Components {
    public sealed class XOR:Gate {
        protected override void UpdateSignal() {
            if(Input == null || SecondInput == null) {
                SignalState = SignalState.Neutral;
            } else {
                SignalState = Input.SignalState.XOR(SecondInput.SignalState);
            }
        }
    }
}
