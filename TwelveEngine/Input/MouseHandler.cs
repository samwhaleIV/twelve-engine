using System;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {
    public sealed class MouseHandler {

        public event Action<int,int> OnMouseDown;
        public event Action<int,int> OnMouseUp;
        public event Action<int,int> OnMouseMove;
        public event Action<int,int,ScrollDirection> OnMouseScroll;

        private void sendScrollEvent(ScrollDirection direction) {
            OnMouseScroll?.Invoke(x,y,direction);
        }

        private MouseState? lastState;

        private bool mouseIsDown = false;
        public bool Capturing => mouseIsDown;

        private int x, y;
        public int X => x; public int Y => y;

        public void Update(MouseState mouseState) {
            if(!this.lastState.HasValue) {
                this.lastState = mouseState;
            }
            var lastState = this.lastState.Value;

            x = mouseState.X; y = mouseState.Y;

            if(x != lastState.X || y != lastState.Y) {
                OnMouseMove?.Invoke(x,y);
            }

            if(mouseState.LeftButton != lastState.LeftButton) {
                if(mouseState.LeftButton == ButtonState.Pressed) {
                    mouseIsDown = true;
                    OnMouseDown?.Invoke(x,y);
                } else {
                    mouseIsDown = false;
                    OnMouseUp?.Invoke(x,y);
                }
            }

            var delta = mouseState.ScrollWheelValue - lastState.ScrollWheelValue;
            if(delta > 0) {
                sendScrollEvent(ScrollDirection.Up);
            } else if(delta < 0) {
                sendScrollEvent(ScrollDirection.Down);
            }
            this.lastState = mouseState;
        }
    }
}
