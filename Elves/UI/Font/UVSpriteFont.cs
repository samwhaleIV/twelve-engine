using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elves.UI.Font {
    public sealed class UVSpriteFont {

        private const int CHARACTER_QUEUE_SIZE = 16;
        private const int CHARACTER_QUEUE_COUNT = 64;

        private readonly int lineHeight, letterSpacing, wordSpacing;
        private readonly Texture2D texture;

        private readonly Dictionary<char,Glyph> glyphs;

        private int characterQueueStartSize;

        public int LineHeight => lineHeight;
        public int LeterSpacing => letterSpacing;
        public int WordSpacing => wordSpacing;

        public UVSpriteFont(
            Texture2D texture,
            int lineHeight,
            int letterSpacing,
            int wordSpacing,
            Dictionary<char,Glyph> glyphs,
            int? wordQueueSize = null,
            int? wordQueuePoolSize = null
        ) {
            this.texture = texture;
            this.lineHeight = lineHeight;
            this.letterSpacing = letterSpacing;
            this.wordSpacing = wordSpacing;
            this.glyphs = glyphs;

            int characterQueueSize = wordQueueSize ?? CHARACTER_QUEUE_SIZE;
            int characterQueueCount = wordQueuePoolSize ?? CHARACTER_QUEUE_COUNT;

            characterQueueStartSize = characterQueueSize;

            for(int i = 0;i<characterQueueCount;i++) {
                characterQueuePool.Enqueue(new Queue<char>(characterQueueSize));
            }
        }

        private Glyph GlyphOrDefault(char character) {
            if(!glyphs.TryGetValue(character,out Glyph value)) {
                value = new Glyph();
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

        private readonly Queue<Queue<char>> characterQueuePool = new Queue<Queue<char>>();

        private Queue<char> GetPooledCharacterQueue() {
            Queue<char> queue;
            if(characterQueuePool.Count <= 0) {
                queue = new Queue<char>(characterQueueStartSize);
            } else {
                queue = characterQueuePool.Dequeue();
            }
            return queue;
        }

        private void ReturnPooledCharacterQueue(Queue<char> characterQueue) {
            characterQueue.Clear();
            characterQueuePool.Enqueue(characterQueue);
        }

        private readonly Queue<Queue<char>> words = new Queue<Queue<char>>();
        private readonly Queue<char> wordBuffer = new Queue<char>();

        private int MeasureWordWidth(Queue<char> word,int scale,int letterSpacing) {
            int width = 0;
            foreach(var character in word) {
                Glyph glyph = GlyphOrDefault(character);
                width += glyph.Source.Width * scale + letterSpacing;
            }
            width -= letterSpacing;
            return width;
        }

        private void TryFlushWordBuffer() {
            if(wordBuffer.Count > 0) {
                var characterQueue = GetPooledCharacterQueue();
                foreach(char character in wordBuffer) {
                    characterQueue.Enqueue(character);
                }
                words.Enqueue(characterQueue);
                wordBuffer.Clear();
            }
        }

        private void FillWordsQueue(StringBuilder stringBuilder) {
            for(int i = 0;i<stringBuilder.Length;i++) {
                char character = stringBuilder[i];
                if(character == ' ') {
                    TryFlushWordBuffer();
                    continue;
                }
                wordBuffer.Enqueue(character);
            }
            TryFlushWordBuffer();
        }

        private void EmptyWordsQueue() {
            foreach(var characterQueue in words) {
                ReturnPooledCharacterQueue(characterQueue);
            }
            words.Clear();
        }

        private int DrawGlyph(char character,int x,int y,int scale,Color color) {
            Glyph glyph = GlyphOrDefault(character);
            Rectangle glyphArea = glyph.Source;
            Rectangle destination = new Rectangle(x,y+glyph.YOffset*scale,glyphArea.Width*scale,glyphArea.Height*scale);
            spriteBatch.Draw(texture,destination,glyphArea,color);
            return destination.Width;
        }

        public void Draw(StringBuilder stringBuilder,Point destination,int scale,Color? color = null,int? maxWidth = null) {
            if(spriteBatch == null) {
                return;
            }
            if(scale < 1) {
                scale = 1;
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
                    x += DrawGlyph(character,x,y,scale,glyphColor) + letterSpacing;
                }
                x = x - letterSpacing + wordSpacing;
            }

            EmptyWordsQueue();
        }

        public void DrawRight(StringBuilder stringBuilder,Point destination,int scale,Color? color = null) {
            if(spriteBatch == null) {
                return;
            }
            if(scale < 1) {
                scale = 1;
            }

            FillWordsQueue(stringBuilder);

            int wordSpacing = this.wordSpacing * scale;
            int letterSpacing = this.letterSpacing * scale;

            int totalWidth = 0;
            foreach(var word in words) {
                int width = MeasureWordWidth(word,scale,letterSpacing);
                totalWidth += width + wordSpacing;
            }
            totalWidth -= wordSpacing;

            int x = destination.X - totalWidth;
            int y = destination.Y;

            Color glyphColor = color ?? Color.White;

            foreach(var word in words) {
                foreach(var character in word) {
                    x += DrawGlyph(character,x,y,scale,glyphColor) + letterSpacing;
                }
                x = x - letterSpacing + wordSpacing;
            }

            EmptyWordsQueue();
        }

        public void DrawCentered(StringBuilder stringBuilder,Point center,int scale,Color? color = null) {
            if(spriteBatch == null) {
                return;
            }
            if(scale < 1) {
                scale = 1;
            }

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
                    x += DrawGlyph(character,x,y,scale,glyphColor) + letterSpacing;
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
