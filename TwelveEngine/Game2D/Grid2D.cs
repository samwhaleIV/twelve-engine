using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D {
    public class Grid2D:GameState {

        private readonly int tileSize;
        public int TileSize => tileSize;

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
        public GridLayer CreateLayer(int[,] grid,ITileRenderer tileRenderer) {
            var layer = new GridLayer(tileRenderer) {
                Data = grid
            };
            return layer;
        }
        public GridLayer CreateLayer(ITileRenderer tileRenderer) {
            var layer = new GridLayer(tileRenderer);
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

        private PanZoom panZoom = null;
        private bool hasPanZoom() {
            return panZoom != null;
        }
        public bool PanZoom {
            get {
                return hasPanZoom();
            }
            set {
                if(value) {
                    if(hasPanZoom()) {
                        return;
                    }
                    Camera.EdgePadding = false;
                    panZoom = new PanZoom(this);
                } else if(hasPanZoom()) {
                    panZoom = null;
                }
            }
        }

        private Viewport viewport;
        private ScreenSpace screenSpace;

        public Viewport Viewport => viewport;
        public ScreenSpace ScreenSpace => screenSpace;

        public ScreenSpace GetScreenSpace() {
            return getScreenSpace(viewport);
        }

        internal override void Update(GameTime gameTime) {
            viewport = Graphics.GetViewport(game);
            for(var i = 0;i<updateables.Length;i++) {
                updateables[i].Update(gameTime);
            }
            panZoom?.Update(gameTime);
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

        public (float x, float y) GetCoordinate(ScreenSpace screenSpace,int screenX,int screenY) {
            float x = (float)screenX / viewport.Width * screenSpace.Width;
            float y = (float)screenY / viewport.Height * screenSpace.Height;
            return (x + screenSpace.X, y + screenSpace.Y);
        }

        public (float x,float y) GetCoordinate(int screenX,int screenY) {
            return GetCoordinate(screenSpace,screenX,screenY);
        }

        private void renderEntities(GameTime gameTime) {
            for(var i = 0;i < renderables.Length;i++) {
                renderables[i].Render(gameTime);
            }
        }

        public Rectangle GetDestination(Entity entity) {
            var tileSize = screenSpace.TileSize;

            Rectangle destination = new Rectangle();

            destination.X = (int)Math.Round((entity.X - screenSpace.X) * tileSize);
            destination.Y = (int)Math.Round((entity.Y - screenSpace.Y) * tileSize);

            destination.Width = (int)Math.Floor(entity.Width * tileSize);
            destination.Height = (int)Math.Floor(entity.Height * tileSize);

            return destination;
        }
        public bool OnScreen(Entity entity) {
            return !(
                entity.X + entity.Width <= screenSpace.X ||
                entity.Y + entity.Height <= screenSpace.Y ||
                entity.X >= screenSpace.X + screenSpace.Width ||
                entity.Y >= screenSpace.Y + screenSpace.Height
            );
        }

        private void renderLayers(int start,int length) {
            for(int i = start;i < length;i++) {
                layers[i].Render(screenSpace);
            }
        }

        internal override void Draw(GameTime gameTime) {
            screenSpace = getScreenSpace(viewport);
            game.GraphicsDevice.Clear(Color.Black);
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
