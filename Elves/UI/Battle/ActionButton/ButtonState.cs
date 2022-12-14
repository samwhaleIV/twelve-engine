using System;

namespace Elves.UI.Battle {
    public readonly struct ButtonState {

        public readonly ButtonPosition Position;
        public readonly bool OnScreen;

        public ButtonState(ButtonPosition position,bool onScreen) {
            Position = position;
            OnScreen = onScreen;
        }

        public ButtonState GetOffscreen() => new ButtonState(Position, false);
        public ButtonState GetOnScreen() => new ButtonState(Position, true);
        public ButtonState GetOpposite() => new ButtonState(Position, !OnScreen);

        public static ButtonState TopLeft = new ButtonState(ButtonPosition.TopLeft, true);
        public static ButtonState TopRight = new ButtonState(ButtonPosition.TopRight, true);
        public static ButtonState BottomLeft = new ButtonState(ButtonPosition.BottomLeft, true);
        public static ButtonState BottomRight = new ButtonState(ButtonPosition.BottomRight, true);

        public static ButtonState CenterLeft = new ButtonState(ButtonPosition.CenterLeft, true);
        public static ButtonState CenterMiddle = new ButtonState(ButtonPosition.CenterMiddle, true);
        public static ButtonState CenterRight = new ButtonState(ButtonPosition.CenterRight, true);
        public static ButtonState CenterBottom = new ButtonState(ButtonPosition.CenterBottom, true);

        public static ButtonState None => new ButtonState(ButtonPosition.None, false);

        public static bool operator == (ButtonState a, ButtonState b) {
            return a.OnScreen == b.OnScreen && a.Position == b.Position;
        }

        public static bool operator != (ButtonState a, ButtonState b) {
            return a.OnScreen != b.OnScreen || a.Position != b.Position;
        }

        public override bool Equals(object obj) {
            if(!(obj is ButtonState)) {
                return false;
            }
            return (ButtonState)obj == this;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Position,OnScreen);
        }
    }
}
