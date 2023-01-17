using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine;

namespace Elves.Scenes.Battle.UI {
    public class UIElement {

        protected Texture2D Texture { get; set; } = Program.Textures.Nothing;

        public VectorRectangle Area {
            get => GetArea();
            set => SetArea(value);
        }

        private VectorRectangle _area;

        protected virtual VectorRectangle GetArea() {
            return _area;
        }
        protected virtual void SetArea(VectorRectangle rectangle) {
            _area = rectangle;
        }

        public virtual void Draw(SpriteBatch spriteBatch,Color? color = null) {
            if(Texture == null) {
                return;
            }
            spriteBatch.Draw(Texture,(Rectangle)Area,color ?? Color.White);
        }
    }
}
