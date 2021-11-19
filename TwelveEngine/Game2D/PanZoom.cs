using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Game2D {
    internal sealed class PanZoom:IUpdateable {

        private const float MIN_SCALE = 1;
        private const float MAX_SCALE = 8;
        private const float ZOOM_RATE = 0.1f;

        private readonly Grid2D grid;
        private readonly Camera camera;

        internal PanZoom(Grid2D grid) {
            this.grid = grid;
            this.camera = grid.Camera;
        }

        private (int x, int y, float cameraX, float cameraY)? panData = null;
        private void refreshPanData() {
            panData = (mouseX, mouseY, camera.X, camera.Y);
        }

        private void zoom(bool scrollingUp,int x,int y) {
            var startPosition = grid.GetCoordinate(x,y);
            var zoomInTarget = startPosition;

            var worldCenter = grid.ScreenSpace.getCenter();

            (float x, float y) distanceToTarget = (
                worldCenter.x - zoomInTarget.x,
                worldCenter.y - zoomInTarget.y
            );

            float scaleChange = 1 + (scrollingUp ? ZOOM_RATE : -ZOOM_RATE);
            float startScale = camera.Scale;
            float newScale = startScale;

            newScale *= scaleChange;
            if(newScale < MIN_SCALE) {
                newScale = MIN_SCALE;
            } else if(newScale > MAX_SCALE) {
                newScale = MAX_SCALE;
            }
            camera.Scale = newScale;

            var newScreenSpace = grid.GetScreenSpace();
            zoomInTarget = grid.GetCoordinate(newScreenSpace,x,y);
            worldCenter = newScreenSpace.getCenter();

            (float x, float y) newDistanceToTarget = (
                worldCenter.x - zoomInTarget.x,
                worldCenter.y - zoomInTarget.y
            );

            camera.X += newDistanceToTarget.x - distanceToTarget.x;
            camera.Y += newDistanceToTarget.y - distanceToTarget.y;

            if(panData.HasValue) {
                refreshPanData();
            }
        }
        private void pan(int x,int y) {
            if(!this.panData.HasValue) {
                return;
            }
            var panData = this.panData.Value;
            int xDifference = panData.x - x;
            int yDifference = panData.y - y;

            float tileSize = grid.GetScreenSpace().TileSize;
            camera.X = panData.cameraX + xDifference / tileSize;
            camera.Y = panData.cameraY + yDifference / tileSize;
            refreshPanData();
        }

        private void mouseMove() {
            pan(mouseX,mouseY);
        }
        private void mouseDown() {
            refreshPanData();
        }
        private void mouseUp() {
            panData = null;
        }
        private void scrollUp() {
            zoom(true,mouseX,mouseY);
        }
        private void scrollDown() {
            zoom(false,mouseX,mouseY);
        }

        MouseState lastMouseState;
        bool hasState = false;
        private bool mouseIsDown = false;
        int mouseX, mouseY;

        public void Update(GameTime gameTime) {
            var mouseState = Mouse.GetState();
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
                scrollUp();
            } else if(scrollDifference < 0) {
                scrollDown();
            }
            lastMouseState = mouseState;
        }
    }
}
