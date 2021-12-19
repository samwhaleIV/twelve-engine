using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.UI.Elements {
    public class Panel:RenderElement {

        private readonly Color color;
        private Texture2D texture;

        public Panel(Color color) {
            this.color = color;

            OnLoad += Panel_OnLoad;
            OnRender += Panel_OnRender;
        }

        private void Panel_OnLoad() => texture = GetColoredTexture(color);

        private void Panel_OnRender(GameTime _) => Draw(texture);

    }
}
