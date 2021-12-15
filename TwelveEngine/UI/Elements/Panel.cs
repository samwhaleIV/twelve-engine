using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.UI.Elements {
    public class Panel:RenderElement {
        private Texture2D texture;

        public Panel(Color color) => OnLoad += () => texture = GetColoredTexture(color);

        public override void Render(GameTime _) => Draw(texture);
    }
}
