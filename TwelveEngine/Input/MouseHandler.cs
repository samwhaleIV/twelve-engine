using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {
    public sealed class MouseHandler {

        public event Action<Point> OnMouseDown, OnMouseUp, OnMouseMove;
        public event Action<Point,ScrollDirection> OnMouseScroll;

        private MouseState? lastState = null;

        public bool Capturing { get; private set; }

        public Point Position { get; private set; }
        public Point Delta { get; private set; }

        public int X => Position.X;
        public int Y => Position.Y;

        public int XDelta => Delta.X;
        public int YDelta => Delta.Y;

        private void FireScrollEvent(int delta) {
            OnMouseScroll?.Invoke(Position,delta > 0 ? ScrollDirection.Up : ScrollDirection.Down);
        }

        public void Update(MouseState mouseState) {
            if(!this.lastState.HasValue) {
                this.lastState = mouseState;
            }
            var lastState = this.lastState.Value;

            Position = mouseState.Position;
            Delta = lastState.Position - Position;

            if(mouseState.LeftButton != lastState.LeftButton) {
                if(mouseState.LeftButton == ButtonState.Pressed) {
                    Capturing = true;
                    OnMouseDown?.Invoke(Position);
                } else {
                    Capturing = false;
                    OnMouseUp?.Invoke(Position);
                }
            }

            int scrollDelta = mouseState.ScrollWheelValue - lastState.ScrollWheelValue;

            if(scrollDelta != 0) {
                FireScrollEvent(scrollDelta);
            }

            if(Position != lastState.Position) {
                OnMouseMove?.Invoke(Position);
            }

            this.lastState = mouseState;
        }
    }
}
