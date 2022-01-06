using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D.Entity.Types {
    public sealed class RedBox:Entity2D, IRenderable {

        public RedBox() {
            OnLoad += RedBox_OnLoad;
            OnUnload += RedBox_OnUnload;
        }

        private void RedBox_OnLoad() {
            texture = new Texture2D(Game.GraphicsDevice,1,1);
            texture.SetData(new Color[] { Color.Red });
        }
        private void RedBox_OnUnload() {
            texture.Dispose();
        }

        protected override int GetEntityType() => Entity2DType.RedBox;

        private Texture2D texture;
        private Rectangle source = new Rectangle(0,0,1,1);


        public void Render(GameTime gameTime) {
            if(!OnScreen()) {
                return;
            }
            Draw(texture,source);
        }

    }
}
