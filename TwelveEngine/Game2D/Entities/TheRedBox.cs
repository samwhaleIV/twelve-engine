using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D.Entities {
    class TheRedBox:Entity,IRenderable  {

        private Texture2D redBoxTexture;

        private Rectangle textureSource = new Rectangle(0,0,1,1);

        public override void Load() {
            FactoryID = "TheRedBox";
            redBoxTexture = new Texture2D(Game.GraphicsDevice,1,1);
            redBoxTexture.SetData(new Color[] { Color.Red });
        }

        public void Render(GameTime gameTime) {
            if(Grid.OnScreen(this)) {
                var destination = Grid.GetDestination(this);
                Game.SpriteBatch.Draw(redBoxTexture,destination,textureSource,Color.White);
            }
        }

        public override void Unload() {
            redBoxTexture.Dispose();
        }
    }
}
