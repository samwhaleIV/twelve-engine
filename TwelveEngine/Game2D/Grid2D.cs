using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D {
    public class Grid2D:GameState {

        private readonly int tileSize;
        public LayerMode LayerMode;

        public Grid2D() {
            this.tileSize = Constants.DefaultTileSize;
            this.LayerMode = LayerModes.Default;
        }

        public Grid2D(int tileSize) {
            this.tileSize = tileSize;
            this.LayerMode = LayerModes.Default;
        }

        public Grid2D(LayerMode layerMode) {
            this.tileSize = Constants.DefaultTileSize;
            LayerMode = layerMode;
        }

        public Grid2D(int tileSize,LayerMode layerMode) {
            this.tileSize = tileSize;
            this.LayerMode = layerMode;
        }

        private readonly List<GridLayer> layers = new List<GridLayer>();

        public GridLayer GetLayer(int index) {
            return layers[index];
        }
        public GridLayer CreateLayer(int[,] data,ITileRenderer tileRenderer) {
            return CreateLayer(new TrackedGrid(data),tileRenderer);
        }
        public GridLayer CreateLayer(TrackedGrid grid,ITileRenderer tileRenderer) {
            var layer = new GridLayer(this.game,tileRenderer,tileSize) {
                Grid = grid
            };
            return layer;
        }

        public void ClearLayers() {
            unloadLayers();
            this.layers.Clear();
        }

        public void SetLayers(GridLayer[] layers) {
            ClearLayers();
            if(layers.Length > 0) {
                this.layers.AddRange(layers);
                loadLayers();
            }
        }
        public void ReplaceLayer(GridLayer layer,int index) {
            var oldLayer = layers[index];
            oldLayer.Unload();
            layers[index] = layer;
            layer.Load(game,this);
        }
        private void loadLayers() {
            foreach(var layer in layers) {
                layer.Load(game,this);
            }
        }

        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;

        private GameManager game;
        public Camera Camera { get; set; } = new Camera();
        public Action OnLoad { get; set; } = null;

        internal override void Load(GameManager game) {
            this.game = game;

            this.entityManager = new EntityManager(this);
            entityManager.UpdateListChanged = updateUpdateables;
            entityManager.RenderListChanged = updateRenderables;

            if(OnLoad != null) {
                OnLoad();
                OnLoad = null;
            }
        }

        private void unloadLayers() {
            foreach(var layer in layers) {
                layer.Unload();
            }
        }

        internal override void Unload() {
            unloadLayers();
            entityManager.Unload();
        }

        private EntityManager entityManager = null;
        public EntityManager EntityManager => entityManager;

        public void AddEntity(Entity entity) {
            EntityManager.AddEntity(entity);
        }

        private IUpdateable[] updateables = new IUpdateable[0];
        private IRenderable[] renderables = new IRenderable[0];

        private void updateUpdateables(IUpdateable[] updateables) {
            this.updateables = updateables;
        }
        private void updateRenderables(IRenderable[] renderables) {
            this.renderables = renderables;
        }

        internal override void Update(GameTime gameTime) {
            for(var i = 0;i<updateables.Length;i++) {
                updateables[i].Update(gameTime);
            }
        }

        private static float zeroMinMax(float value,float width,int gridWidth) {
            if(value < 0f) return 0f;
            if(value + width >= gridWidth) return gridWidth - width;
            return value;
        }

        private ScreenSpace getScreenSpace(Viewport viewport) {
            float tileSize = Camera.Scale * this.tileSize;

            float screenWidth = viewport.Width / tileSize;
            float screenHeight = viewport.Height / tileSize;

            float halfScreenWidth = screenWidth * 0.5f;
            float halfScreenHeight = screenHeight * 0.5f;

            float x = Camera.X - halfScreenWidth + Camera.XOffset;
            float y = Camera.Y - halfScreenHeight + Camera.YOffset;

            if(Camera.HorizontalEdgePadding) x = zeroMinMax(x,screenWidth,Width);
            if(Camera.VerticalEdgePadding) y = zeroMinMax(y,screenHeight,Height);

            return new ScreenSpace() {
                X = x, Y = y, Width = screenWidth, Height = screenHeight, TileSize = tileSize
            };
        }

        public (float x,float y) GetCoordinate(int screenX,int screenY) {
            var viewport = Graphics.GetViewport(game);
            var screenSpace = getScreenSpace(viewport);
            float x = (float)screenX / viewport.Width * screenSpace.Width;
            float y = (float)screenY / viewport.Height * screenSpace.Height;
            return (x + screenSpace.X, y + screenSpace.Y);
        }

        private void renderEntities(GameTime gameTime) {
            for(var i = 0;i < renderables.Length;i++) {
                renderables[i].Render(gameTime);
            }
        }

        private void updateLayers() {
            foreach(var layer in layers) {
                layer.UpdateBuffer();
            }
        }

        private Rectangle rasterizeScreenSpace(ScreenSpace screenSpace) {
            return new Rectangle(
                (int)Math.Floor(screenSpace.X * tileSize),
                (int)Math.Floor(screenSpace.Y * tileSize),
                (int)Math.Floor(screenSpace.Width * tileSize),
                (int)Math.Floor(screenSpace.Height * tileSize)
            );
        }

        private void renderLayers(int start,int length) {
            Rectangle source = rasterizeScreenSpace(screenSpace);
            for(int i = start;i < length;i++) {
                layers[i].Render(viewport,source);
            }
        }

        public Rectangle GetDestination(Entity entity) {
            var tileSize = screenSpace.TileSize;
            return new Rectangle(
                (int)((screenSpace.X + entity.X) * tileSize),
                (int)((screenSpace.Y + entity.Y) * tileSize),
                (int)(entity.Width * tileSize),
                (int)(entity.Height * tileSize)
            );
        }
        public bool OnScreen(Entity entity) {
            return !(
                entity.X + entity.Width <= screenSpace.X ||
                entity.Y + entity.Height <= screenSpace.Y ||
                entity.X >= screenSpace.X + screenSpace.Width ||
                entity.Y >= screenSpace.Y + screenSpace.Height
            );
        }

        private Rectangle viewport;
        private ScreenSpace screenSpace;

        internal override void Draw(GameTime gameTime) {
            game.GraphicsDevice.Clear(Color.Black);
            Viewport viewport = Graphics.GetViewport(game);
            this.viewport = viewport.Bounds;
            screenSpace = getScreenSpace(viewport);
            updateLayers();
            game.SpriteBatch.Begin(SpriteSortMode.Deferred,BlendState.NonPremultiplied,SamplerState.PointClamp);
            if(LayerMode.Background) {
                renderLayers(LayerMode.BackgroundStart,LayerMode.BackgroundLength);
            }
            renderEntities(gameTime);
            if(LayerMode.Foreground) {
                renderLayers(LayerMode.ForegroundStart,LayerMode.ForegroundLength);
            }
            game.SpriteBatch.End();
        }

        public override void Export(SerialFrame frame) {
            frame.Set("Camera",Camera);
            entityManager.Export(frame);
        }

        public override void Import(SerialFrame frame) {
            frame.Get("Camera",Camera);
            entityManager.Import(frame);
        }
    }
}
