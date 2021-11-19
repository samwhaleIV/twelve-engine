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

        public void Update(GameTime gameTime) {
            this.X = (float)gameTime.TotalGameTime.TotalMilliseconds / 2000f;
            this.Y = (float)gameTime.TotalGameTime.TotalMilliseconds / 3400f;
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
