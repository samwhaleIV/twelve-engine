using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D {
    public sealed class TilesetRenderer:TileRenderer {

        public TilesetRenderer() {
            OnLoad += TilesetRenderer_OnLoad;
        }

        private Texture2D tileset;
        private SpriteBatch spriteBatch;
        private Rectangle[] textureSources;

        private void TilesetRenderer_OnLoad() {
            tileset = Game.Content.Load<Texture2D>(Constants.Tileset);
            int tileSize = Grid.TileSize;
            spriteBatch = Game.SpriteBatch;

            int rows = tileset.Height / tileSize;
            int columns = tileset.Width / tileSize;

            textureSources = getTextureSources(rows * columns,columns,tileSize);
        }

        private static Rectangle[] getTextureSources(int count,int columns,int size) {
            var sources = new Rectangle[count];
            for(int i = 0; i < sources.Length;i++) {
                var source = new Rectangle();
                source.X = (i % columns) * size;
                source.Y = (i / columns) * size;
                source.Width = size;
                source.Height = size;
                sources[i] = source;
            }
            return sources;
        }

        public override void RenderTiles(ScreenSpace screenSpace,int[,] data) {
            int tileSize = screenSpace.TileSize;

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
            if(endX > Grid.Width) width -= endX - Grid.Width;
            if(endY > Grid.Height) height -= endY - Grid.Height;

            var target = new Rectangle(0,0,tileSize,tileSize);

            float depthBase = 1f / Grid.Viewport.Height;
            int gridX, value, y;
            float depth;

            for(int x = xOffset;x < width;x++) {
                gridX = x + startX;
                target.X = tileX + x * tileSize;
                for(y = yOffset;y < height;y++) {
                    value = data[gridX,y + startY];
                    if(value < 1) continue;

                    target.Y = tileY + y * tileSize;

                    depth = target.Y * depthBase;
                    if(depth < 0) depth = 0;

                    spriteBatch.Draw(tileset,target,textureSources[value],Color.White,0f,Vector2.Zero,SpriteEffects.None,depth);
                }
            }
        }
    }
}
