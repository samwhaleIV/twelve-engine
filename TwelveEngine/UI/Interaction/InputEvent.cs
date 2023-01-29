using Microsoft.Xna.Framework;
using TwelveEngine.Input;

namespace TwelveEngine.UI.Interaction {
    public readonly struct InputEvent {

        private readonly int value1, value2;

        public readonly InputEventType Type;

        public readonly Point MousePosition => new(value1,value2);
        public readonly Direction Direction => (Direction)value1;

        private InputEvent(InputEventType type,Point position) {
            Type = type;
            value1 = position.X;
            value2 = position.Y;
        }

        private InputEvent(InputEventType type,Direction direction) {
            Type = type;
            value1 = (int)direction;
            value2 = 0;
        }

        private InputEvent(InputEventType type) {
            Type = type;
            value1 = 0;
            value2 = 0;
        }

        public static InputEvent CreateMouseUpdate(Point point) {
            return new(InputEventType.MouseUpdate,point);
        }

        public static InputEvent CreateDirectionImpulse(Direction directionImpulse) {
            return new(InputEventType.DirectionImpulse,directionImpulse);
        }

        public static readonly InputEvent MousePressed = new(InputEventType.MousePressed);
        public static readonly InputEvent MouseReleased = new(InputEventType.MouseReleased);

        public static readonly InputEvent AcceptPressed = new(InputEventType.AcceptPressed);
        public static readonly InputEvent AcceptReleased = new(InputEventType.AcceptReleased);

        public static readonly InputEvent BackButtonActivated = new(InputEventType.BackButtonActivated);
    }
}
