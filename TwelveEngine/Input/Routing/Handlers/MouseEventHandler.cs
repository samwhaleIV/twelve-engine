using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input.Routing {
    public sealed class MouseEventHandler:InputEventHandler<MouseEvent,MouseEventRouter> {

        private MouseState? lastState = null;

        public bool CapturingLeft { get; private set; }
        public bool CapturingRight { get; private set; }

        public bool Capturing {
            get {
                return CapturingLeft || CapturingRight;
            }
            private set {
                CapturingLeft = value;
                CapturingRight = value;
            }
        }

        public Point Position { get; private set; }
        public Point Delta { get; private set; }

        public int X => Position.X;
        public int Y => Position.Y;

        public int XDelta => Delta.X;
        public int YDelta => Delta.Y;

        public void Import(MouseEventHandler oldHandler) {
            Position = oldHandler.Position;
            Delta = oldHandler.Delta;
            CapturingLeft = oldHandler.CapturingLeft;
            CapturingRight = oldHandler.CapturingRight;
            lastState = oldHandler.lastState;
        }

        private bool lastUpdateHadInactiveWindow = false;

        private void UpdateLeftClick(MouseState mouseState,MouseState lastState) {
            if(mouseState.LeftButton == lastState.LeftButton) {
                return;
            }
            if(mouseState.LeftButton == ButtonState.Pressed) {
                CapturingLeft = true;
                SendEvent(MouseEvent.LeftClickPress(mouseState.Position));
            } else {
                CapturingLeft = false;
                SendEvent(MouseEvent.LeftClickRelease(mouseState.Position));
            }
        }

        private void UpdateRightClick(MouseState mouseState,MouseState lastState) {
            if(mouseState.RightButton == lastState.RightButton) {
                return;
            }
            if(mouseState.RightButton == ButtonState.Pressed) {
                CapturingRight = true;
                SendEvent(MouseEvent.RightClickPress(mouseState.Position));
            } else {
                CapturingRight = false;
                SendEvent(MouseEvent.RightClickRelease(mouseState.Position));
            }
        }

        public void Update(MouseState mouseState,bool eventsAreAllowed,bool gameIsActive) {
            if(!this.lastState.HasValue) {
                this.lastState = mouseState;
            }
            var lastState = this.lastState.Value;
            Position = mouseState.Position;
            Delta = lastState.Position - Position;
            this.lastState = mouseState;

            if(!eventsAreAllowed) {
                Delta = Point.Zero;
                Capturing = false;
                /* Hint for the mouse button change processor to fire an extra OnPress event so we don't get a release event without a press event */
                lastUpdateHadInactiveWindow = !gameIsActive;
                return;
            }

            SendEvent(MouseEvent.Update(Position));

            bool leftClicking = mouseState.LeftButton == ButtonState.Pressed, rightClicking = mouseState.RightButton == ButtonState.Released;

            if(gameIsActive && lastUpdateHadInactiveWindow && (leftClicking || rightClicking)) {
                if(leftClicking) {
                    CapturingLeft = true;
                    SendEvent(MouseEvent.LeftClickPress(mouseState.Position));
                }
                if(rightClicking) {
                    CapturingRight = true;
                    SendEvent(MouseEvent.RightClickPress(mouseState.Position));
                }
            } else {
                UpdateLeftClick(mouseState,lastState);
                UpdateRightClick(mouseState,lastState);
            }
            lastUpdateHadInactiveWindow = !gameIsActive;

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
