using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D {
    class TilesetRenderer:ITileRenderer {

        private Texture2D tileset;
        private SpriteBatch spriteBatch;

        private Rectangle[] textureSources = new Rectangle[0];

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

        public void Load(GameManager game,Grid2D grid) {
            tileset = game.Content.Load<Texture2D>(Constants.Tileset);
            int tileSize = grid.TileSize;
            spriteBatch = game.SpriteBatch;

            int rows = tileset.Height / tileSize;
            int columns = tileset.Width / tileSize;

            textureSources = getTextureSources(rows * columns,columns,tileSize);
        }

        public void Unload() => spriteBatch = null;

        public void RenderTile(int value,Rectangle destination) {
            if(value < 1) return;
            spriteBatch.Draw(tileset,destination,textureSources[value],Color.White);
        }
        public void RenderTileDepth(int value,Rectangle destination,float viewportHeight) {
            if(value < 1) return;
            float depth = 1 - Math.Max(destination.Y / viewportHeight,0);
            spriteBatch.Draw(tileset,destination,textureSources[value],Color.White,0f,Vector2.Zero,SpriteEffects.None,depth);
        }
    }
}
