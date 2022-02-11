using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.UI.Elements {
    public class ImagePanel:RenderElement {

        private Texture2D texture;
        private Rectangle source;

        protected Texture2D Texture => texture;

        private void AddRenderer(Rectangle? source) {
            if(source.HasValue) {
                this.source = source.Value;
                OnRender += _ => Draw(texture,this.source);
            } else {
                OnRender += _ => Draw(texture);
            }
        }

        public ImagePanel(string imageName,Rectangle? source = null) {
            OnLoad += () => texture = GetImage(imageName);
            AddRenderer(source);
        }

        public ImagePanel(Texture2D texture,Rectangle? source = null) {
            this.texture = texture;
            AddRenderer(source);
        }

    }
}
