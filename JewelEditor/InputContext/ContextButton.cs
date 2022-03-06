using TwelveEngine.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JewelEditor.Entity;

namespace JewelEditor.InputContext {
    internal sealed class ContextButton:ImageButton {

        private readonly JewelEntity owner;
        private readonly ButtonData source;

        private static Rectangle selectionRectangle = new Rectangle(0,32,16,16);

        public ContextButton(JewelEntity owner,ButtonData source) : base(Editor.Tileset,source.TextureSource) {
            this.owner = owner;
            this.source = source;
            OnClick += SelectorUI_OnClick;
            OnRender += SelectorUI_OnRender;
        }

        private void SelectorUI_OnRender(GameTime gameTime) {
            var state = owner.GetState();
            if(state.InputMode != source.InputMode) {
                return;
            }
            if(source.InputMode == InputMode.Tile && source.TileType != state.TileType) {
                return;
            }

            var lineArea = ScreenArea;
            lineArea.X -= 4;
            lineArea.Y -= 4;
            lineArea.Width += 8;
            lineArea.Height += 8;
            Game.SpriteBatch.Draw(Texture,lineArea,selectionRectangle,Color.White,0f,Vector2.Zero,SpriteEffects.None,0.25f);
        }

        private void SelectorUI_OnClick(ImageButton _) {
            var state = owner.GetState();
            state.InputMode = source.InputMode;
            var tileType = source.TileType;
            if(!tileType.HasValue) {
                return;
            }
            state.TileType = tileType.Value;
        }
    }
}
