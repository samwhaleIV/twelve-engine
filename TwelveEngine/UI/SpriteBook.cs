using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TwelveEngine.UI {
    public class SpriteBook:Book<SpriteElement> {
        public void Render(SpriteBatch spriteBatch) {
            spriteBatch.Begin(SpriteSortMode.FrontToBack,null,SamplerState.PointClamp);
            foreach(var element in Elements) {
                if(!element.TextureSource.HasValue || element.ComputedArea.Destination.Size == Vector2.Zero) {
                    continue;
                }
                element.Render(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
