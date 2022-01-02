using TwelveEngine.Porthole.PuzzleGame;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine {
    public sealed partial class SerialFrame {
        public void Set(Direction direction) => Set((byte)direction);
        public Direction GetDirection() => (Direction)GetByte();

        public void Set(SignalState signalState) => Set((int)signalState);
        public SignalState GetSignalState() => (SignalState)GetInt();

        public void Set(Keys key) => Set((byte)key);
        public Keys GetKey() => (Keys)GetByte();

        public void Set(Impulse bind) => Set((int)bind);
        public Impulse GetBind() => (Impulse)GetInt();
    }
}
