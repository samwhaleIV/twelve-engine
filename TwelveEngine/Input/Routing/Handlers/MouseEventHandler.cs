using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input.Routing {
    public sealed class MouseEventHandler:InputEventHandler<MouseEvent,MouseEventRouter> {

        private MouseState? lastState = null;

        public Point Position { get; private set; }
        public Point Delta { get; private set; }
        public bool Capturing { get; private set; }
        public bool CapturingLeft { get; private set; }
        public bool CapturingRight { get; private set; }

        public void Import(MouseEventHandler oldHandler) {
            lastState = oldHandler.lastState;
            Position = oldHandler.Position;
            Delta = oldHandler.Delta;
            Capturing = oldHandler.Capturing;
            CapturingLeft = oldHandler.CapturingLeft;
            CapturingRight = oldHandler.CapturingRight;
        }

        private void UpdateLeftClick(MouseState mouseState,MouseState lastState) {
            if(mouseState.LeftButton == lastState.LeftButton) {
                return;
            }
            if(mouseState.LeftButton == ButtonState.Pressed) {
                SendEvent(MouseEvent.LeftClickPress(mouseState.Position));
            } else {
                SendEvent(MouseEvent.LeftClickRelease(mouseState.Position));
            }
        }

        private void UpdateRightClick(MouseState mouseState,MouseState lastState) {
            if(mouseState.RightButton == lastState.RightButton) {
                return;
            }
            if(mouseState.RightButton == ButtonState.Pressed) {
                SendEvent(MouseEvent.RightClickPress(mouseState.Position));
            } else {
                SendEvent(MouseEvent.RightClickRelease(mouseState.Position));
            }
        }

        public void Release() {
            Delta = Point.Zero;
            Point position = InputStateCache.Mouse.Position;
            if(CapturingLeft) {
                SendEvent(MouseEvent.LeftClickRelease(position));
            }
            if(CapturingRight) {
                SendEvent(MouseEvent.RightClickRelease(position));
            }
            CapturingLeft = false;
            CapturingRight = false;
            Capturing = false;
        }

        /// <summary>
        ///  Update event based mouse inputs for the current frame.
        /// </summary>
        /// <param name="eventsAreAllowed">Whether or not to send mouse events for this update cycle. <see cref="MouseEventType.Update"/> is always sent.</param>
        public void Update(bool eventsAreAllowed) {
            MouseState mouseState = InputStateCache.Mouse;

            if(!this.lastState.HasValue) {
                this.lastState = mouseState;
            }
            var lastState = this.lastState.Value;
            Position = mouseState.Position;
            this.lastState = mouseState;

            SendEvent(MouseEvent.Update(Position));

            if(!eventsAreAllowed) {
                return;
            }

            Delta = lastState.Position - Position;

            bool leftButtonPressed = mouseState.LeftButton == ButtonState.Pressed;
            bool rightButtonPressed = mouseState.RightButton == ButtonState.Pressed;

            CapturingLeft = leftButtonPressed;
            CapturingRight = rightButtonPressed;

            Capturing = CapturingLeft || CapturingRight;

            UpdateLeftClick(mouseState,lastState);
            UpdateRightClick(mouseState,lastState);

            int scrollDelta = mouseState.ScrollWheelValue - lastState.ScrollWheelValue;

            if(scrollDelta != 0) {
                SendEvent(MouseEvent.Scroll(scrollDelta > 0,mouseState.Position));
            }

            if(Position != lastState.Position) {
                SendEvent(MouseEvent.Move(mouseState.Position));
            }
        }
    }
}
