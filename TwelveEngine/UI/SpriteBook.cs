using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.UI {
    public class SpriteBook:Book<SpriteElement> {

        private SpriteBatch spriteBatch = null;

        protected override void RenderElement(SpriteElement element) {
            element.Render(spriteBatch);
        }

        public void Render(SpriteBatch spriteBatch) {
            this.spriteBatch = spriteBatch;
            spriteBatch.Begin(SpriteSortMode.FrontToBack,null,SamplerState.PointClamp);
            RenderElements();
            spriteBatch.End();
            this.spriteBatch = null;
        }
    }
}
