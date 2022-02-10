using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;
using TwelveEngine.Input;

namespace JewelEditor.InputContext.MouseHandlers {
    internal sealed class Pointer:MouseHandler {

        private readonly EntityMover entityMover;

        public Pointer(Grid2D grid):base(grid) {
            entityMover = new EntityMover(grid.Entities);
        }

        private bool Capturing { get; set; } = false;

        private Point? StartPoint = null;
        private Vector2? CamStart = null;

        public float MaxZoom { get; set; } = 10f;
        public float MinZoom { get; set; } = 2f;
        public float ZoomRate { get; set; } = 0.25f;

        private void AdjustCameraZoom(ScrollDirection direction) {
            float scaleChange = 1 + -(int)direction * ZoomRate;
            float startScale = Grid.Camera.Scale;
            float newScale = startScale;

            newScale *= scaleChange;
            if(newScale < MinZoom) {
                newScale = MinZoom;
            } else if(newScale > MaxZoom) {
                newScale = MaxZoom;
            }
            Grid.Camera.Scale = newScale;
        }

        public override void Scroll(Point point,ScrollDirection direction) {

            Vector2 startPosition = Grid.GetWorldVector(point);
            Vector2 zoomInTarget = startPosition;

            Vector2 worldCenter = Grid.ScreenSpace.GetCenter();
            Vector2 distanceToTarget = worldCenter - zoomInTarget;

            AdjustCameraZoom(direction);

            ScreenSpace newScreenSpace = Grid.GetScreenSpace();
            zoomInTarget = Grid.GetWorldVector(newScreenSpace,point);
            worldCenter = newScreenSpace.GetCenter();

            Vector2 newDistanceToTarget = worldCenter - zoomInTarget;

            Grid.Camera.Position += newDistanceToTarget - distanceToTarget;
            if(!Capturing) {
                return;
            }

            StartPoint = point;
            CamStart = Grid.Camera.Position;

            if(!entityMover.HasTarget) {
                return;
            }

            newScreenSpace = Grid.GetScreenSpace();
            zoomInTarget = Grid.GetWorldVector(newScreenSpace,point);
            entityMover.MoveTarget(zoomInTarget);
        }

        public override void MouseDown(Point point) {
            if(Capturing) {
                return;
            }
            Capturing = true;
            entityMover.SearchForTarget(TranslatePoint(point));

            StartPoint = point;
            CamStart = Grid.Camera.Position;
        }

        public override void MouseUp(Point point) {
            if(!Capturing) {
                return;
            }
            if(entityMover.HasTarget) {
                entityMover.ReleaseTarget(TranslatePoint(point));
            }
            StartPoint = null;
            CamStart = null;

            Capturing = false;
        }

        private void PanCamera(Point point) {
            var difference = StartPoint.Value - point;
            Grid.Camera.Position = CamStart.Value + difference.ToVector2() / Grid.ScreenSpace.TileSize;
        }

        public override void MouseMove(Point point) {
            if(!Capturing) {
                return;
            }
            if(entityMover.HasTarget) {
                entityMover.MoveTarget(TranslatePoint(point));
            } else {
                PanCamera(point);
            }
        }
    }
}
