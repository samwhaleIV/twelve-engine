using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elves.UI.Font {
    public sealed class UVSpriteFont {

        private readonly int lineHeight, letterSpacing, wordSpacing;
        private readonly Texture2D texture;

        private readonly Dictionary<char,Rectangle> glyphs;

        public UVSpriteFont(Texture2D texture,int lineHeight,int letterSpacing,int wordSpacing,Dictionary<char,Rectangle> glyphs) {
            this.texture = texture;
            this.lineHeight = lineHeight;
            this.letterSpacing = letterSpacing;
            this.wordSpacing = wordSpacing;
            this.glyphs = glyphs;
        }

        private Rectangle GlyphOrDefault(char character) {
            if(!glyphs.TryGetValue(character,out Rectangle value)) {
                value = new Rectangle();
            }
            return value;
        }

        private SpriteBatch spriteBatch = null;

        public void Begin(SpriteBatch spriteBatch) {
            if(this.spriteBatch != null) {
                this.spriteBatch.End();
            }
            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.NonPremultiplied,SamplerState.PointClamp);
            this.spriteBatch = spriteBatch;
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
            spriteBatch.Draw(texture,destination,glyph,color);
            return glyph.Width;
        }

        public void Draw(StringBuilder stringBuilder,Point destination,int scale,Color? color = null,int? maxWidth = null) {
            if(spriteBatch == null) {
                return;
            }

            FillWordsQueue(stringBuilder);

            int x = destination.X, y = destination.Y;

            int lineHeight = this.lineHeight * scale;

            Color glyphColor = color ?? Color.White;

            int wordSpacing = this.wordSpacing * scale;
            int letterSpacing = this.letterSpacing * scale;

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

            int wordSpacing = this.wordSpacing * scale;
            int letterSpacing = this.letterSpacing * scale;

            int totalWidth = 0;
            foreach(var word in words) {
                int width = MeasureWordWidth(word,scale,letterSpacing);
                totalWidth += width + wordSpacing;
            }
            totalWidth -= wordSpacing;

            int x = center.X - totalWidth / 2;
            int y = center.Y - lineHeight * scale / 2;

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
            if(spriteBatch == null) {
                return;
            }
            spriteBatch.End();
            spriteBatch = null;
        }
    }
}
