using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Serial.Map;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game2D.Entity;
using TwelveEngine.Serial;
using TwelveEngine.Shell.States;
using TwelveEngine.Shell.UI;
using TwelveEngine.Game2D.Collision;

namespace TwelveEngine.Game2D {
    public class Grid2D:InputGameState {

        public Grid2D(EntityFactory<Entity2D,Grid2D> entityFactory = null,int? tileSize = null) {

            EntityFactory = entityFactory ?? Entity2DType.GetFactory();
            if(tileSize.HasValue) TileSize = tileSize.Value;

            collisionInterface = new CollisionInterface(this);
            Camera = new Camera();

            OnLoad += Grid2D_OnLoad;
            OnUnload += Grid2D_OnUnload;

            OnImport += Grid2D_OnImport;
            OnExport += Grid2D_OnExport;

            OnRender += Grid2D_OnRender;
            OnUpdate += Grid2D_OnUpdate;
        }

        private readonly CollisionInterface collisionInterface;
        private CollisionTypes _collisionTypes;

        public CollisionTypes CollisionTypes {
            get => _collisionTypes;
            set {
                if(_collisionTypes == value) return;
                if(IsLoaded && !value.IsLoaded) value.Load();
                _collisionTypes = value;
            }
        }

        public CollisionInterface Collision => collisionInterface;
        

        private Camera _camera = null;
        private int[][,] layers;

        public LayerMode LayerMode { get; set; } = LayerModes.Default;
        public Color BackgroundColor { get; set; } = Color.Black;

        public ScreenSpace ScreenSpace { get; private set; }
        public Point Size { get; private set; } = Point.Zero;

        public EntityFactory<Entity2D,Grid2D> EntityFactory { get; private set; }
        public EntityManager<Entity2D,Grid2D> Entities { get; private set; }

        public int TileSize { get; private set; } = Constants.Config.TileSize;

        public TileRenderer TileRenderer { get; set; }

        public int Columns => Size.X;
        public int Rows => Size.Y;

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

        public bool TileInRange(Point tile) => new Rectangle(Point.Zero,Size).Contains(tile);
        public Point GetTile(Vector2 location) => Vector2.Floor(location).ToPoint();
        public Point GetTile(Point screenPoint) => GetTile(GetWorldVector(screenPoint));

        public bool TryGetTile(Point screenPoint,out Point tile) {
            tile = GetTile(screenPoint);
            return TileInRange(tile);
        }

        public bool TryGetTile(Vector2 location,out Point tile) {
            tile = GetTile(location);
            return TileInRange(tile);
        }

        public void ImportMap(Map map) {
            layers = map.Layers2D;
            Size = new Point(map.Width,map.Height);
        }

        public void Fill(int layerIndex,Func<int,int,int> pattern) {
            var map2D = layers[layerIndex];
            for(var x = 0;x < Columns;x++) {
                for(var y = 0;y < Rows;y++) {
                    map2D[x,y] = pattern(x,y);
                }
            }
        }

        public int[,] GetLayer(int index) => layers[index];

        private void Grid2D_OnLoad() {
            CollisionTypes?.Load();
            Entities = new EntityManager<Entity2D,Grid2D>(this,EntityFactory);
            TileRenderer?.Load(this);
        }

        private void Grid2D_OnUnload() {
            TileRenderer?.Unload();
        }

        private void RenderEntities(GameTime gameTime) {
            Entities.IterateImmutable(Entity2D.Render,gameTime);
        }

        private static float CameraBounds(float value,float size,int gridSize) {
            if(value < 0f) {
                return 0f;
            }
            if(value + size > gridSize) {
                return gridSize - size;
            }
            return value;
        }

        public int CalculateTileSize() {
            int tileSize = (int)Math.Round(Camera.Scale * TileSize);
            if(tileSize % 2 == 1) {
                tileSize++;
            }
            return tileSize;
        }

        public Vector2 GetCenter() => ScreenSpace.GetCenter();

        private ScreenSpace GetScreenSpace() {
            int tileSize = CalculateTileSize();

            Vector2 size = Game.Viewport.Bounds.Size.ToVector2() / tileSize;
            Vector2 position = Camera.Position + Camera.Offset - (size * 0.5f) + new Vector2(0.5f);

            if(Camera.HorizontalPadding) {
                position.X = CameraBounds(position.X,size.X,Columns);
            }
            if(Camera.VerticalPadding) {
                position.Y = CameraBounds(position.Y,size.Y,Rows);
            }

            return new ScreenSpace(position,size,tileSize);
        }

        public Vector2 GetWorldVector(Point screenLocation) {
            return ScreenSpace.Location + screenLocation.ToVector2() / Game.Viewport.Bounds.Size.ToVector2() * ScreenSpace.Size;
        }

        public Point GetScreenPoint(Vector2 worldLocation) {
            return ((worldLocation - ScreenSpace.Location) * ScreenSpace.TileSize).ToPoint();
        }

        private void RenderLayers(int start,int length) {
            if(TileRenderer == null) {
                return;
            }
            int end = start + length;
            for(int i = start;i<end;i++) {
                TileRenderer.RenderTiles(layers[i]);
            }
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

        private void Grid2D_OnRender(GameTime gameTime) {
            ScreenSpace = GetScreenSpace();
            TileRenderer.CacheArea(ScreenSpace);

            Game.GraphicsDevice.Clear(BackgroundColor);
            SpriteBatch.SamplerState = SamplerState.PointClamp;
            
            if(LayerMode.Background) {
                if(LayerMode.BackgroundLength == 2) {
                    SpriteBatch.Begin();
                    RenderLayers(LayerMode.BackgroundStart,1);
                    SpriteBatch.End();

                    SpriteBatch.Begin(SpriteSortMode.BackToFront);
                    RenderLayers(LayerMode.BackgroundStart+1,1);
                    RenderEntities(gameTime);
                    SpriteBatch.End();
                } else {
                    SpriteBatch.Begin();
                    RenderLayers(LayerMode.BackgroundStart,LayerMode.BackgroundLength);
                    RenderEntities(gameTime);
                }
            }

            SpriteBatch.TryBegin();
            if(LayerMode.Foreground) {
                RenderLayers(LayerMode.ForegroundStart,LayerMode.ForegroundLength);
            }
            InputGuide.Render();
            SpriteBatch.End();
        }

        private void Grid2D_OnExport(SerialFrame frame) {
            frame.Set(BackgroundColor);
            frame.Set(Camera);
            frame.Set(LayerMode);
            frame.Set(Size);
            frame.Set(layers.Length);
            for(var i = 0;i<layers.Length;i++) {
                frame.Set(layers[i]);
            }
        }

        private void Grid2D_OnImport(SerialFrame frame) {
            BackgroundColor = frame.GetColor();
            frame.Get(Camera);
            frame.Get(LayerMode);
            Size = frame.GetPoint();
            var layerCount = frame.GetInt();
            var layers = new int[layerCount][,];
            for(var i = 0;i<layerCount;i++) {
                layers[i] = frame.GetIntArray2D();
            }
            this.layers = layers;
        }
    }
}
