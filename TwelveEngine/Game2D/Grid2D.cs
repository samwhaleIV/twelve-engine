using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Serial;

namespace TwelveEngine.Game2D {
    public partial class Grid2D:GameState {

        public int Width { get; set; } = 0;
        public int Height { get; set; } = 0;

        public Camera Camera { get; set; } = new Camera();

        private readonly int tileSize;
        public int TileSize => tileSize;

        public LayerMode LayerMode;

        private readonly CollisionInterface collisionInterface;
        public CollisionInterface CollisionInterface => collisionInterface;

        private InteractionLayer interactionLayer = null;
        public InteractionLayer InteractionLayer {
            get => interactionLayer;
            set => interactionLayer = value;
        }

        private EntityManager entityManager = null;
        public EntityManager EntityManager => entityManager;
        public void AddEntity(Entity entity) => EntityManager.AddEntity(entity);

        private IUpdateable[] updateables = new IUpdateable[0];
        private IRenderable[] renderables = new IRenderable[0];

        private void updateUpdateables(IUpdateable[] updateables) => this.updateables = updateables;
        private void updateRenderables(IRenderable[] renderables) => this.renderables = renderables;

        private int[][,] layers;
        private ITileRenderer tileRenderer = null;
        private ITileRenderer pendingTileRenderer = null;

        private bool loaded = false;

        private PanZoom panZoom = null;

        private ScreenSpace screenSpace;
        public Viewport Viewport => Game.GraphicsDevice.Viewport;
        public ScreenSpace ScreenSpace => screenSpace;

        private bool spriteBatchActive = false;

        public Grid2D() {
            tileSize = Constants.DefaultTileSize;
            LayerMode = LayerModes.Default;
            collisionInterface = new CollisionInterface(this);
        }

        public Grid2D(int tileSize) {
            this.tileSize = tileSize;
            LayerMode = LayerModes.Default;
            collisionInterface = new CollisionInterface(this);
        }

        public Grid2D(LayerMode layerMode) {
            tileSize = Constants.DefaultTileSize;
            LayerMode = layerMode;
            collisionInterface = new CollisionInterface(this);
        }

        public Grid2D(int tileSize,LayerMode layerMode) {
            this.tileSize = tileSize;
            LayerMode = layerMode;
            collisionInterface = new CollisionInterface(this);
        }

        private static int[,] convertFlatArray(int[] array,int width,int height) {
            var array2D = new int[width,height];

            for(var x = 0;x<width;x++) {
                for(var y = 0;y<height;y++) {
                    array2D[x,y] = array[x + y * width];
                }
            }

            return array2D;
        }

        public void ImportMap(Map map) {
            var width = map.Width;
            var height = map.Height;

            var layers = map.Layers;
            var newLayers = new int[map.Layers.Length][,];
            for(var i = 0;i<newLayers.Length;i++) {
                newLayers[i] = convertFlatArray(layers[i],width,height);
            }

            this.layers = newLayers;

            Width = width;
            Height = height;
        }

        public void Fill(int layerIndex,Func<int,int,int> pattern) {
            var map2D = layers[layerIndex];
            for(var x = 0;x < Width;x++) {
                for(var y = 0;y < Height;y++) {
                    map2D[x,y] = pattern(x,y);
                }
            }
        }

        public int[,] GetLayer(int index) => layers[index];
        private bool hasTileRenderer() => tileRenderer != null;

        private void setTileRenderer(ITileRenderer tileRenderer) {
            if(!loaded) {
                pendingTileRenderer = tileRenderer;
                return;
            }
        }

        public ITileRenderer TileRenderer {
            get {
                if(!hasTileRenderer()) {
                    return pendingTileRenderer;
                }
                return tileRenderer;
            }
            set {
                setTileRenderer(value);
            }
        }

        private void loadEntityManager() {
            entityManager = new EntityManager(this) {
                RenderListChanged = updateRenderables,
                UpdateListChanged = updateUpdateables
            };
            OnImport += entityManager.Import;
            OnExport += entityManager.Export;
        }

        internal override void Load() {
            loadEntityManager();
            if(pendingTileRenderer != null) {
                tileRenderer = pendingTileRenderer;
                pendingTileRenderer = null;
                tileRenderer.Load(Game,this);
            }
            base.Load(); /* For future reference, base.Load() of GameState puts
                          * execution order responsibility on the dervived class */
            loaded = true;
        }

        internal override void Unload() {
            base.Unload();
            if(hasTileRenderer()) {
                tileRenderer.Unload();
            }
            entityManager.Unload();
        }

