using System;

namespace Elves.Scenes.Battle.UI {
    public readonly struct ButtonState {

        public readonly ButtonPosition Position;
        public readonly bool OnScreen;

        public ButtonState(ButtonPosition position,bool onScreen) {
            Position = position;
            OnScreen = onScreen;
        }

        public ButtonState GetOffscreen() => new(Position, false);
        public ButtonState GetOnScreen() => new(Position, true);
        public ButtonState GetOpposite() => new(Position, !OnScreen);

        public static readonly ButtonState TopLeft = new(ButtonPosition.TopLeft, true);
        public static readonly ButtonState TopRight = new(ButtonPosition.TopRight, true);
        public static readonly ButtonState BottomLeft = new(ButtonPosition.BottomLeft, true);
        public static readonly ButtonState BottomRight = new(ButtonPosition.BottomRight, true);

        public static readonly ButtonState CenterLeft = new(ButtonPosition.CenterLeft, true);
        public static readonly ButtonState CenterMiddle = new(ButtonPosition.CenterMiddle, true);
        public static readonly ButtonState CenterRight = new(ButtonPosition.CenterRight, true);
        public static readonly ButtonState CenterBottom = new(ButtonPosition.CenterBottom, true);

        public static readonly ButtonState None = new(ButtonPosition.None, false);

        public static bool operator == (ButtonState a, ButtonState b) {
            return a.OnScreen == b.OnScreen && a.Position == b.Position;
        }

        public static bool operator != (ButtonState a, ButtonState b) {
            return a.OnScreen != b.OnScreen || a.Position != b.Position;
        }

        public override bool Equals(object obj) {
            if(obj is not ButtonState) {
                return false;
            }
            return (ButtonState)obj == this;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Position,OnScreen);
        }
    }
}
