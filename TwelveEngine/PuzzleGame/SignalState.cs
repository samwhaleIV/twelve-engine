namespace TwelveEngine.PuzzleGame {
    public enum SignalState {
        Neutral = 0, Positive = 1, Negative = -1
    }
    public static class SignalStateExtensions {
        public static bool Value(this SignalState signalState) {
            return signalState != SignalState.Neutral;
        }
        public static SignalState AND(this SignalState state,SignalState other) {
            return state == other && state != SignalState.Neutral ? state : SignalState.Neutral;
        }
        public static SignalState OR(this SignalState state,SignalState other) {
            bool state1 = state.Value(), state2 = other.Value();
            if(!(state1 || state2)) {
                return SignalState.Neutral;
            } else if(state1 && state2) {
                if(state == SignalState.Positive || state == SignalState.Positive) {
                    return SignalState.Positive;
                } else {
                    return SignalState.Negative;
                }
            } else if(state1) {
                return state;
            } else {
                return other;
            }
        }
        public static SignalState XOR(this SignalState state,SignalState other) {
            bool state1 = state.Value(), state2 = other.Value();
            if(state1 ^ state2) {
                if(state1) {
                    return state;
                } else {
                    return other;
                }
            } else {
                return SignalState.Neutral;
            }
        }
        public static SignalState NOT(this SignalState state) {
            if(state == SignalState.Neutral) {
                return SignalState.Positive;
            } else {
                return SignalState.Neutral;
            }
        }
    }
}
