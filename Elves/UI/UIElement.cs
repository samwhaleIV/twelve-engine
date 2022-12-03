using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Shell;

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

        public virtual void Draw(SpriteBatch spriteBatch) {
            if(Texture == null) {
                return;
            }
            spriteBatch.Draw(Texture,Area,Color.White);
        }
    }
}
