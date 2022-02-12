using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;
using TwelveEngine.Shell.Input;

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
        private bool Capturing => panData.HasValue;

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

        private void UpdatePanData(Point point) {
            PanData newPanData = panData.Value;
            newPanData.Camera = Grid.Camera.Position;
            newPanData.Screen = point;
            panData = newPanData;
        }

        private void Pointer_Scroll(Point point,ScrollDirection scrollDirection) {

            bool useCapturedPosition = Capturing && !entityMover.HasTarget;

            Vector2 zoomTarget;
            if(useCapturedPosition) {
                zoomTarget = panData.Value.World;
            } else {
                zoomTarget = Grid.GetWorldVector(point);
            }

            Vector2 worldCenter = Grid.GetCenter();
            Vector2 distanceToTarget = worldCenter - zoomTarget;

            AdjustCameraZoom(scrollDirection);

            zoomTarget = Grid.GetWorldVector(point);
            worldCenter = Grid.GetCenter();

            Grid.Camera.Position += worldCenter - zoomTarget - distanceToTarget;

            if(Capturing) UpdatePanData(point);
        }

        public override void MouseDown(Point point) {
            if(Capturing) {
                return;
            }
            var world = Grid.GetWorldVector(point);
            entityMover.SearchForTarget(GetState(),world);
            panData = new PanData(world,point,Grid.Camera.Position);
        }

        public override void MouseUp(Point point) {
            if(!Capturing) {
                return;
            }
            if(entityMover.HasTarget) {
                entityMover.ReleaseTarget(GetState(),Grid.GetWorldVector(point));
            }
            panData = null;
        }

        private void PanCamera(Point point) {
            var panData = this.panData.Value;

            var difference = panData.Screen - point;

            var newPosition = panData.Camera + difference.ToVector2() / Grid.ScreenSpace.TileSize;

            Grid.Camera.Position = newPosition;
        }

        public override void MouseMove(Point point) {
            if(!Capturing) {
                return;
            }
            if(entityMover.HasTarget) {
                entityMover.MoveTarget(Grid.GetWorldVector(point));
            } else {
                PanCamera(point);
            }
        }
    }
}
