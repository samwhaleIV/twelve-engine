using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using TwelveEngine.Input;
using TwelveEngine.Input.Glyphs;

namespace TwelveEngine.GameUI {
    public sealed class ImpulseGuide {

        private const string GAMEPAD_GLYPHS = "ImpulseGuide/controller-glyphs";
        private const string KEYBOARD_GLYPHS = "ImpulseGuide/keyboard-glyphs";
        private const string GUIDE_FONT = "ImpulseGuide/impulse-guide-font";

        public int GlyphScale { get; set; } = 2;

        public int ScreenEdgePadding { get; set; } = Constants.ScreenEdgePadding;
        public int VerticalPadding { get; set; } = 4;
        public int TextPadding { get; set; } = 4;

        private Texture2D gamePadTexture, keyboardTexture;

        private readonly ImpulseHandler impulseHandler;

        private readonly GamePadMap gamePadMap = new GamePadMap();
        private readonly KeyboardMap keyboardMap = new KeyboardMap();

        private readonly GameManager game;

        private SpriteFont guideFont;

        public ImpulseGuide(GameManager game) {
            this.game = game;
            impulseHandler = game.ImpulseHandler;
            gamePadMap.LoadGlyphs();
            keyboardMap.LoadGlyphs();
        }

        public void Load() {
            gamePadTexture = game.Content.Load<Texture2D>(GAMEPAD_GLYPHS);
            keyboardTexture = game.Content.Load<Texture2D>(KEYBOARD_GLYPHS);
            guideFont = game.Content.Load<SpriteFont>(GUIDE_FONT);
        }

        private (Impulse Type, string Text)[] descriptions = null;
        public void SetDescriptions(params (Impulse Type,string Text)[] descriptions) {
            if(descriptions.Length < 1) {
                descriptions = null;
            }
            this.descriptions = descriptions;
        }

        private Stack<(Impulse,string)[]> descriptionStack = new Stack<(Impulse,string)[]>();

        public void ClearDescriptions() => descriptions = null;

        public void SaveDescriptions() {
            descriptionStack.Push(descriptions);
        }
        public void RestoreDescriptions() {
            if(descriptionStack.TryPop(out var result)) {
                descriptions = result;
            }
        }

        private int calculateStackHeight(int glyphSize) {
            return (glyphSize + VerticalPadding) * descriptions.Length - VerticalPadding;
        }

        private void renderGuide<TKey>(Texture2D texture,GlyphMap<TKey> map,Func<Impulse,TKey> getKey,Point sourceOffset) {
            Point glyphSize = new Point(map.GlyphSize * GlyphScale);
            Point location = new Point(ScreenEdgePadding,(int)(game.GraphicsDevice.Viewport.Height * 0.5f - calculateStackHeight(glyphSize.Y) * 0.5f));

            for(int i = 0;i<descriptions.Length;i++) {
                var description = descriptions[i];
                Rectangle source = map.GetGlyph(getKey(description.Type));
                source.Location += sourceOffset;

                game.SpriteBatch.Draw(texture,new Rectangle(location,glyphSize),source,Color.White);

                var textLocation = (location + glyphSize).ToVector2();
                textLocation.Y += glyphSize.Y * -0.5f - 6f;
                textLocation.X += TextPadding;
                game.SpriteBatch.DrawString(guideFont,description.Text,textLocation,Color.White);

                location.Y += glyphSize.Y + VerticalPadding;
            }
        }

        private Point getGamepadBlockOffset() {
            switch(impulseHandler.GamePadType) {
                default:
                case GamePadType.MicrosoftXbox:
                    return Point.Zero;
                case GamePadType.SonyPlaystation:
                    return new Point(gamePadMap.BlockSize,0);
                case GamePadType.NintendoSwitch:
                    return new Point(0,gamePadMap.BlockSize);
            }
        }

        public void Render() {
            if(descriptions == null) {
                return;
            }
            switch(impulseHandler.InputMethod) {
                default: return;
                case InputMethod.Keyboard:
                    renderGuide(keyboardTexture,keyboardMap,impulseHandler.GetKeyboardBind,Point.Zero);
                    return;
                case InputMethod.GamePad:
                    renderGuide(gamePadTexture,gamePadMap,impulseHandler.GetGamePadBind,getGamepadBlockOffset());
                    return;
            }
        }
    }
}
