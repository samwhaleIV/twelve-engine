using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine {
    public class TestTileRenderer:ITileRenderer<int> {

        private const int SIZE = 32;

        private GameManager game;
        private Grid2D<int> grid2D;

        private Rectangle textureOrigin;
        Texture2D[] colorLookup;

        private Texture2D getColorTexture(Color color) {
            var texture = new Texture2D(this.game.GraphicsDevice,1,1);
            texture.SetData(new Color[] { color });
            return texture;
        }

        public void Load(GameManager game,Grid2D<int> grid2D) {
            this.game = game;
            this.grid2D = grid2D;

            colorLookup = new Texture2D[] {
                getColorTexture(Color.DimGray),
                getColorTexture(Color.DarkGray),
                getColorTexture(Color.Coral)
            };
            textureOrigin = new Rectangle(0,0,1,1);

            var grid = this.grid2D.CreateGrid(SIZE,SIZE);
            grid.Fill((x,y) => (x + y) % 2 == 0 ? 0 : 1);

            grid[0,0] = 2;
            grid[0,SIZE - 1] = 2;
            grid[SIZE - 1,SIZE - 1] = 2;
            grid[SIZE - 1,0] = 2;
        }

        public void RenderTile(int value,Rectangle destination) {
            Texture2D texture = colorLookup[value];
            game.SpriteBatch.Draw(texture,destination,textureOrigin,Color.White,0f,Vector2.Zero,SpriteEffects.None,1f);
        }

        public void Unload() {
            foreach(var texture in colorLookup) {
                texture.Dispose();
            }
        }
    }
}
