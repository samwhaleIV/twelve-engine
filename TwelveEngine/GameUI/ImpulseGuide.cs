using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using TwelveEngine.Input;
using TwelveEngine.Input.Glyphs;

namespace TwelveEngine.GameUI {
    public sealed class ImpulseGuide {

        public int GlyphScale { get; set; } = 2;
        public int Padding { get; set; } = 2;

        private const string GAMEPAD_GLYPHS = "controller-glyphs", KEYBOARD_GLYPHS = "keyboard-glyphs", GUIDE_FONT = "guide-font";
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

        private void renderGuide<TKey>(Texture2D texture,GlyphMap<TKey> map,Func<Impulse,TKey> getKey) {
            var location = new Point(Padding);
            var glyphSize = new Point(keyboardMap.GlyphSize * GlyphScale);
            for(int i = 0;i<descriptions.Length;i++) {
                var description = descriptions[i];
                var source = map.GetGlyph(getKey(description.Type));
                game.SpriteBatch.Draw(texture,new Rectangle(location,glyphSize),source,Color.White);

                var textLocation = (location + glyphSize).ToVector2();
                textLocation.Y += glyphSize.Y * -0.5f - 7f;
                textLocation.X += Padding + 1;
                game.SpriteBatch.DrawString(guideFont,description.Text,textLocation,Color.White);

                location.Y += glyphSize.Y + Padding;
            }
        }

        public void Render() {
            if(descriptions == null) {
                return;
            }
            switch(impulseHandler.InputMethod) {
                default: return;
                case InputMethod.Keyboard:
                    renderGuide(keyboardTexture,keyboardMap,impulseHandler.GetKeyboardBind);
                    return;
                case InputMethod.GamePad:
                    renderGuide(gamePadTexture,gamePadMap,impulseHandler.GetGamePadBind);
                    return;
            }
        }
    }
}
