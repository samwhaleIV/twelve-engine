using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D {
    public class Grid2D<T>:GameState where T:struct {

        private GameManager game;
        public Camera Camera { get; set; } = new Camera();

        private TrackedGrid<T> grid = null;
        private bool hasGrid() {
            return grid != null;
        }

        private ITileRenderer<T> tileRenderer;
        private bool hasTileRenderer() {
            return tileRenderer != null;
        }

        private RenderTarget2D tileBuffer = null;
        private bool hasTileBuffer() {
            return tileBuffer != null;
        }

        private bool refreshTileBuffer = false;
        private Stack<Rectangle> tileRefreshAreas = new Stack<Rectangle>();

        private void createTileBuffer() {
            refreshTileBuffer = false;
            tileRefreshAreas.Clear();

            if(hasTileBuffer()) {
                tileBuffer.Dispose();
                tileBuffer = null;
            }
            if(!hasGrid()) {
                return;
            }

            int textureWidth = Width * TileSize;
            int textureHeight = Height * TileSize;

            tileBuffer = new RenderTarget2D(game.GraphicsDevice,textureWidth,textureHeight);
            refreshTileBuffer = true;
        }

        public TrackedGrid<T> CreateGrid(int width,int height) {
            var trackedGrid = new TrackedGrid<T>(width,height);
            setGrid(trackedGrid);
            return trackedGrid;
        }

        private void gridValueChanged(Rectangle rectangle) {
            tileRefreshAreas.Push(rectangle);
        }
        private void gridInvalidated() {
            refreshTileBuffer = true;
        }

        private void unloadGrid() {
            grid.Invalidated = null;
            grid.ValueChanged = null;
        }
        private void setGrid(TrackedGrid<T> grid) {
            if(hasGrid()) unloadGrid();

            this.grid = grid;

            grid.Invalidated = gridInvalidated;
            grid.ValueChanged = gridValueChanged;

            createTileBuffer();
        }
        public TrackedGrid<T> Grid {
            get {
                return grid;
            }
            set {
                setGrid(value);
            }
        }

        public int Width => grid.Width;
        public int Height => grid.Height;

        public int TileSize { get; set; } = Constants.DefaultTileSize;

        private void setTileRenderer(ITileRenderer<T> tileRenderer) {
            if(hasTileRenderer()) {
                tileRenderer.Unload();
            }
            this.tileRenderer = tileRenderer;
            if(!hasTileRenderer()) {
                refreshTileBuffer = false;
                tileRefreshAreas.Clear();
                if(!hasTileBuffer()) {
                    return;
                }
                tileBuffer.Dispose();
                tileBuffer = null;
                return; 
            }
            tileRenderer.Load(game,this);
            createTileBuffer();
        }

        public ITileRenderer<T> TileRenderer {
            get {
                return tileRenderer;
            }
            set {
                setTileRenderer(value);
            }
        }

        private ITileRenderer<T> pendingTileRenderer = null;
        public Grid2D(ITileRenderer<T> tileRenderer) {
            pendingTileRenderer = tileRenderer;
        }

        public Action OnLoad { get; set; } = null;

        internal override void Load(GameManager game) {
            this.game = game;
            if(pendingTileRenderer == null) {
                return;
            }
            setTileRenderer(pendingTileRenderer);
            pendingTileRenderer = null;

            this.entityManager = new EntityManager(this);
            entityManager.UpdateListChanged = updateUpdateables;
            entityManager.RenderListChanged = updateRenderables;

            if(OnLoad != null) {
                OnLoad();
                OnLoad = null;
            }
        }

        internal override void Unload() {
            if(hasTileRenderer()) tileRenderer.Unload();
            if(hasTileBuffer()) tileBuffer.Dispose();
            if(hasGrid()) unloadGrid();
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

        private void renderTiles(Rectangle area) {
            int endX = area.X + area.Width;
            int endY = area.Y + area.Height;
            for(int x = area.X;x < endX;x++) {
                for(int y = area.Y;y < endY;y++) {
                    Rectangle destination = new Rectangle(x * TileSize,y * TileSize,TileSize,TileSize);
                    tileRenderer.RenderTile(grid[x,y],destination);
                }
            }
        }

        private struct ScreenSpace {
            internal float X;
            internal float Y;
            internal float Width;
            internal float Height;
        }

        private Rectangle getScreenSpaceRectangle(ScreenSpace screenSpace) {
            return new Rectangle(
                (int)Math.Floor(screenSpace.X * TileSize),
                (int)Math.Floor(screenSpace.Y * TileSize),
                (int)Math.Floor(screenSpace.Width * TileSize),
                (int)Math.Floor(screenSpace.Height * TileSize)
            );
        }

        private ScreenSpace getScreenSpace(Viewport viewport) {
            float tileSize = Camera.Scale * TileSize;

            float screenWidth = viewport.Width / tileSize;
            float screenHeight = viewport.Height / tileSize;

            float halfScreenWidth = screenWidth * 0.5f;
            float halfScreenHeight = screenHeight * 0.5f;

            float x = Camera.X - halfScreenWidth + Camera.XOffset;
            float y = Camera.Y - halfScreenHeight + Camera.YOffset;

            if(Camera.HorizontalEdgePadding) {
                if(x < 0) {
                    x = 0;
                } else if(x + screenWidth >= Width) {
                    x = Width - screenWidth;
                }
            }

            if(Camera.VerticalEdgePadding) {
                if(y < 0) {
                    y = 0;
                } else if(y + screenHeight >= Height) {
                    y = Height - screenHeight;
                }
            }

            return new ScreenSpace() {
                X = x,
                Y = y,
                Width = screenWidth,
                Height = screenHeight
            };
        }
        public (float x,float y) GetCoordinate(int screenX,int screenY) {
            /* Don't call this during rendering! (For performance) */
            var viewport = Graphics.GetViewport(game);
            var screenSpace = getScreenSpace(viewport);
            float x = (float)screenX / viewport.Width * screenSpace.Width;
            float y = (float)screenY / viewport.Height * screenSpace.Height;
            return (x + screenSpace.X, y + screenSpace.Y);
        }

        private void updateTileBuffer() {
            bool redrawAll = tileBuffer.IsContentLost || refreshTileBuffer;
            bool redrawSome = tileRefreshAreas.Count > 0;

            if(!(redrawAll || redrawSome)) return;

            game.GraphicsDevice.SetRenderTarget(tileBuffer);
            game.SpriteBatch.Begin();

            if(redrawAll) {
                renderTiles(new Rectangle(0,0,Width,Height));
            } else {
                Rectangle area;
                while(tileRefreshAreas.TryPeek(out area)) {
                    renderTiles(area);
                    tileRefreshAreas.Pop();
                }
            }

            game.SpriteBatch.End();
            game.GraphicsDevice.SetRenderTarget(null);
            refreshTileBuffer = false;
        }

        private void renderTileBuffer(Rectangle destination,Rectangle source) {
            game.SpriteBatch.Draw(tileBuffer,destination,source,Color.White,0,Vector2.Zero,SpriteEffects.None,1f);
        }

        private void renderEntities(GameTime gameTime) {
            for(var i = 0;i < renderables.Length;i++) {
                renderables[i].Render(gameTime);
            }
        }

        internal override void Draw(GameTime gameTime) {
            game.GraphicsDevice.Clear(Color.Black);

            if(!hasTileBuffer()) return;

            Viewport viewport = Graphics.GetViewport(game);
            ScreenSpace screenSpace = getScreenSpace(viewport);
            Rectangle source = getScreenSpaceRectangle(screenSpace);

            if(hasTileRenderer()) updateTileBuffer();

            game.SpriteBatch.Begin(SpriteSortMode.Deferred,BlendState.NonPremultiplied,SamplerState.PointClamp);
            renderTileBuffer(viewport.Bounds,source);
            renderEntities(gameTime);
            game.SpriteBatch.End();
        }

        public override void Export(SerialFrame frame) {
            frame.Set("Camera",Camera);
            frame.Set("TileSize",TileSize);
            frame.SetArray2D("GridData",Grid.Data);
            entityManager.Export(frame);
        }

        public override void Import(SerialFrame frame) {
            frame.Get("Camera",Camera);
            TileSize = frame.GetInt("TileSize");
            T[,] gridData = frame.GetArray2D<T>("GridData");
            var grid = new TrackedGrid<T>(gridData);
            setGrid(grid);
            entityManager.Import(frame);
        }
    }
}