        private bool hasPanZoom() => panZoom != null;
        public bool PanZoom {
            get => hasPanZoom();
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

        public ScreenSpace GetScreenSpace() => getScreenSpace(Viewport);

        internal override void Update(GameTime gameTime) {
            panZoom?.Update(gameTime);
            for(var i = 0;i<updateables.Length;i++) {
                updateables[i].Update(gameTime);
            }
        }

        private static float zeroMinMax(float value,float width,int gridWidth) {
            if(value < 0f) return 0f;
            if(value + width >= gridWidth) return gridWidth - width;
            return value;
        }

        private int getTileSize() {
            int tileSize = (int)Math.Round(Camera.Scale * this.tileSize);
            if(tileSize % 2 == 1) {
                tileSize++;
            }
            return tileSize;
        }

        private ScreenSpace getScreenSpace(Viewport viewport) {
            int tileSize = getTileSize();

            float screenWidth = viewport.Width / (float)tileSize;
            float screenHeight = viewport.Height / (float)tileSize;

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
            var viewport = Viewport;
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

        public float GetPixelSafeValue(float value) {
            var tileSize = getTileSize();
            var pixelSize = 1 / (float)tileSize;
            var pixelCount = (int)Math.Floor(value / pixelSize);
            return pixelCount * pixelSize;
        }

        public Rectangle GetDestination(Entity entity) {
            var tileSize = screenSpace.TileSize;

            var destination = new Rectangle {
                X = (int)Math.Round((entity.X - screenSpace.X) * tileSize),
                Y = (int)Math.Round((entity.Y - screenSpace.Y) * tileSize),

                Width = (int)Math.Floor(entity.Width * tileSize),
                Height = (int)Math.Floor(entity.Height * tileSize)
            };

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

        private void renderTiles(int[,] data,ScreenSpace screenSpace,bool calculateDepth) {
            int tileSize = screenSpace.TileSize;

            Rectangle target = new Rectangle(0,0,tileSize,tileSize);

            int startX = (int)Math.Floor(screenSpace.X);
            int startY = (int)Math.Floor(screenSpace.Y);

            float renderX = startX - screenSpace.X;
            float renderY = startY - screenSpace.Y;

            int width = (int)Math.Ceiling(screenSpace.Width - renderX);
            int height = (int)Math.Ceiling(screenSpace.Height - renderY);

            if(renderX * -2 > tileSize) width++;
            if(renderY * -2 > tileSize) height++;

            int tileX = (int)Math.Round(renderX * tileSize);
            int tileY = (int)Math.Round(renderY * tileSize);

            int xOffset = 0;
            int yOffset = 0;
            if(startX < 0) xOffset = -startX;
            if(startY < 0) yOffset = -startY;

            int endX = startX + width;
            int endY = startY + height;
            if(endX > Width) width -= endX - Width;
            if(endY > Height) height -= endY - Height;

            if(calculateDepth) {
                float viewportHeight = Viewport.Height;
                for(int x = xOffset;x < width;x++) {
                    int gridX = x + startX;
                    target.X = tileX + x * tileSize;
                    for(int y = yOffset;y < height;y++) {
                        int gridY = y + startY;
                        target.Y = tileY + y * tileSize;    
                        tileRenderer.RenderTileDepth(data[gridX,gridY],target,viewportHeight);
                    }
                }
            } else {
                for(int x = xOffset;x < width;x++) {
                    int gridX = x + startX;
                    target.X = tileX + x * tileSize;
                    for(int y = yOffset;y < height;y++) {
                        int gridY = y + startY;
                        target.Y = tileY + y * tileSize;
                        tileRenderer.RenderTile(data[gridX,gridY],target);
                    }
                }
            }
        }

        private void renderLayers(int start,int length,bool calculateDepth) {
            if(!hasTileRenderer()) {
                return;
            }
            int end = start + length;
            for(int i = start;i<end;i++) {
                renderTiles(layers[i],screenSpace,calculateDepth);
            }
        }

        private void startDeferredSpriteBatch() {
            Game.SpriteBatch.Begin(SpriteSortMode.Deferred,BlendState.NonPremultiplied,SamplerState.PointClamp);
            spriteBatchActive = true;
        }
        private void startBackToFrontSpriteBatch() {
            Game.SpriteBatch.Begin(SpriteSortMode.BackToFront,BlendState.NonPremultiplied,SamplerState.PointClamp);
            spriteBatchActive = true;
        }
        private void endSpriteBatch() {
            Game.SpriteBatch.End();
            spriteBatchActive = false;
        }
        private void tryStartSpriteBatch() {
            if(spriteBatchActive) {
                return;
            }
            startDeferredSpriteBatch();
        }
        private void tryEndSpriteBatch() {
            if(!spriteBatchActive) {
                return;
            }
            endSpriteBatch();
        }

        internal override void Draw(GameTime gameTime) {
            screenSpace = getScreenSpace(Game.GraphicsDevice.Viewport);
            Game.GraphicsDevice.Clear(Color.Black);
            
            if(LayerMode.Background) {
                if(LayerMode.BackgroundLength == 2) {
                    startDeferredSpriteBatch();
                    renderLayers(LayerMode.BackgroundStart,1,false);
                    endSpriteBatch();

                    startBackToFrontSpriteBatch();
                    renderLayers(LayerMode.BackgroundStart+1,1,true);
                    renderEntities(gameTime);
                    endSpriteBatch();
                } else {
                    startDeferredSpriteBatch();
                    renderLayers(LayerMode.BackgroundStart,LayerMode.BackgroundLength,false);
                    renderEntities(gameTime);
                }
            }

            if(LayerMode.Foreground) {
                tryStartSpriteBatch();
                renderLayers(LayerMode.ForegroundStart,LayerMode.ForegroundLength,false);
                endSpriteBatch();
            }

            tryEndSpriteBatch();
        }

        public event Action<SerialFrame> OnExport;
        public event Action<SerialFrame> OnImport;

        public override void Export(SerialFrame frame) {
            frame.Set(Camera);
            frame.Set(Width);
            frame.Set(Height);
            frame.Set(LayerMode);
            frame.Set(PanZoom);
            frame.Set(layers.Length);

            for(var i = 0;i<layers.Length;i++) {
                frame.Set(layers[i]);
            }

            OnExport?.Invoke(frame);
        }

        public override void Import(SerialFrame frame) {
            frame.Get(Camera);
            Width = frame.GetInt();
            Height = frame.GetInt();
            frame.Get(LayerMode);
            PanZoom = frame.GetBool();
            var layerCount = frame.GetInt();

            var layers = new int[layerCount][,];
            for(var i = 0;i<layerCount;i++) {
                layers[i] = frame.GetIntArray2D();
            }
            this.layers = layers;

            OnImport?.Invoke(frame);
        }
    }
}
