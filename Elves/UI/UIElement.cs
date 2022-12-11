using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Elves.UI {
    public class UIElement {

        protected Texture2D Texture { get; set; } = UITextures.Nothing;

        public Rectangle Area {
            get => GetArea();
            set => SetArea(value);
        }

        private Rectangle _area;

        protected virtual Rectangle GetArea() {
            return _area;
        }
        protected virtual void SetArea(Rectangle rectangle) {
            _area = rectangle;
        }

        public virtual void Draw(SpriteBatch spriteBatch,Color? color = null) {
            if(Texture == null) {
                return;
            }
            spriteBatch.Draw(Texture,Area,color ?? Color.White);
        }
    }
}
