using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {
    public sealed class MouseHandler {

        public event Action OnPress, OnRelease, OnMove;
        public event Action<ScrollDirection> OnScroll;

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

        private bool lastUpdateHadInactiveWindow = false;

        public void Update(MouseState mouseState,bool eventsAreAllowed,bool gameIsActive) {
            if(!this.lastState.HasValue) {
                this.lastState = mouseState;
            }
            var lastState = this.lastState.Value;
            Position = mouseState.Position;
            this.lastState = mouseState;
            Delta = lastState.Position - Position;

            if(!eventsAreAllowed) {
                Delta = Point.Zero;
                Capturing = false;
                /* Hint for the mouse button change processor to fire an extra OnPress event so we don't get a release event without a press event */
                lastUpdateHadInactiveWindow = !gameIsActive;
                return;
            }

            if(gameIsActive && lastUpdateHadInactiveWindow && mouseState.LeftButton == ButtonState.Pressed) {
                Capturing = true;
                OnPress?.Invoke();
            } else {
                if(mouseState.LeftButton != lastState.LeftButton) {
                    if(mouseState.LeftButton == ButtonState.Pressed) {
                        Capturing = true;
                        OnPress?.Invoke();
                    } else {
                        Capturing = false;
                        OnRelease?.Invoke();
                    }
                }
            }
            lastUpdateHadInactiveWindow = !gameIsActive;

            int scrollDelta = mouseState.ScrollWheelValue - lastState.ScrollWheelValue;

            if(scrollDelta != 0) {
                OnScroll?.Invoke(scrollDelta > 0 ? ScrollDirection.Up : ScrollDirection.Down);
            }

            if(Position != lastState.Position) {
                OnMove?.Invoke();
            }
        }
    }
}
