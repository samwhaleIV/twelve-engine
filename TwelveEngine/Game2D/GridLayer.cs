using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D {
    public partial class Grid2D {

        private void renderTiles(int[,] data,ScreenSpace screenSpace) {
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

            drawTiles(data,startX,startY,horizontalTiles,verticalTiles,xOffset,yOffset,screenSpace.TileSize);
        }

        private void drawTiles(
            int[,] data, int startX,int startY,int width,int height,float renderX,float renderY,float tileSize
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
            if(endX > Width) {
                width -= endX - Width;
            }
            if(endY > Height) {
                height -= endY - Height;
            }

            for(int x = xOffset;x < width;x++) {
                int gridX = x + startX;
                for(int y = yOffset;y < height;y++) {
                    int gridY = y + startY;
                    target.X = (int)Math.Floor(renderX + x * tileSize);
                    target.Y = (int)Math.Floor(renderY + y * tileSize);
                    tileRenderer.RenderTile(data[gridX,gridY],target);
                }
            }
        }
    }
}
