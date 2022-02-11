using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;
using TwelveEngine.Input;

namespace JewelEditor.InputContext.MouseHandlers {
    internal sealed class Pointer:MouseHandler {

        private readonly EntityMover entityMover;

        public Pointer(Grid2D grid):base(grid) {
            entityMover = new EntityMover(grid.Entities);
            OnScroll += Pointer_Scroll;
        }

        private struct PanData {
            public PanData(Vector2 world,Point screen,Vector2 camera) {
                World = world;
                Screen = screen;
                Camera = camera;
            }
            public readonly Vector2 World;
            public Point Screen;
            public Vector2 Camera;
        }

        private PanData? panData = null;
        private bool Capturing => panData != null;

        public float MaxZoom { get; set; } = 10f;
        public float MinZoom { get; set; } = 2f;
        public float ZoomRate { get; set; } = 0.2f;

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

        private void Pointer_Scroll(Point point,ScrollDirection direction) {

            var panData = this.panData;

            Vector2 startPosition = Capturing && !entityMover.HasTarget ? panData.Value.World : Grid.GetWorldVector(point);
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

            var newPanData = panData.Value;
            newPanData.Camera = Grid.Camera.Position;
            newPanData.Screen = point;
            this.panData = newPanData;
        }

        public override void MouseDown(Point point) {
            if(Capturing) {
                return;
            }
            var world = TranslatePoint(point);
            entityMover.SearchForTarget(State,world);
            panData = new PanData(world,point,Grid.Camera.Position);
        }

        public override void MouseUp(Point point) {
            if(!Capturing) {
                return;
            }
            if(entityMover.HasTarget) {
                entityMover.ReleaseTarget(State,TranslatePoint(point));
            }
            panData = null;
        }

        private void PanCamera(Point point) {
            var panData = this.panData.Value;

            var difference = panData.Screen - point;
            Grid.Camera.Position = panData.Camera + difference.ToVector2() / Grid.ScreenSpace.TileSize;
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
