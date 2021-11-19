using Microsoft.Xna.Framework;
using TwelveEngine.Input;

namespace TwelveEngine.Game2D {
    internal sealed class PanZoom:IUpdateable {

        private const float MIN_SCALE = 1;
        private const float MAX_SCALE = 8;
        private const float ZOOM_RATE = 0.1f;

        private readonly Grid2D grid;
        private readonly Camera camera;

        private readonly MouseHandler mouseHandler;

        internal PanZoom(Grid2D grid) {
            this.grid = grid;
            this.camera = grid.Camera;

            mouseHandler = new MouseHandler() {
                MouseMove = pan,
                Scroll = zoom,
                MouseDown = refreshPanData,
                MouseUp = clearPanData
            };
        }

        private (int x, int y, float cameraX, float cameraY)? panData = null;
        private void refreshPanData(int x,int y) {
            panData = (x, y, camera.X, camera.Y);
        }
        private void clearPanData(int x,int y) {
            panData = null;
        }

        private void zoom(int x,int y,bool scrollingUp) {
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
                refreshPanData(x,y);
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
            refreshPanData(x,y);
        }

        public void Update(GameTime gameTime) {
            mouseHandler.Update();
        }
    }
}
