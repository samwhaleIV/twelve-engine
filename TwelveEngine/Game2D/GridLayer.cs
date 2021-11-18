using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D {
    public class GridLayer:ISerializable {

        private readonly ITileRenderer tileRenderer;
        private TrackedGrid grid = null;

        private readonly int tileSize;
        private readonly GameManager game;

        private bool refreshTileBuffer = false;
        private readonly Stack<Rectangle> tileRefreshAreas = new Stack<Rectangle>();

        private bool hasGrid() {
            return grid != null;
        }

        private void bindGrid() {
            if(!hasGrid()) {
                return;
            }
            grid.Invalidated = gridInvalidated;
            grid.ValueChanged = gridValueChanged;
        }
        private void unbindGrid() {
            if(!hasGrid()) {
                return;
            }
            grid.Invalidated = null;
            grid.ValueChanged = null;
        }

        private void setGrid(TrackedGrid grid) {
            unbindGrid();
            this.grid = grid;
            bindGrid();
            signalBufferFlush(true);
        }

        public GridLayer(GameManager game,ITileRenderer tileRenderer,int tileSize) {
            this.tileRenderer = tileRenderer;
            this.tileSize = tileSize;
            this.game = game;
        }

        public TrackedGrid Grid {
            get {
                return grid;
            }
            set {
                setGrid(value);
            }
        }

        private void signalBufferFlush(bool redraw) {
            tileRefreshAreas.Clear();
            refreshTileBuffer = redraw;
        }

        private RenderTarget2D tileBuffer = null;
        private bool hasTileBuffer() {
            return tileBuffer != null;
        }

        private void gridValueChanged(Rectangle rectangle) {
            tileRefreshAreas.Push(rectangle);
        }
        private void gridInvalidated() {
            refreshTileBuffer = true;
        }

        private void createTileBuffer() {
            if(hasTileBuffer()) {
                tileBuffer.Dispose();
                tileBuffer = null;
            }

            int textureWidth = grid.Width * tileSize;
            int textureHeight = grid.Height * tileSize;

            tileBuffer = new RenderTarget2D(game.GraphicsDevice,textureWidth,textureHeight);

            signalBufferFlush(true);
        }
        private void deleteTileBuffer() {
            if(!hasTileBuffer()) {
                return;
            }
            tileBuffer.Dispose();
            tileBuffer = null;

            signalBufferFlush(false);
        }

        public void UpdateBuffer() {
            if(!hasTileBuffer()) {
                return;
            }
            bool redrawAll = tileBuffer.IsContentLost || refreshTileBuffer;
            bool redrawSome = tileRefreshAreas.Count > 0;

            if(!(redrawAll || redrawSome))
                return;

            game.GraphicsDevice.SetRenderTarget(tileBuffer);
            game.SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);

            if(redrawAll) {
                renderTiles(new Rectangle(0,0,grid.Width,grid.Height));
            } else {
                while(tileRefreshAreas.TryPeek(out Rectangle area)) {
                    renderTiles(area);
                    tileRefreshAreas.Pop();
                }
            }

            game.SpriteBatch.End();
            game.GraphicsDevice.SetRenderTarget(null);
            refreshTileBuffer = false;
        }

        public void Render(Rectangle destination,ScreenSpace screenSpace) {
            if(!hasTileBuffer()) {
                return;
            }

            var source = new Rectangle(
                (int)(screenSpace.X * tileSize),
                (int)(screenSpace.Y * tileSize),
                (int)(screenSpace.Width * tileSize),
                (int)(screenSpace.Height * tileSize)
            );

            game.SpriteBatch.Draw(tileBuffer,destination,source,Color.White,0,Vector2.Zero,SpriteEffects.None,1f);
        }

        private Rectangle getTileDestination(int x,int y) {
            return new Rectangle(x * tileSize,y * tileSize,tileSize,tileSize);
        }

        private void renderTiles(Rectangle area) {
            int endX = area.X + area.Width;
            int endY = area.Y + area.Height;
            for(int x = area.X;x < endX;x++) {
                for(int y = area.Y;y < endY;y++) {
                    var destination = getTileDestination(x,y);
                    tileRenderer.RenderTile(grid[x,y],destination);
                }
            }
        }

        private bool loaded = false;
        public bool Loaded => loaded;

        public void Load(GameManager game,Grid2D grid) {
            if(loaded) {
                return;
            }
            tileRenderer.Load(game,grid);
            createTileBuffer();
            bindGrid();
            loaded = true;
        }
        public void Unload() {
            if(!loaded) {
                return;
            }
            tileRenderer.Unload();
            deleteTileBuffer();
            unbindGrid();
            loaded = false;
        }

        public void Export(SerialFrame frame) {
            frame.SetArray2D("Data",grid.Data);
        }

        public void Import(SerialFrame frame) {
            var gridData = frame.GetArray2D<int>("Data");
            setGrid(new TrackedGrid(gridData));
        }
    }
}
