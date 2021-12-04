using TwelveEngine.Game2D;

namespace TwelveEngine.PuzzleGame.Components {
    public sealed class Counter:WorldComponent {

        private const int MAX_VALUE = 4;
        private const int MAX_VALUE_SMALL = 2;

        private readonly bool isSmallType;
        private readonly int maxValue;
        private readonly Tiles.CounterTiles tiles;

        private int value = 0;
        private readonly int x; private readonly int y;

        public Counter(Grid2D grid,int x,int y,bool horizontal,bool smallType) : base(grid) {
            isSmallType = smallType;
            maxValue = smallType ? MAX_VALUE_SMALL : MAX_VALUE;
            tiles = horizontal ? Tiles.Counter.Horizontal : Tiles.Counter.Vertical;
            this.x = x; this.y = y;
        }

        private int getDelta() {
            var state = Input.SignalState;
            if(state == SignalState.Positive) {
                return 1;
            } else if(state == SignalState.Negative) {
                return -1;
            }
            return 0;
        }

        private SignalState getSignalState() {
            return value == maxValue ? SignalState.Positive : SignalState.Neutral;
        }

        private SignalState oldInputState = SignalState.Neutral;
        protected override void UpdateSignal() {
            var newInputState = Input.SignalState;
            if(newInputState == oldInputState) {
                return;
            }
            oldInputState = newInputState;
            var delta = getDelta();
            value += delta;
            if(value < 0) {
                value = 0;
            } else if(value > maxValue) {
                value = maxValue;
            }
            SignalState = getSignalState();
        }

        protected override void OnChange() {
            int[] tileValues = isSmallType ? tiles.Small : tiles.Big;
            ObjectLayer[x,y] = tileValues[value];
        }

        public override void Export(SerialFrame frame) {
            base.Export(frame);
            frame.Set(value);
            frame.Set(oldInputState);
        }
        public override void Import(SerialFrame frame) {
            base.Import(frame);
            value = frame.GetInt();
            oldInputState = frame.GetSignalState();
            SendSignal();
        }
    }
}
