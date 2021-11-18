using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Game2D {
    internal sealed class PanZoom:IUpdateable {

        private readonly Grid2D grid;
        private readonly Camera camera;

        internal PanZoom(Grid2D grid) {
            this.grid = grid;
            this.camera = grid.Camera;
        }

        private (int mx,int my,float camX,float camY)? startPosition = null;
        private (int mx, int my, bool pressed) mouseState;

        private void startMouseCapture() {
            startPosition = (mouseState.mx, mouseState.my,camera.X,camera.Y);
        }

        private (float x,float y) getCameraPosition() {
            var differenceX = (startPosition.Value.mx - mouseState.mx) / (camera.Scale * grid.TileSize);
            var differenceY = (startPosition.Value.my - mouseState.my) / (camera.Scale * grid.TileSize);

            return (startPosition.Value.camX + differenceX,
                startPosition.Value.camY + differenceY);
        }

        private void updateCameraPosition() {
            var (x,y) = getCameraPosition();
            camera.X = x;
            camera.Y = y;
        }

        private void endMouseCapture() {
            updateCameraPosition();
            startPosition = null;
        }
        private void updateMouseCapture() {
            updateCameraPosition();
        }

        private bool capturing => startPosition.HasValue;

        private void updateMouseState() {
            var mouseState = Mouse.GetState();
            this.mouseState.mx = mouseState.X;
            this.mouseState.my = mouseState.Y;
            this.mouseState.pressed = mouseState.LeftButton == ButtonState.Pressed;
        }

        public void Update(GameTime gameTime) {
            updateMouseState();
            if(mouseState.pressed) {
                if(capturing) {
                    updateMouseCapture();
                } else {
                    startMouseCapture();
                }
            } else if(capturing) {
                endMouseCapture();
            }
        }
    }
}
