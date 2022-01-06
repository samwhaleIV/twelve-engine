using Microsoft.Xna.Framework;
using TwelveEngine.Input;

namespace TwelveEngine.Game2D {
    internal sealed class PanZoom {

        private const float MIN_SCALE = 1, MAX_SCALE = 8, ZOOM_RATE = 0.1f;

        private readonly Grid2D grid;
        private readonly Camera camera;

        internal PanZoom(Grid2D grid) {
            this.grid = grid;
            camera = grid.Camera;

            grid.OnUnload += Grid_OnUnload;
            var mouseHandler = grid.Game.MouseHandler;
            mouseHandler.OnMouseMove += pan;
            mouseHandler.OnMouseScroll += zoom;
            mouseHandler.OnMouseDown += refreshPanData;
            mouseHandler.OnMouseUp += clearPanData;
        }

        private void Grid_OnUnload() {
            var mouseHandler = grid.Game.MouseHandler;
            mouseHandler.OnMouseMove -= pan;
            mouseHandler.OnMouseScroll -= zoom;
            mouseHandler.OnMouseDown -= refreshPanData;
            mouseHandler.OnMouseUp -= clearPanData;
        }

        private (Point MousePosition, Vector2 CameraPosition)? panData = null;

        private void refreshPanData(Point mousePosition) => panData = (mousePosition,camera.Position);

        private void clearPanData(Point mousePosition) => panData = null;

        private void zoom(Point mousePosition,ScrollDirection direction) {

            var startPosition = grid.GetCoordinate(mousePosition);
            var zoomInTarget = startPosition;

            var worldCenter = grid.ScreenSpace.GetCenter();

            Vector2 distanceToTarget = worldCenter - zoomInTarget;

            float scaleChange = 1 + (int)direction * ZOOM_RATE;
            float startScale = camera.Scale;
            float newScale = startScale;

            newScale *= scaleChange;
            if(newScale < MIN_SCALE) {
                newScale = MIN_SCALE;
            } else if(newScale > MAX_SCALE) {
                newScale = MAX_SCALE;
            }
            camera.Scale = newScale;

            var newScreenSpace = grid.ScreenSpace;
            zoomInTarget = grid.GetCoordinate(newScreenSpace,mousePosition);
            worldCenter = newScreenSpace.GetCenter();

            Vector2 newDistanceToTarget = worldCenter - zoomInTarget;

            camera.Position += newDistanceToTarget - distanceToTarget;
            if(panData.HasValue) {
                refreshPanData(mousePosition);
            }
        }
        private void pan(Point mousePosition) {
            if(!this.panData.HasValue) {
                return;
            }

            var panData = this.panData.Value;
            Point difference = panData.MousePosition - mousePosition;

            float tileSize = grid.GetScreenSpace().TileSize;

            camera.Position = panData.CameraPosition + difference.ToVector2() / tileSize;
            refreshPanData(mousePosition);
        }
    }
}
