using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine {
    public class Grid2D:GameState {

        public static T[,] GetGrid<T>(int width,int height,Func<int,int,T> pattern) {
            T[,] grid = new T[width,height];
            for(int x = 0;x < width;x++) {
                for(int y = 0;y < height;y++) {
                    grid[x,y] = pattern(x,y);
                }
            }
            return grid;
        }

        public Camera Camera { get; set; } = new Camera();

        protected int gridWidth = 0;
        protected int gridHeight = 0;

        private int[,] grid = new int[0,0];

        private ITileRenderer tileRenderer;
        private GameManager game;

        private RenderTarget2D tileBuffer = null;

        private bool refreshTileBuffer = false;
        private Stack<Rectangle> tileRefreshAreas = new Stack<Rectangle>();

        private void createTileBuffer() {
            int textureWidth = gridWidth * TileSize;
            int textureHeight = gridHeight * TileSize;
            if(tileBuffer != null) {
                tileBuffer.Dispose();
            }
            tileBuffer = new RenderTarget2D(game.GraphicsDevice,textureWidth,textureHeight);
            tileRefreshAreas.Clear();
            refreshTileBuffer = true;
        }

        private void setGrid(int[,] grid) {
            if(grid == null) {
                grid = new int[0,0];
            }
            this.grid = grid;
            this.gridWidth = grid.GetLength(0);
            this.gridHeight = grid.GetLength(1);
            createTileBuffer();
        }
        public int[,] Grid {
            get {
                return grid;
            }
            set {
                setGrid(value);
            }
        }
        public int Width {
            get {
                return gridWidth;
            }
        }
        public int Height {
            get {
                return gridHeight;
            }
        }

        public int TileSize { get; set; } = Constants.DefaultTileSize;

        private void setTileRenderer(ITileRenderer tileRenderer) {
            if(hasTileRenderer()) {
                tileRenderer.Unload();
            }
            this.tileRenderer = tileRenderer;
            if(!hasTileRenderer()) {
                if(tileBuffer == null) {
                    return;
                }
                tileBuffer.Dispose();
                tileBuffer = null;
                refreshTileBuffer = false;
                tileRefreshAreas.Clear();
                return;
            }
            tileRenderer.Load(game,this);
            createTileBuffer();
        }

        private bool hasTileRenderer() {
            return tileRenderer != null;
        }
        public ITileRenderer TileRenderer {
            get {
                return tileRenderer;
            }
            set {
                setTileRenderer(value);
            }
        }

        private ITileRenderer pendingTileRenderer = null;
        public Grid2D(ITileRenderer tileRenderer) {
            pendingTileRenderer = tileRenderer;
        }

        internal override void Load(GameManager game) {
            this.game = game;
            if(pendingTileRenderer == null) {
                return;
            }
            setTileRenderer(pendingTileRenderer);
            pendingTileRenderer = null;
        }

        internal override void Unload() {
            if(hasTileRenderer()) {
                tileRenderer.Unload();
            }
            if(tileBuffer != null) {
                tileBuffer.Dispose();
            }
        }

        internal override void Update(GameTime gameTime) {
            /* TODO */
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
                } else if(x + screenWidth >= gridWidth) {
                    x = gridWidth - screenWidth;
                }
            }

            if(Camera.VerticalEdgePadding) {
                if(y < 0) {
                    y = 0;
                } else if(y + screenHeight >= gridHeight) {
                    y = gridHeight - screenHeight;
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
            var viewport = Graphics2D.GetViewport(game);
            var screenSpace = getScreenSpace(viewport);
            float x = (float)screenX / viewport.Width * screenSpace.Width;
            float y = (float)screenY / viewport.Height * screenSpace.Height;
            return (x + screenSpace.X, y + screenSpace.Y);
        }

        private void updateTileBuffer() {
            if(tileBuffer == null) {
                return;
            }
            if(tileBuffer.IsContentLost || refreshTileBuffer) {
                game.GraphicsDevice.SetRenderTarget(tileBuffer);
                game.SpriteBatch.Begin();

                renderTiles(new Rectangle(0,0,gridWidth,gridHeight));

                game.SpriteBatch.End();
                game.GraphicsDevice.SetRenderTarget(null);

                refreshTileBuffer = false;
                tileRefreshAreas.Clear();
            } else if(tileRefreshAreas.Count > 0) {
                Rectangle area;

                game.GraphicsDevice.SetRenderTarget(tileBuffer);
                game.SpriteBatch.Begin();
                while(tileRefreshAreas.TryPeek(out area)) {
                    renderTiles(area);
                    tileRefreshAreas.Pop();
                }

                game.SpriteBatch.End();
                game.GraphicsDevice.SetRenderTarget(null);
            }
        }

        internal override void Draw(GameTime gameTime) {
            game.GraphicsDevice.Clear(Color.Black);

            if(!hasTileRenderer()) {
                return;
            }

            Viewport viewport = Graphics2D.GetViewport(game);
            ScreenSpace screenSpace = getScreenSpace(viewport);
            Rectangle sourceRectangle = getScreenSpaceRectangle(screenSpace);

            updateTileBuffer();

            game.SpriteBatch.Begin();
            game.SpriteBatch.Draw(tileBuffer,viewport.Bounds,sourceRectangle,Color.White,0,Vector2.Zero,SpriteEffects.None,1f);
            game.SpriteBatch.End();
        }

        public override void Export(SerialFrame frame) {
            frame.Set("Camera",Camera);
            frame.Set("Grid",Grid);
            frame.Set("TileSize",TileSize);
        }

        public override void Import(SerialFrame frame) {
            frame.Get("Camera",Camera);
            Grid = frame.GetIntArray2D("Grid");
            frame.Set("TileSize",TileSize);
        }
    }
}
