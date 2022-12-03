using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Shell;

namespace Elves.UI {
    public class UIElement {

        private GameManager game;

        protected GameManager Game => game;
        protected SpriteBatch SpriteBatch => game.SpriteBatch;

        internal void Load(GameManager game) {
            this.game = game;
            if(Texture != null) {
                return;
            }
            Texture = game.Content.Load<Texture2D>(PendingTexture);
            PendingTexture = null;
        }

        protected string PendingTexture { get; set; } = "UI/nothing";

        protected Texture2D Texture { get; set; }

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

        public virtual void Draw() {
            if(Texture == null) {
                return;
            }
            SpriteBatch.Draw(Texture,Area,Color.White);
        }
    }
}
