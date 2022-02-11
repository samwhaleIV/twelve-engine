﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Serial.Map;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game2D.Collision;
using TwelveEngine.Game2D.Entity;
using TwelveEngine.GameUI;
using TwelveEngine.Serial;

namespace TwelveEngine.Game2D {
    public class Grid2D:GameState {

        public Grid2D(
            int? tileSize = null,
            LayerMode? layerMode = null,
            CollisionTypes collisionTypes = null,
            EntityFactory<Entity2D,Grid2D> entityFactory = null
        ) {

            camera = new Camera();
            camera.Invalidated += Camera_Invalidated;

            if(!tileSize.HasValue) {
                tileSize = Constants.Config.TileSize;
            }
            this.tileSize = tileSize.Value;
            LayerMode = layerMode.HasValue ? layerMode.Value : LayerModes.Default;

            collisionInterface = new CollisionInterface(this);
            collisionInterface.Types = collisionTypes;

            if(entityFactory == null) {
                entityFactory = Entity2DType.GetFactory();
            }
            this.entityFactory = entityFactory;

            OnLoad += Grid2D_OnLoad;
            OnUnload += Grid2D_OnUnload;

            OnImport += Grid2D_OnImport;
            OnExport += Grid2D_OnExport;

            OnRender += render;
            OnUpdate += update;
        }

        private readonly EntityFactory<Entity2D,Grid2D> entityFactory;

        public ImpulseGuide ImpulseGuide { get; private set; }

        private Point size;

        public int Columns => size.X;
        public int Rows => size.Y;

        public bool TileInRange(Point tile) {
            return tile.X >= 0 && tile.Y >= 0 && tile.X < Columns && tile.Y < Rows;
        }

        public Point GetTile(Vector2 location) {
            return Vector2.Floor(location).ToPoint();
        }

        public Point GetTile(Point screenPoint) {
            return GetTile(GetWorldVector(screenPoint));
        }

        public bool TryGetTile(Point screenPoint,out Point tile) {
            tile = GetTile(screenPoint);
            return TileInRange(tile);
        }

        public bool TryGetTile(Vector2 location,out Point tile) {
            tile = GetTile(location);
            return TileInRange(tile);
        }

        private Camera camera;

        public Camera Camera {
            get => camera;
            set {
                if(value == null) {
                    throw new ArgumentNullException("value");
                }
                if(camera == value) {
                    return;
                }
                camera.Invalidated -= Camera_Invalidated;
                camera = value;
            }
        }

        public Color BackgroundColor { get; set; } = Color.Black;

        private readonly int tileSize;
        public int TileSize => tileSize;

        public LayerMode LayerMode;

        private readonly CollisionInterface collisionInterface;
        public CollisionInterface Collision => collisionInterface;

        public InteractionLayer Interaction { get; set; }

        public EntityManager<Entity2D,Grid2D> Entities { get; private set; }

        private int[][,] layers;
        private TileRenderer tileRenderer = null;
        private TileRenderer pendingTileRenderer = null;

        public Viewport Viewport => Game.Viewport;

        public ScreenSpace ScreenSpace { get; private set; }

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

        private void Grid2D_OnLoad() {
            ImpulseGuide = new ImpulseGuide(Game);
            collisionInterface.Types?.LoadTypes();
            Entities = new EntityManager<Entity2D,Grid2D>(this,entityFactory);
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
        }

        private void renderEntities(GameTime gameTime) {
            Entities.IterateImmutable(Entity2D.Render,gameTime);
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

        public int CalculateTileSize() {
            int tileSize = (int)Math.Round(camera.Scale * this.tileSize);
            if(tileSize % 2 == 1) {
                tileSize++;
            }
            return tileSize;
        }

        private void updateScreenSpace(bool automatic = true) {
            if(!automatic) {
                if(didManualScreenSpaceUpdate) {
                    // Ignore repeated requests
                    return;
                }
                didManualScreenSpaceUpdate = true;
            }
            ScreenSpace = getScreenSpace();
        }

        public void UpdateScreenSpace() => updateScreenSpace(automatic: false);

        public Vector2 GetCenter() => ScreenSpace.GetCenter();

        private ScreenSpace getScreenSpace() {
            int tileSize = CalculateTileSize();

            Vector2 size = Viewport.Bounds.Size.ToVector2() / tileSize;
            Vector2 position = Camera.Position + Camera.Offset - (size * 0.5f) + new Vector2(0.5f);

            if(camera.HorizontalPadding) {
                position.X = cameraBounds(position.X,size.X,Columns);
            }
            if(camera.VerticalPadding) {
                position.Y = cameraBounds(position.Y,size.Y,Rows);
            }

            return new ScreenSpace(position,size,tileSize);
        }

        public Vector2 GetWorldVector(Point screenLocation) {
            return ScreenSpace.Location + screenLocation.ToVector2() / Viewport.Bounds.Size.ToVector2() * ScreenSpace.Size;
        }

        public Point GetScreenPoint(Vector2 worldLocation) {
            return ((worldLocation - ScreenSpace.Location) * ScreenSpace.TileSize).ToPoint();
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

        private bool rendering = false;

        private bool didManualScreenSpaceUpdate = false;

        private void Camera_Invalidated() {
            if(rendering) {
                throw new InvalidOperationException("Cannot invalidate the camera during the render cycle!");
            }
            if(!didManualScreenSpaceUpdate) {
                return;
            }
            updateScreenSpace(automatic: true);
        }

        private void update(GameTime gameTime) {
            Entities.IterateMutable(Entity2D.Update,gameTime);
            didManualScreenSpaceUpdate = false;
        }

        private void render(GameTime gameTime) {
            updateScreenSpace(automatic: true);
            rendering = true;
            tileRenderer.CacheArea(ScreenSpace);

            Game.GraphicsDevice.Clear(BackgroundColor);
            
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
            rendering = false;
        }

        private void Grid2D_OnExport(SerialFrame frame) {
            frame.Set(BackgroundColor);
            frame.Set(camera);
            frame.Set(LayerMode);
            frame.Set(size);
            frame.Set(layers.Length);
            for(var i = 0;i<layers.Length;i++) {
                frame.Set(layers[i]);
            }
        }

        private void Grid2D_OnImport(SerialFrame frame) {
            BackgroundColor = frame.GetColor();
            frame.Get(camera);
            frame.Get(LayerMode);
            size = frame.GetPoint();
            var layerCount = frame.GetInt();

            var layers = new int[layerCount][,];
            for(var i = 0;i<layerCount;i++) {
                layers[i] = frame.GetIntArray2D();
            }
            this.layers = layers;
        }
    }
}
