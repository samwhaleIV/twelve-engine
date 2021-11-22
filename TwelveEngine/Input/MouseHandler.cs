using System;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {
    internal sealed class MouseHandler {

        public Action<int,int> MouseMove { get; set; }
        public Action<int,int> MouseDown { get; set; }
        public Action<int,int> MouseUp { get; set; }
        public Action<int,int,bool> Scroll { get; set; }

        private void mouseMove() {
            MouseMove?.Invoke(mouseX,mouseY);
        }
        private void mouseDown() {
            MouseDown?.Invoke(mouseX,mouseY);
        }
        private void mouseUp() {
            MouseUp?.Invoke(mouseX,mouseY);
        }
        private void scroll(bool scrollUp) {
            Scroll?.Invoke(mouseX,mouseY,scrollUp);
        }

        MouseState lastMouseState;
        bool hasState = false;
        private bool mouseIsDown = false;
        int mouseX, mouseY;

        public int X => mouseX;
        public int Y => mouseY;
        public bool Capturing => mouseIsDown;

        public void Update(GameManager gameManager) {
            var mouseState = gameManager.MouseState;
            if(!hasState) {
                lastMouseState = mouseState;
                hasState = true;
            }
            if(mouseState.LeftButton != lastMouseState.LeftButton) {
                if(mouseState.LeftButton == ButtonState.Pressed) {
                    mouseIsDown = true;
                    mouseDown();
                } else {
                    mouseIsDown = false;
                    mouseUp();
                }
            }

            mouseX = mouseState.X;
            mouseY = mouseState.Y;
            if(mouseX != lastMouseState.X || mouseY != lastMouseState.Y) {
                mouseMove();
            }
            int scrollDifference = mouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue;
            if(scrollDifference > 0) {
                scroll(true);
            } else if(scrollDifference < 0) {
                scroll(false);
            }
            lastMouseState = mouseState;
        }
    }
}
