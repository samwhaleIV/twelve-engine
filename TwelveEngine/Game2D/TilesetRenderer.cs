using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D {
    class TilesetRenderer:ITileRenderer {

        private Texture2D tileset;

        private int tilesetColumns;
        private int tileSize;

        private GameManager game;

        public void Load(GameManager game,Grid2D grid2D) {
            tileset = game.Content.Load<Texture2D>(Constants.Tileset);
            tileSize = grid2D.TileSize;
            tilesetColumns = tileset.Width / tileSize;
            this.game = game;
        }

        public void Unload() {
            tileset.Dispose();
        }

        public void RenderTile(int value,Rectangle destination) {
            if(value <= 1) {
                return;
            }
            value--;
            var source = new Rectangle();
            source.X = (value % tilesetColumns) * tileSize;
            source.Y = (value / tilesetColumns) * tileSize;
            source.Width = tileSize;
            source.Height = tileSize;
            game.SpriteBatch.Draw(tileset,destination,source,Color.White);
        }
    }
}
