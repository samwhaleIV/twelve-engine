using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D {
    public class TestTileRenderer:ITileRenderer {

        private readonly TrackedGrid grid;

        public TestTileRenderer(TrackedGrid grid) {
            this.grid = grid;
        }

        private GameManager game;

        private Rectangle textureOrigin;
        Texture2D[] colorLookup;

        private Texture2D getColorTexture(Color color) {
            var texture = new Texture2D(this.game.GraphicsDevice,1,1);
            texture.SetData(new Color[] { color });
            return texture;
        }

        public void Load(GameManager game,Grid2D grid2D) {
            this.game = game;

            colorLookup = new Texture2D[] {
                getColorTexture(Color.DimGray),
                getColorTexture(Color.DarkGray),
                getColorTexture(Color.Coral)
            };
            textureOrigin = new Rectangle(0,0,1,1);

            grid.Fill((x,y) => (x + y) % 2 == 0 ? 0 : 1);
            grid[0,0] = 2;
            grid[0,grid.Width - 1] = 2;
            grid[grid.Width - 1,grid.Height - 1] = 2;
            grid[grid.Width - 1,0] = 2;
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
