using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.UI.Elements {
    public class Label:RenderElement {

        public string Text { get; set; } = string.Empty;
        public float Scale { get; set; } = 1f;
        public float Rotation { get; set; } = 0f;

        private Color Color { get; set; } = Color.Black;
        public Vector2 Origin { get; set; } = Vector2.Zero;
        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

        private SpriteFont spriteFont;

        public Label(string fontName) {
            OnLoad += () => spriteFont = GetSpriteFont(fontName);
            OnRender += Label_OnRender;
        }

        private void Label_OnRender(GameTime gameTime) {
            Game.SpriteBatch.DrawString(spriteFont,Text,ScreenArea.Location.ToVector2(),Color,Rotation,Origin,Scale,SpriteEffects,Depth);
        }

    }
}
