using System;
using Microsoft.Xna.Framework;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game2D.Entity;
using TwelveEngine.Game2D.Objects;
using TwelveEngine.Serial;
using TwelveEngine.Shell.States;

namespace TwelveEngine.Game2D {
    public abstract class Grid2D:InputGameState {

        public Grid2D() {
            Camera = new Camera();

            OnLoad += Grid2D_OnLoad;

            OnImport += Grid2D_OnImport;
            OnExport += Grid2D_OnExport;

            OnRender += Grid2D_OnRender;
            OnUpdate += Grid2D_OnUpdate;
        }

        public Vector2 Size { get; protected set; } = Vector2.Zero;

        public float Width => Size.X;
        public float Height => Size.Y;

        public int UnitWidth => (int)Size.X;
        public int UnitHeight => (int)Size.Y;

        public bool UnitInRange(Point point) => new Rectangle(Point.Zero,Size.ToPoint()).Contains(point);

        public Rectangle UnitArea => new Rectangle(Point.Zero,Size.ToPoint());

        protected abstract void RenderGrid(GameTime gameTime);

        public Color BackgroundColor { get; set; } = Color.Black;

        private void Grid2D_OnRender(GameTime gameTime) {
            Game.GraphicsDevice.Clear(BackgroundColor);
            ScreenSpace = GetScreenSpace();
            RenderGrid(gameTime);
        }

        private Camera _camera = null;

        public ScreenSpace ScreenSpace { get; private set; }

        public int UnitSize { get; set; } = Constants.Config.TileSize;

        public EntityFactory<Entity2D,Grid2D> EntityFactory { get; set; }
        public EntityManager<Entity2D,Grid2D> Entities { get; private set; }

        public Camera Camera {
            get => _camera;
            set {
                if(value == null) {
                    throw new ArgumentNullException("value");
                }
                var oldCamera = _camera;
                if(oldCamera == value) {
                    return;
                }
                if(oldCamera != null) {
                    oldCamera.Invalidated -= Camera_Invalidated;
                }
                value.Invalidated += Camera_Invalidated;
                _camera = value;
            }
        }

        private void Grid2D_OnLoad() {
            Entities = new EntityManager<Entity2D,Grid2D>(this,EntityFactory);
        }

        private static float CameraBounds(float value,float size,float length) {
            //todo disable overflow axis when size of screen space width is larger than the width of the map
            if(value < 0f) {
                return 0f;
            }
            if(value + size > length) {
                return length - size;
            }
            return value;
        }

        protected void RenderEntities(GameTime gameTime) {
            Entities.IterateImmutable(Entity2D.Render,gameTime);
        }

        public int CalculateTileSize() {
            int tileSize = (int)Math.Round(Camera.Scale * UnitSize);
            if(tileSize % 2 == 1) {
                tileSize++;
            }
            return tileSize;
        }

        public Vector2 GetCenter() => ScreenSpace.GetCenter();

        public Rectangle GetDestination(Vector2 position,Vector2 size) {
            int tileSize = ScreenSpace.TileSize;

            var destination = new Rectangle {
                X = (int)Math.Floor((position.X - ScreenSpace.X) * tileSize),
                Y = (int)Math.Floor((position.Y - ScreenSpace.Y) * tileSize),

                Width = (int)Math.Floor(size.X * tileSize),
                Height = (int)Math.Floor(size.Y * tileSize)
            };

            return destination;
        }

        public bool OnScreen(Vector2 position,Vector2 size) {
            //Todo: Account for rotation
            return !(
                position.X + size.X <= ScreenSpace.X ||
                position.Y + size.Y <= ScreenSpace.Y ||
                position.X >= ScreenSpace.X + ScreenSpace.Width ||
                position.Y >= ScreenSpace.Y + ScreenSpace.Height
            );
        }

        public bool OnScreen(Entity2D entity) {
            return OnScreen(entity.Position,entity.Size);
        }

        public bool OnScreen(GameObject gameObject) {
            return OnScreen(gameObject.Position,gameObject.Size);
        }

        public Rectangle GetDestination(Entity2D entity) {
            return GetDestination(entity.Position,entity.Size);
        }

        public Rectangle GetDestination(GameObject gameObject) {
            return GetDestination(gameObject.Position,gameObject.Size);
        }

        public float GetRenderDepth(int destinationY) {
            return 1f - Math.Max(destinationY / (float)Game.Viewport.Height,0f);
        }

        private ScreenSpace GetScreenSpace() {
            int tileSize = CalculateTileSize();

            Vector2 size = Game.Viewport.Bounds.Size.ToVector2() / tileSize;
            Vector2 position = Camera.Position + Camera.Offset - (size * 0.5f) + new Vector2(0.5f);

            if(Camera.HorizontalPadding) {
                position.X = CameraBounds(position.X,size.X,Width);
            }
            if(Camera.VerticalPadding) {
                position.Y = CameraBounds(position.Y,size.Y,Height);
            }

            return new ScreenSpace(position,size,tileSize);
        }

        public Vector2 GetWorldVector(Point screenLocation) {
            return ScreenSpace.Location + screenLocation.ToVector2() / Game.Viewport.Bounds.Size.ToVector2() * ScreenSpace.Size;
        }

        public Point GetScreenPoint(Vector2 worldLocation) {
            return ((worldLocation - ScreenSpace.Location) * ScreenSpace.TileSize).ToPoint();
        }

        private void Camera_Invalidated() {
            if(IsRendering) {
                throw new InvalidOperationException("Cannot invalidate the camera during the render cycle!");
            }
            if(IsUpdating) {
                ScreenSpace = GetScreenSpace();
            }
        }

        private void Grid2D_OnUpdate(GameTime gameTime) {
            ScreenSpace = GetScreenSpace();
            UpdateInputs(gameTime);
            Entities.IterateMutable(Entity2D.Update,gameTime);
        }

        private void Grid2D_OnExport(SerialFrame frame) {
            frame.Set(Camera);
            frame.Set(BackgroundColor);
        }

        private void Grid2D_OnImport(SerialFrame frame) {
            frame.Get(Camera);
            BackgroundColor = frame.GetColor();
        }
    }
}
