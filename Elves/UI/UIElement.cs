using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Shell;

namespace Elves.UI {
    public class UIElement {

        private Texture2D _texture = null;

        public void SetTexture(GameManager game,string textureName = "UI/nothing") {
            _texture = game.Content.Load<Texture2D>(textureName);
        }

        public void SetTexture(Texture2D texture) {
            _texture = texture;
        }

        protected Texture2D Texture => _texture;

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
            if(_texture == null) {
                return;
            }
            spriteBatch.Draw(_texture,Area,Color.White);
        }
    }
}
