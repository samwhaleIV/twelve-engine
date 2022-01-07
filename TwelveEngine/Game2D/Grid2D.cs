using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Serial.Map;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game2D.Collision;
using TwelveEngine.Game2D.Entity;
using TwelveEngine.GameUI;

namespace TwelveEngine.Game2D {
    public sealed class Grid2D:GameState {

        public Grid2D(
            int? tileSize = null,
            LayerMode? layerMode = null,
            CollisionTypes collisionTypes = null
        ) {
            if(!tileSize.HasValue) {
                tileSize = Constants.Config.TileSize;
            }
            this.tileSize = tileSize.Value;
            LayerMode = layerMode.HasValue ? layerMode.Value : LayerModes.Default;

            collisionInterface = new CollisionInterface(this);
            collisionInterface.Types = collisionTypes;

            OnLoad += Grid2D_OnLoad;
            OnUnload += Grid2D_OnUnload;

            OnImport += Grid2D_OnImport;
            OnExport += Grid2D_OnExport;
        }

        public ImpulseGuide ImpulseGuide { get; private set; }

        private Point size;

        public int Columns => size.X;
        public int Rows => size.Y;

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

        public InteractionLayer Interaction { get; set; }

        public EntityManager<Entity2D,Grid2D> EntityManager { get; private set; }

        public void AddEntity(Entity2D entity) => EntityManager.AddEntity(entity);

        private IUpdateable[] updateables = new IUpdateable[0];
        private IRenderable[] renderables = new IRenderable[0];

        private void updateUpdateables(IUpdateable[] updateables) {
            this.updateables = updateables;
        }
        private void updateRenderables(IRenderable[] renderables) {
            this.renderables = renderables;
        }

        private int[][,] layers;
        private TileRenderer tileRenderer = null;
        private TileRenderer pendingTileRenderer = null;

        public Viewport Viewport => Game.GraphicsDevice.Viewport;

        public ScreenSpace ScreenSpace { get; private set; }
        public ScreenSpace GetScreenSpace() => getScreenSpace(Viewport);

        private bool spriteBatchActive = false;

        public void ImportMap(Map map) {
            layers = map.Layers2D;
            size = new Point(map.Width,map.Height);
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
            var entityManager = new EntityManager<Entity2D,Grid2D>(this,Entity2DType.GetFactory());

            entityManager.OnUpdateListChanged += updateUpdateables;
            entityManager.OnRenderListChanged += updateRenderables;

            OnImport += entityManager.Import;
            OnExport += entityManager.Export;

            EntityManager = entityManager;
        }

        private void loadImpulseGuide() {
            var impulseGuide = new ImpulseGuide(Game);
            impulseGuide.Load();
            ImpulseGuide = impulseGuide;
        }

        private void Grid2D_OnLoad() {
            loadImpulseGuide();          
            collisionInterface.Types.LoadTypes();
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
            EntityManager.Unload();
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
                    camera.SetPadding(false);
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

        private static float cameraBounds(float value,float size,int gridSize) {
            if(value < 0f) {
                return 0f;
            }
            if(value + size > gridSize) {
                return gridSize - size;
            }
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

            Vector2 size = viewport.Bounds.Size.ToVector2() / tileSize;
            Vector2 position = Camera.Position + Camera.Offset - (size * 0.5f) + new Vector2(0.5f);

            if(camera.HorizontalPadding) {
                position.X = cameraBounds(position.X,size.X,Columns);
            }
            if(camera.VerticalPadding) {
                position.Y = cameraBounds(position.Y,size.Y,Rows);
            }

            return new ScreenSpace(position,size,tileSize);
        }

        public Vector2 GetCoordinate(ScreenSpace screenSpace,Point position) {
            return position.ToVector2() / Viewport.Bounds.Size.ToVector2() * screenSpace.Size;
        }

        public Vector2 GetCoordinate(Point position) {
            return GetCoordinate(ScreenSpace,position);
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
                tileRenderer.RenderTiles(layers[i]);
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

        public override void Render(GameTime gameTime) {
            ScreenSpace = getScreenSpace(Game.GraphicsDevice.Viewport);
            tileRenderer.CacheArea(ScreenSpace);

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

            tryStartSpriteBatch();

            if(LayerMode.Foreground) {

                renderLayers(LayerMode.ForegroundStart,LayerMode.ForegroundLength);
            }

            ImpulseGuide.Render();
            tryEndSpriteBatch();
        }

        private void Grid2D_OnExport(SerialFrame frame) {
            frame.Set(camera);
            frame.Set(LayerMode);
            frame.Set(size);
            frame.Set(PanZoom);
            frame.Set(layers.Length);
            for(var i = 0;i<layers.Length;i++) {
                frame.Set(layers[i]);
            }
        }

        private void Grid2D_OnImport(SerialFrame frame) {
            frame.Get(camera);
            frame.Get(LayerMode);
            size = frame.GetPoint();
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
