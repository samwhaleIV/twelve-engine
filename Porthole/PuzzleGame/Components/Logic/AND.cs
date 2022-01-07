namespace Porthole.PuzzleGame.Components {
    public sealed class AND:Gate {
        protected override void UpdateSignal() {
            if(Input == null || SecondInput == null) {
                SignalState = SignalState.Neutral;
            } else {
                SignalState = Input.SignalState.AND(SecondInput.SignalState);
            }
        }
    }
}
