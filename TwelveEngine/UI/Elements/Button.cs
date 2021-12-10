using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.UI.Elements {
    internal class Button:RenderElement {

        private readonly Color color;

        public Button(Color color) {
            this.color = color;
            OnLoad += Button_OnLoad;
            OnUnload += Button_OnUnload;
            IsInteractable = true;
        }

        private Texture2D texture;

        private void Button_OnLoad() {
            texture = new Texture2D(Game.GraphicsDevice,1,1);
            texture.SetData(new Color[] { color });
        }

        private void Button_OnUnload() {
            texture.Dispose();
        }

        public override void Render(GameTime gameTime) {
            Game.SpriteBatch.Draw(texture,RenderArea,Pressed ? Color.DarkSlateGray : Hovered ? Color.LightGray : Color.White);
        }
    }
}
