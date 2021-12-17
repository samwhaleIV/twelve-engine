using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Serial.Map;

namespace TwelveEngine.Game2D {
    public sealed class Grid2D:GameState {

        public Grid2D(

            int tileSize = Constants.DefaultTileSize,
            LayerMode? layerMode = null,
            CollisionTypes collisionTypes = null

        ) {
            this.tileSize = tileSize;
            LayerMode = layerMode.HasValue ? layerMode.Value : LayerModes.Default;

            collisionInterface = new CollisionInterface(this);
            if(collisionTypes == null) {
                collisionTypes = new CollisionTypes(tileSize);
            }
            collisionInterface.Types = collisionTypes;

            OnLoad += Grid2D_OnLoad;
            OnUnload += Grid2D_OnUnload;

            OnImport += Grid2D_OnImport;
            OnExport += Grid2D_OnExport;
        }

        private int width = 0, height = 0;

        public int Width => width;
        public int Height => height;

        private Camera camera = new Camera();

        public Camera Camera {
            get => camera;
            set {
                if(value == null) {
                    throw new ArgumentNullException("value");
                }
                camera = value;
            }
        }

        private readonly int tileSize;
        public int TileSize => tileSize;

        public LayerMode LayerMode;

        private readonly CollisionInterface collisionInterface;
        public CollisionInterface Collision => collisionInterface;

        private InteractionLayer interactionLayer = null;
        public InteractionLayer Interaction {
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
        private TileRenderer tileRenderer = null;
        private TileRenderer pendingTileRenderer = null;

        public Viewport Viewport => Game.GraphicsDevice.Viewport;

        private ScreenSpace screenSpace;
        public ScreenSpace ScreenSpace => screenSpace;
        public ScreenSpace GetScreenSpace() => getScreenSpace(Viewport);

        private bool spriteBatchActive = false;

        private static int[,] expandArray(int[] array,int width,int height) {
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
                newLayers[i] = expandArray(layers[i],width,height);
            }

            this.layers = newLayers;

            this.width = width;
            this.height = height;
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

        private void setTileRenderer(TileRenderer tileRenderer) {
            if(!IsLoaded) {
                pendingTileRenderer = tileRenderer;
                return;
            }
        }

        public TileRenderer TileRenderer {
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
            entityManager = new EntityManager(this);

            entityManager.OnUpdateListChanged += updateUpdateables;
            entityManager.OnRenderListChanged += updateRenderables;

            OnImport += entityManager.Import;
            OnExport += entityManager.Export;
        }

        private void Grid2D_OnLoad() {
            collisionInterface.Types?.LoadTypes(Game.Content);
            loadEntityManager();
            if(pendingTileRenderer != null) {
                tileRenderer = pendingTileRenderer;
                pendingTileRenderer = null;
                tileRenderer.Load(Game,this);
            }
        }

        private void Grid2D_OnUnload() {
            if(hasTileRenderer()) {
                tileRenderer.Unload();
            }
            entityManager.Unload();
        }

        private PanZoom panZoom = null;
        private bool hasPanZoom() => panZoom != null;
        public bool PanZoom {
            get => hasPanZoom();
            set {
                if(value) {
                    if(hasPanZoom()) {
                        return;
                    }
                    camera.EdgePadding = false;
                    panZoom = new PanZoom(this);
                } else if(hasPanZoom()) {
                    panZoom = null;
                }
            }
        }

        public override void Update(GameTime gameTime) {
            for(var i = 0;i<updateables.Length;i++) {
                updateables[i].Update(gameTime);
            }
        }

        private static float zeroMinMax(float value,float width,int gridWidth) {
            if(value < 0f) return 0f;
            if(value + width > gridWidth) return gridWidth - width;
            return value;
        }

        private int getTileSize() {
            int tileSize = (int)Math.Round(camera.Scale * this.tileSize);
            if(tileSize % 2 == 1) {
                tileSize++;
            }
            return tileSize;
        }

        private ScreenSpace getScreenSpace(Viewport viewport) {
            int tileSize = getTileSize();

            float width = viewport.Width / (float)tileSize;
            float height = viewport.Height / (float)tileSize;

            float halfScreenWidth = width * 0.5f;
            float halfScreenHeight = height * 0.5f;

            float x = camera.AbsoluteX - halfScreenWidth + 0.5f;
            float y = camera.AbsoluteY - halfScreenHeight + 0.5f;

            if(camera.HorizontalPadding) x = zeroMinMax(x,width,Width);
            if(camera.VerticalPadding) y = zeroMinMax(y,height,Height);

            return new ScreenSpace(x,y,width,height,tileSize);
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

        private void renderLayers(int start,int length) {
            if(!hasTileRenderer()) {
                return;
            }
            int end = start + length;
            for(int i = start;i<end;i++) {
                tileRenderer.RenderTiles(screenSpace,layers[i]);
            }
        }

        private void startDeferredSpriteBatch() {
            Game.SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            spriteBatchActive = true;
        }
        private void startBackToFrontSpriteBatch() {
            Game.SpriteBatch.Begin(SpriteSortMode.BackToFront,null,SamplerState.PointClamp);
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

        public override void Draw(GameTime gameTime) {
            screenSpace = getScreenSpace(Game.GraphicsDevice.Viewport);
            Game.GraphicsDevice.Clear(Color.Black);
            
            if(LayerMode.Background) {
                if(LayerMode.BackgroundLength == 2) {
                    startDeferredSpriteBatch();
                    renderLayers(LayerMode.BackgroundStart,1);
                    endSpriteBatch();

                    startBackToFrontSpriteBatch();
                    renderLayers(LayerMode.BackgroundStart+1,1);
                    renderEntities(gameTime);
                    endSpriteBatch();
                } else {
                    startDeferredSpriteBatch();
                    renderLayers(LayerMode.BackgroundStart,LayerMode.BackgroundLength);
                    renderEntities(gameTime);
                }
            }

            if(LayerMode.Foreground) {
                tryStartSpriteBatch();
                renderLayers(LayerMode.ForegroundStart,LayerMode.ForegroundLength);
            }

            tryEndSpriteBatch();
        }

        private void Grid2D_OnExport(SerialFrame frame) {
            frame.Set(camera);
            frame.Set(Width);
            frame.Set(Height);
            frame.Set(LayerMode);
            frame.Set(PanZoom);
            frame.Set(layers.Length);
            for(var i = 0;i<layers.Length;i++) {
                frame.Set(layers[i]);
            }
        }

        private void Grid2D_OnImport(SerialFrame frame) {
            frame.Get(camera);
            width = frame.GetInt();
            height = frame.GetInt();
            frame.Get(LayerMode);
            PanZoom = frame.GetBool();
            var layerCount = frame.GetInt();

            var layers = new int[layerCount][,];
            for(var i = 0;i<layerCount;i++) {
                layers[i] = frame.GetIntArray2D();
            }
            this.layers = layers;
        }
    }
}
