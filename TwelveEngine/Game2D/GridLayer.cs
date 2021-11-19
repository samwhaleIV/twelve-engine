using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D {
    public class GridLayer:ISerializable {

        private readonly ITileRenderer tileRenderer;

        private int[,] gridData;
        private int width;
        private int height;

        private void setGrid(int[,] gridData) {
            this.gridData = gridData;
            width = gridData.GetLength(0);
            height = gridData.GetLength(1);
        }

        public int[,] Data {
            get {
                return gridData;
            }
            set {
                setGrid(value);
            }
        }
        public int Width => width;
        public int Height => height;

        public void Fill(Func<int,int,int> pattern) {
            for(var x = 0;x<width;x++) {
                for(var y = 0;y<height;y++) {
                    gridData[x,y] = pattern(x,y);
                }
            }
        }

        public GridLayer(ITileRenderer tileRenderer) {
            this.tileRenderer = tileRenderer;
        }

        public void Render(ScreenSpace screenSpace) {
            int startX = (int)Math.Floor(screenSpace.X);
            int startY = (int)Math.Floor(screenSpace.Y);

            float xOffset = startX - screenSpace.X;
            float yOffset = startY - screenSpace.Y;

            float tileSize = screenSpace.TileSize;

            int horizontalTiles = (int)Math.Ceiling(screenSpace.Width - xOffset);
            if(xOffset * -2 > tileSize) horizontalTiles++;

            int verticalTiles = (int)Math.Ceiling(screenSpace.Height - yOffset);
            if(yOffset * -2 > tileSize) verticalTiles++;

            xOffset *= tileSize; yOffset *= tileSize;

            renderTiles(startX,startY,horizontalTiles,verticalTiles,xOffset,yOffset,screenSpace.TileSize);
        }

        private void renderTiles(
            int startX,int startY,int width,int height,float renderX,float renderY,float tileSize
        ) {
            int renderSize = (int)Math.Ceiling(tileSize);
            if(renderSize % 2 == 1) renderSize++;
            Rectangle target = new Rectangle(0,0,renderSize,renderSize);

            int xOffset = 0, yOffset = 0;
            if(startX < 0) {
                xOffset = -startX;
            }
            if(startY < 0) {
                yOffset = -startY;
            }

            int endX = startX + width, endY = startY + height;
            if(endX > this.width) {
                width -= endX - this.width;
            }
            if(endY > this.height) {
                height -= endY - this.height;
            }

            for(int x = xOffset;x < width;x++) {
                int gridX = x + startX;
                for(int y = yOffset;y < height;y++) {
                    int gridY = y + startY;
                    target.X = (int)Math.Floor(renderX + x * tileSize);
                    target.Y = (int)Math.Floor(renderY + y * tileSize);
                    tileRenderer.RenderTile(gridData[gridX,gridY],target);
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
            loaded = true;
        }
        public void Unload() {
            if(!loaded) {
                return;
            }
            tileRenderer.Unload();
            loaded = false;
        }

        public void Export(SerialFrame frame) {
            frame.SetArray2D("Data",gridData);
        }

        public void Import(SerialFrame frame) {
            setGrid(frame.GetArray2D<int>("Data"));
        }
    }
}
