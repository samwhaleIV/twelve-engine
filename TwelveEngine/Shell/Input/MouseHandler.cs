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

        public void Import(MouseHandler oldHandler) {
            Position = oldHandler.Position;
            Delta = oldHandler.Delta;
            Capturing = oldHandler.Capturing;
            lastState = oldHandler.lastState;
        }

        public void Update(MouseState mouseState,bool fireEvents = true) {
            if(!this.lastState.HasValue) {
                this.lastState = mouseState;
            }
            var lastState = this.lastState.Value;
            Position = mouseState.Position;
            this.lastState = mouseState;
            Delta = lastState.Position - Position;

            if(!fireEvents) {
                Delta = Point.Zero;
                Capturing = false;
                return;
            }

            if(mouseState.LeftButton != lastState.LeftButton) {
                if(mouseState.LeftButton == ButtonState.Pressed) {
                    Capturing = true;
                    OnPress?.Invoke(Position);
                } else {
                    Capturing = false;
                    OnRelease?.Invoke(Position);
                }
            }

            int scrollDelta = mouseState.ScrollWheelValue - lastState.ScrollWheelValue;

            if(scrollDelta != 0) {
                OnScroll?.Invoke(Position,scrollDelta > 0 ? ScrollDirection.Up : ScrollDirection.Down);
            }

            if(Position != lastState.Position) {
                OnMove?.Invoke(Position);
            }
        }
    }
}
