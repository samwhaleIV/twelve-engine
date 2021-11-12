using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D.Entities {
    class TheRedBox:Entity,IRenderable,IUpdateable  {

        private Texture2D redBoxTexture;

        private Rectangle textureSource = new Rectangle(0,0,1,1);

        public override void Load() {
            FactoryID = "TheRedBox";
            redBoxTexture = new Texture2D(Game.GraphicsDevice,1,1);
            redBoxTexture.SetData(new Color[] { Color.Red });
        }

        public void Render(GameTime gameTime) {
            Game.SpriteBatch.Draw(redBoxTexture,Grid.GetDestination(this),textureSource,Color.White);
        }

        public override void Unload() {
            redBoxTexture.Dispose();
        }

        public void Update(GameTime gameTime) {
            X = (float)gameTime.TotalGameTime.TotalMilliseconds / 10000f;
            Y = (float)gameTime.TotalGameTime.TotalMilliseconds / 10000f;
        }
    }
}
