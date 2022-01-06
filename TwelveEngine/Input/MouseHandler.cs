using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {
    public sealed class MouseHandler {

        public event Action<Point> OnMouseDown, OnMouseUp, OnMouseMove;
        public event Action<Point,ScrollDirection> OnMouseScroll;

        private MouseState? lastState;

        private bool mouseIsDown = false;
        public bool Capturing => mouseIsDown;

        private Point position;

        public int X => position.X;
        public int Y => position.Y;

        public void Update(MouseState mouseState) {
            if(!this.lastState.HasValue) {
                this.lastState = mouseState;
            }
            var lastState = this.lastState.Value;

            position = mouseState.Position;

            if(mouseState.LeftButton != lastState.LeftButton) {
                if(mouseState.LeftButton == ButtonState.Pressed) {
                    mouseIsDown = true;
                    OnMouseDown?.Invoke(position);
                } else {
                    mouseIsDown = false;
                    OnMouseUp?.Invoke(position);
                }
            }

            int scrollDelta = mouseState.ScrollWheelValue - lastState.ScrollWheelValue;

            if(scrollDelta != 0) {
                OnMouseScroll?.Invoke(position,scrollDelta > 0 ? ScrollDirection.Up : ScrollDirection.Down);
            }

            if(position != lastState.Position) {
                OnMouseMove?.Invoke(position);
            }

            this.lastState = mouseState;
        }
    }
}
