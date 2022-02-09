using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;
using TwelveEngine.Input;

namespace JewelEditor {
    internal sealed class MouseController {

        private readonly Grid2D grid;
        private readonly EntityController entityController;

        public MouseController(Grid2D grid) {
            this.grid = grid;
            entityController = new EntityController(grid.Entities);
        }

        private bool Capturing { get; set; } = false;

        private Vector2 TranslatePoint(Point point) => grid.GetWorldVector(point);

        private Point? StartPoint = null;
        private Vector2? CamStart = null;

        public float MaxZoom { get; set; } = 10f;
        public float MinZoom { get; set; } = 2f;
        public float ZoomRate { get; set; } = 0.25f;

        private void AdjustCameraZoom(ScrollDirection direction) {
            float scaleChange = 1 + -(int)direction * ZoomRate;
            float startScale = grid.Camera.Scale;
            float newScale = startScale;

            newScale *= scaleChange;
            if(newScale < MinZoom) {
                newScale = MinZoom;
            } else if(newScale > MaxZoom) {
                newScale = MaxZoom;
            }
            grid.Camera.Scale = newScale;
        }

        public void Scroll(Point point,ScrollDirection direction) {

            Vector2 startPosition = grid.GetWorldVector(point);
            Vector2 zoomInTarget = startPosition;

            Vector2 worldCenter = grid.ScreenSpace.GetCenter();
            Vector2 distanceToTarget = worldCenter - zoomInTarget;

            AdjustCameraZoom(direction);

            ScreenSpace newScreenSpace = grid.GetScreenSpace();
            zoomInTarget = grid.GetWorldVector(newScreenSpace,point);
            worldCenter = newScreenSpace.GetCenter();

            Vector2 newDistanceToTarget = worldCenter - zoomInTarget;

            grid.Camera.Position += newDistanceToTarget - distanceToTarget;
            if(!Capturing) {
                return;
            }

            StartPoint = point;
            CamStart = grid.Camera.Position;

            if(!entityController.HasTarget) {
                return;
            }

            newScreenSpace = grid.GetScreenSpace();
            zoomInTarget = grid.GetWorldVector(newScreenSpace,point);
            entityController.MoveTarget(zoomInTarget);
        }

        public void MouseDown(Point point) {
            if(Capturing) {
                return;
            }
            Capturing = true;
            entityController.SearchForTarget(TranslatePoint(point));

            StartPoint = point;
            CamStart = grid.Camera.Position;
        }

        public void MouseUp(Point point) {
            if(!Capturing) {
                return;
            }
            if(entityController.HasTarget) {
                entityController.ReleaseTarget(TranslatePoint(point));
            }
            StartPoint = null;
            CamStart = null;

            Capturing = false;
        }

        private void PanCamera(Point point) {
            var difference = StartPoint.Value - point;
            grid.Camera.Position = CamStart.Value + difference.ToVector2() / grid.ScreenSpace.TileSize;
        }

        public void MouseMove(Point point) {
            if(!Capturing) {
                return;
            }
            if(entityController.HasTarget) {
                entityController.MoveTarget(TranslatePoint(point));
            } else {
                PanCamera(point);
            }
        }
    }
}
