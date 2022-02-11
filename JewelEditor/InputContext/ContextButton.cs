using TwelveEngine.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JewelEditor.Entity;

namespace JewelEditor.InputContext {
    internal sealed class ContextButton:ImageButton {

        private readonly UIEntity owner;
        private readonly ButtonData source;

        private Texture2D texture;

        private static Color OuterColor = Color.FromNonPremultiplied(198,0,49,255);
        private static Color InnerColor = Color.FromNonPremultiplied(227,0,47,255);

        public ContextButton(UIEntity owner,ButtonData source) : base(Editor.Tileset,source.TextureSource) {
            this.owner = owner;
            this.source = source;
            OnClick += SelectorUI_OnClick;
            OnRender += SelectorUI_OnRender;
            OnLoad += SelectorUI_OnLoad;
        }

        private void SelectorUI_OnLoad() {
            texture = GetColoredTexture(Color.White);
        }

        private void SelectorUI_OnRender(GameTime gameTime) {
            var state = owner.State;
            if(state.InputMode != source.InputMode) {
                return;
            }
            if(source.InputMode == InputMode.Tile && source.TileType != state.TileType) {
                return;
            }

            var lineArea = ScreenArea;
            lineArea.Height = 6;
            lineArea.Width += 4;
            lineArea.X -= 2;
            lineArea.Y = ScreenArea.Bottom + 4;
            Game.SpriteBatch.Draw(texture,lineArea,null,OuterColor,0f,Vector2.Zero,SpriteEffects.None,Depth);
            lineArea.Width -= 4;
            lineArea.Height -= 4;
            lineArea.X += 2;
            lineArea.Y += 2;
            Game.SpriteBatch.Draw(texture,lineArea,null,InnerColor,0f,Vector2.Zero,SpriteEffects.None,Depth);
        }

        private void SelectorUI_OnClick(ImageButton _) {
            var state = owner.State;
            state.InputMode = source.InputMode;
            var tileType = source.TileType;
            if(!tileType.HasValue) {
                return;
            }
            state.TileType = tileType.Value;
        }
    }
}
