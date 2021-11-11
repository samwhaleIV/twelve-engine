using System;
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

        protected void SetGrid(int[,] grid) {
            if(grid == null) {
                grid = new int[0,0];
            }
            this.grid = grid;
            this.gridWidth = grid.GetLength(0);
            this.gridHeight = grid.GetLength(1);
        }
        public int[,] Grid {
            get {
                return grid;
            }
            set {
                SetGrid(value);
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
                return;
            }
            tileRenderer.Load(game,this);
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
            if(!hasTileRenderer()) {
                return;
            }
            tileRenderer.Unload();
        }

        internal override void Update(GameTime gameTime) {
            /* TODO */
        }

        private void renderTiles(
            int startX,int startY,int width,int height,float renderX,float renderY,float tileSize
        ) {
            for(int x = 0;x < width;x++) {
                int gridX = x + startX;
                if(gridX < 0) {
                    continue;
                } else if(gridX >= gridWidth) {
                    break;
                }
                for(int y = 0;y < height;y++) {
                    int gridY = y + startY;
                    if(gridY < 0) {
                        continue;
                    } else if(gridY >= gridHeight) {
                        break;
                    }
                    var destination = Graphics2D.GetDestination(
                        renderX + x * tileSize,renderY + y * tileSize,tileSize
                    );
                    tileRenderer.RenderTile(grid[gridX,gridY],destination);
                }
            }
        }

        private struct ScreenSpace {
            internal float X;
            internal float Y;
            internal float Width;
            internal float Height;
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
        public (float,float) GetCoordinate(int screenX,int screenY) {
            var viewport = Graphics2D.GetViewport(game);
            var screenSpace = getScreenSpace(viewport);
            float x = (float)screenX / viewport.Width * screenSpace.Width;
            float y = (float)screenY / viewport.Height * screenSpace.Height;
            return (x + screenSpace.X, y + screenSpace.Y);
        }

        internal override void Draw(GameTime gameTime) {
            game.GraphicsDevice.Clear(Color.Black);

            if(!hasTileRenderer()) {
                return;
            }

            ScreenSpace screenSpace = getScreenSpace(Graphics2D.GetViewport(game));
            float tileSize = Camera.Scale * TileSize;

            int startX = (int)Math.Floor(screenSpace.X);
            int startY = (int)Math.Floor(screenSpace.Y);

            float xOffset = (startX - screenSpace.X) * tileSize;
            float yOffset = (startY - screenSpace.Y) * tileSize;

            int horizontalTiles = (int)Math.Ceiling(screenSpace.Width);
            if(xOffset * -2 > tileSize) horizontalTiles++;

            int verticalTiles = (int)Math.Ceiling(screenSpace.Height);
            if(yOffset * -2 > tileSize) verticalTiles++;

            game.SpriteBatch.Begin();
            renderTiles(startX,startY,horizontalTiles,verticalTiles,xOffset,yOffset,tileSize);
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
