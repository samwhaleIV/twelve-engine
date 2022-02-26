using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.UI.Elements {
    public class LabelButton:Button {

        public string Text { get; set; }
        public Color TextColor { get; set; } = Color.Black;

        private SpriteFont spriteFont;

        public LabelButton(Color color,string fontName) : base(color) {
            OnLoad += () => spriteFont = GetSpriteFont(fontName);
            OnRender += LabelButton_OnRender;
        }

        private void LabelButton_OnRender(GameTime gameTime) {
            var textLocation = ScreenArea.Center.ToVector2() - spriteFont.MeasureString(Text) * 0.5f;
            Game.SpriteBatch.DrawString(spriteFont,Text,textLocation,TextColor,0f,Vector2.Zero,1f,SpriteEffects.None,Depth);
        }
    }
}
