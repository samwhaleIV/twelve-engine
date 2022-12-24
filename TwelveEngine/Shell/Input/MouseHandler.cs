using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Shell.Input {
    public sealed class MouseHandler {

        public event Action<Point> OnPress, OnRelease, OnMove;
        public event Action<Point,ScrollDirection> OnScroll;

        private MouseState? lastState = null;

        public bool Capturing { get; private set; }

        public Point Position { get; private set; }
        public Point Delta { get; private set; }

        public int X => Position.X;
        public int Y => Position.Y;

        public int XDelta => Delta.X;
        public int YDelta => Delta.Y;

        private void FireScrollEvent(int delta) {
            OnScroll?.Invoke(Position,delta > 0 ? ScrollDirection.Up : ScrollDirection.Down);
        }

        public void Update(MouseState mouseState,bool fireEvents = true) {
            if(!this.lastState.HasValue) {
                this.lastState = mouseState;
            }
            var lastState = this.lastState.Value;

            Position = mouseState.Position;
            Delta = lastState.Position - Position;

            if(fireEvents && mouseState.LeftButton != lastState.LeftButton) {
                if(mouseState.LeftButton == ButtonState.Pressed) {
                    Capturing = true;
                    OnPress?.Invoke(Position);
                } else {
                    Capturing = false;
                    OnRelease?.Invoke(Position);
                }
            }

            int scrollDelta = mouseState.ScrollWheelValue - lastState.ScrollWheelValue;

            if(fireEvents && scrollDelta != 0) {
                FireScrollEvent(scrollDelta);
            }

            if(fireEvents && Position != lastState.Position) {
                OnMove?.Invoke(Position);
            }

            this.lastState = mouseState;
        }
    }
}
