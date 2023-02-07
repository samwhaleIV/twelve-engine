using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input.Routing {
    public sealed class MouseEventHandler:InputEventHandler<MouseEvent,MouseEventRouter> {

        private MouseState? lastState = null;

        public Point Position { get; private set; }
        public Point Delta { get; private set; }

        private bool _lastUpdateHadInactiveWindow = false;

        public bool Capturing { get; private set; }

        public bool CapturingLeft { get; private set; }

        public bool CapturingRight { get; private set; }

        public void Import(MouseEventHandler oldHandler) {
            Position = oldHandler.Position;
            Delta = oldHandler.Delta;
            lastState = oldHandler.lastState;
            Capturing = oldHandler.Capturing;
            _lastUpdateHadInactiveWindow = oldHandler._lastUpdateHadInactiveWindow;
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

        public void Update(bool eventsAreAllowed,bool gameIsActive) {
            MouseState mouseState = InputStateCache.Mouse;

            if(!this.lastState.HasValue) {
                this.lastState = mouseState;
            }

            var lastState = this.lastState.Value;
            Position = mouseState.Position;
            this.lastState = mouseState;

            SendEvent(MouseEvent.Update(Position));

            if(!eventsAreAllowed) {
                Delta = Point.Zero;
                Capturing = false;
                CapturingLeft = false;
                CapturingRight = false;
                /* Hint for the mouse button change processor to fire an extra OnPress event so we don't get a release event without a press event */
                _lastUpdateHadInactiveWindow = !gameIsActive;
                return;
            }

            Delta = lastState.Position - Position;

            bool leftButtonPressed = mouseState.LeftButton == ButtonState.Pressed;
            bool rightButtonPressed = mouseState.RightButton == ButtonState.Pressed;

            Capturing = gameIsActive && (leftButtonPressed || rightButtonPressed);

            CapturingLeft = gameIsActive && leftButtonPressed;
            CapturingRight = gameIsActive && rightButtonPressed;

            if(gameIsActive && _lastUpdateHadInactiveWindow && (leftButtonPressed || rightButtonPressed)) {
                if(leftButtonPressed) {
                    SendEvent(MouseEvent.LeftClickPress(mouseState.Position));
                }
                if(rightButtonPressed) {
                    SendEvent(MouseEvent.RightClickPress(mouseState.Position));
                }
            } else {
                UpdateLeftClick(mouseState,lastState);
                UpdateRightClick(mouseState,lastState);
            }
            _lastUpdateHadInactiveWindow = !gameIsActive;

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
