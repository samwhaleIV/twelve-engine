using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TwelveEngine.UI {
    public class SpriteBook:Book<SpriteElement> {

        private SpriteBatch spriteBatch = null;

        public void Render(SpriteBatch spriteBatch) {
            this.spriteBatch = spriteBatch;
            spriteBatch.Begin(SpriteSortMode.FrontToBack,null,SamplerState.PointClamp);
            foreach(var element in Elements) {
                if(!element.TextureSource.HasValue || element.ComputedArea.Destination.Size == Vector2.Zero) {
                    return;
                }
                element.Render(spriteBatch);
            }
            spriteBatch.End();
            this.spriteBatch = null;
        }
    }
}
