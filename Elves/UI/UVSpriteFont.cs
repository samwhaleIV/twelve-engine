using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Shell;

namespace Elves.UI {
    public sealed partial class UVSpriteFont {

        private const string TEXTURE_NAME = "UI/font";

        private const int LINE_HEIGHT = 38;
        private const int LETTER_SPACING = 1;
        private const int WORD_SPACING = 4;

        private Texture2D fontTexture;

        private string textureName;

        private GameManager game;

        public UVSpriteFont(string textureName = TEXTURE_NAME) {
            string lowercase = "abcdefghijklmnopqrstuvwxyz";
            foreach(char character in lowercase) {
                Glpyhs[character] = Glpyhs[char.ToUpperInvariant(character)];
            }
            this.textureName = textureName;
        }
        public void Load(GameManager game) {
            this.game = game;
            fontTexture = game.Content.Load<Texture2D>(textureName);
        }
        public void Begin() {
            game.SpriteBatch.Begin(SpriteSortMode.Deferred,BlendState.NonPremultiplied,SamplerState.PointClamp);
        }

        private Rectangle GlyphOrDefault(char character) {
            if(!Glpyhs.TryGetValue(character,out Rectangle value)) {
                value = new Rectangle();
            }
            return value;
        }

        private Queue<char[]> words = new Queue<char[]>();
        private Queue<char> wordBuffer = new Queue<char>();

        private int MeasureWordWidth(char[] word,int scale,int letterSpacing) {
            int width = 0;
            foreach(var character in word) {
                Rectangle glyph = GlyphOrDefault(character);
                width += glyph.Width * scale + letterSpacing;
            }
            width -= letterSpacing;
            return width;
        }

        private void TryFlushWordBuffer() {
            if(wordBuffer.Count > 0) {
                words.Enqueue(wordBuffer.ToArray());
                wordBuffer.Clear();
            }
        }

        private void FillWordsQueue(StringBuilder stringBuilder) {
            foreach(var character in stringBuilder.ToString()) {
                if(character == ' ') {
                    TryFlushWordBuffer();
                    continue;
                }
                wordBuffer.Enqueue(character);
            }
            TryFlushWordBuffer();
        }

        private void EmptyWordsQueue() {
            words.Clear();
        }

        private int DrawGlpyh(char character,int x,int y,int scale,Color color) {
            Rectangle glyph = GlyphOrDefault(character);
            Rectangle destination = new Rectangle(x,y,glyph.Width*scale,glyph.Height*scale);
            game.SpriteBatch.Draw(fontTexture,destination,glyph,color);
            return glyph.Width;
        }

        public void Draw(StringBuilder stringBuilder,Point destination,int scale,Color? color = null,int? maxWidth = null) {
            FillWordsQueue(stringBuilder);

            int x = destination.X, y = destination.Y;

            int lineHeight = LINE_HEIGHT * scale;

            Color glyphColor = color ?? Color.White;

            int wordSpacing = WORD_SPACING * scale;
            int letterSpacing = LETTER_SPACING * scale;

            foreach(var word in words) {
                if(maxWidth.HasValue && x + MeasureWordWidth(word,scale,letterSpacing) > maxWidth.Value) {
                    x = destination.X;
                    y += lineHeight;
                }
                foreach(var character in word) {
                    x += DrawGlpyh(character,x,y,scale,glyphColor) * scale + letterSpacing;
                }
                x = x - letterSpacing + wordSpacing;
            }

            EmptyWordsQueue();
        }
        
        public void DrawCentered(StringBuilder stringBuilder,Point center,int scale,Color? color = null) {
            FillWordsQueue(stringBuilder);

            int wordSpacing = WORD_SPACING * scale;
            int letterSpacing = LETTER_SPACING * scale;

            int totalWidth = 0;
            foreach(var word in words) {
                int width = MeasureWordWidth(word,scale,letterSpacing);
                totalWidth += width + wordSpacing;
            }
            totalWidth -= wordSpacing;

            int x = center.X - totalWidth / 2;
            int y = center.Y - LINE_HEIGHT * scale / 2;

            Color glyphColor = color ?? Color.White;

            foreach(var word in words) {
                foreach(var character in word) {
                    x += DrawGlpyh(character,x,y,scale,glyphColor) * scale + letterSpacing;
                }
                x = x - letterSpacing + wordSpacing;
            }

            EmptyWordsQueue();
        }
        
        public void End() {
            game.SpriteBatch.End();
        }
    }
}
