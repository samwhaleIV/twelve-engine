using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game2D.Entity;
using TwelveEngine.Game2D.Objects;
using TwelveEngine.Shell.States;

namespace TwelveEngine.Game2D {
    public abstract class Grid2D:InputGameState {

        public Grid2D() {
            Camera = new Camera();

            OnLoad += Grid2D_OnLoad;
            OnUnload += Grid2D_OnUnload;

            OnRender += Grid2D_OnRender;
            OnPreRender += Grid2D_OnPreRender;
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


        private Camera _camera = null;

        public ScreenSpace ScreenSpace { get; private set; }

        public int UnitSize { get; set; } = Constants.Config.TileSize;

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

        public Texture2D BlankTexture { get; private set; }

        private void Grid2D_OnLoad() {
            Entities = new EntityManager<Entity2D,Grid2D>(this);
            BlankTexture = new Texture2D(Game.GraphicsDevice,1,1);
            BlankTexture.SetData(new Color[] { Color.White });
        }

        private void Grid2D_OnUnload() => BlankTexture?.Dispose();

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
            Entities.Render(gameTime);
        }

        public int CalculateTileSize() {
            int tileSize = (int)Math.Round(Camera.Scale * UnitSize);
            if(tileSize % 2 == 1) {
                tileSize++;
            }
            return tileSize;
        }

        public Vector2 GetCenter() => ScreenSpace.GetCenter();

        public VectorRectangle GetDestination(Vector2 position,Vector2 worldSize,Vector2 textureSize) {
            float tileSize = ScreenSpace.TileSize;
            Vector2 location = (position - ScreenSpace.Location) * tileSize;
            return new VectorRectangle(location,worldSize / textureSize * tileSize);
        }

        public VectorRectangle GetDestination(Vector2 position,Vector2 worldSize,Point textureSize) {
            return GetDestination(position,worldSize,textureSize.ToVector2());
        }

        private bool OnScreen(Vector2 position,Vector2 size) {
            return position.X < ScreenSpace.X + ScreenSpace.Width &&
                     ScreenSpace.X < position.X + size.X &&
                     position.Y < ScreenSpace.Y + ScreenSpace.Height &&
                     ScreenSpace.Y < position.Y + size.Y;
        }

        public bool OnScreen(Entity2D entity) {
            return OnScreen(entity.Position,entity.Size);
        }

        private const float CORNER_RADIUS_MARGIN = 0.20703125f;

        public bool OnScreen(GameObject gameObject) {
            Vector2 margin = gameObject.Size * CORNER_RADIUS_MARGIN;
            return OnScreen(gameObject.Position - margin,gameObject.Size + margin * 2);
        }

        public VectorRectangle GetDestination(Entity2D entity,Point textureSize) {
            return GetDestination(entity.Position,entity.Size,textureSize.ToVector2());
        }

        public VectorRectangle GetDestination(GameObject gameObject,Point textureSize) {
            return GetDestination(gameObject.Position,gameObject.Size,textureSize.ToVector2());
        }

        public VectorRectangle GetDestination(Entity2D entity,Vector2 textureSize) {
            return GetDestination(entity.Position,entity.Size,textureSize);
        }

        public VectorRectangle GetDestination(GameObject gameObject,Vector2 textureSize) {
            return GetDestination(gameObject.Position,gameObject.Size,textureSize);
        }

        public float GetRenderDepth(float destinationY) {
            float depth = 1f - Math.Max(destinationY / Game.Viewport.Height,0f);
            if(depth > 1f) {
                depth = 1f;
            } else if(depth < 0f) {
                depth = 0f;
            }
            return depth;
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
            UpdateUI(gameTime);
            UpdateInputs(gameTime);
            Entities.Update(gameTime);
        }

        private void Grid2D_OnPreRender(GameTime gameTime) {
            Entities.PreRender(gameTime);
        }

        private void Grid2D_OnRender(GameTime gameTime) {
            Game.GraphicsDevice.Clear(BackgroundColor);
            ScreenSpace = GetScreenSpace();
            RenderGrid(gameTime);
        }
    }
}
