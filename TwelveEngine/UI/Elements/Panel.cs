using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.UI.Elements {
    public class Panel:RenderElement {

        private readonly Color color;

        public Panel(Color color) {
            this.color = color;
            OnLoad += Panel_OnLoad;
            OnUnload += Panel_OnUnload;
        }

        private Texture2D texture;

        private void Panel_OnLoad() {
            texture = new Texture2D(Game.GraphicsDevice,1,1);
            texture.SetData(new Color[] { color });
        }

        private void Panel_OnUnload() => texture.Dispose();

        public override void Render(GameTime gameTime) {
            Game.SpriteBatch.Draw(texture,RenderArea,GetRenderColor());
        }
    }
}
