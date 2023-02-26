using System.Text;

namespace TwelveEngine.Font {
    public sealed class UVSpriteFont {

        private const int CHARACTER_QUEUE_SIZE = 16;
        private const int CHARACTER_QUEUE_COUNT = 64;

        private readonly float lineHeight, letterSpacing, wordSpacing;
        private readonly Texture2D texture;

        private readonly Dictionary<char,Glyph> glyphs;

        private readonly int characterQueueStartSize;

        public float LineHeight => lineHeight;
        public float LetterSpacing => letterSpacing;
        public float WordSpacing => wordSpacing;

        private const char SPACE_CHARACTER = ' ';

        public float LineSpace { get; set; } = Constants.DefaultLineSpacing;

        public Color DefaultColor { get; set; } = Color.White;

        private readonly Queue<Queue<char>> characterQueuePool = new();
        private readonly Queue<Queue<char>> words = new();
        private readonly Queue<char> wordBuffer = new();

        private SpriteBatch spriteBatch = null;

        public UVSpriteFont(
            Texture2D texture,
            float lineHeight,
            float letterSpacing,
            float wordSpacing,
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

            for(int i = 0;i<characterQueueCount;i++) {
                characterQueuePool.Enqueue(new Queue<char>(characterQueueSize));
            }

            characterQueueStartSize = characterQueueSize;
        }

        public Glyph GlyphOrDefault(char character) {
            if(!glyphs.TryGetValue(character,out Glyph value)) {
                value = new Glyph();
            }
            return value;
        }

        private void AssertSpriteBatch() {
            if(spriteBatch == null) {
                throw new InvalidOperationException("Cannot draw text before calling begin method!");
            }
        }

        public void Begin(SpriteBatch spriteBatch) {
            if(this.spriteBatch != null) {
                throw new InvalidOperationException("Cannot begin sprite font after having already called begin!");
            }
            spriteBatch.Begin(SpriteSortMode.Deferred,BlendState.NonPremultiplied,SamplerState.PointClamp);
            this.spriteBatch = spriteBatch;
        }

        public void End() {
            if(spriteBatch == null) {
                throw new InvalidOperationException("Cannot end sprite font before calling begin method!");
            }
            spriteBatch.End();
            spriteBatch = null;
        }

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

        private float MeasureWordWidth(Queue<char> word,float scale,float letterSpacing) {
            float width = 0;
            foreach(var character in word) {
                Glyph glyph = GlyphOrDefault(character);
                width += glyph.Source.Width * scale + letterSpacing;
            }
            width -= letterSpacing;
            return width;
        }

        private void TryFlushWordBuffer() {
            if(wordBuffer.Count <= 0) {
                return;
            }
            var characterQueue = GetPooledCharacterQueue();
            foreach(char character in wordBuffer) {
                characterQueue.Enqueue(character);
            }
            words.Enqueue(characterQueue);
            wordBuffer.Clear();
        }

        private void FillWordsQueue(StringBuilder stringBuilder) {
            for(int i = 0;i<stringBuilder.Length;i++) {
                char character = stringBuilder[i];
                if(character == SPACE_CHARACTER) {
                    TryFlushWordBuffer();
                    continue;
                }
                wordBuffer.Enqueue(character);
            }
            TryFlushWordBuffer();
        }

        private void FillWordsQueue(string text) {
            for(int i = 0;i<text.Length;i++) {
                char character = text[i];
                if(character == SPACE_CHARACTER) {
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

        private float DrawGlyph(char character,float x,float y,float scale,Color color) {
            Glyph glyph = GlyphOrDefault(character);
            if(glyph.Source.Width <= 0) {
                return 0;
            }
            Rectangle glyphArea = glyph.Source;
            Vector2 position = new(x,y+glyph.YOffset*scale);
            spriteBatch.Draw(texture,position,glyphArea,color,0f,Vector2.Zero,scale,SpriteEffects.None,1f);
            return glyphArea.Width * scale;
        }

        private void DrawLineBreaking(Vector2 destination,float scale,Color color,float maxWidth) {
            float x = destination.X, y = destination.Y;

            float lineHeight = LineSpace * this.lineHeight * scale;

            float wordSpacing = this.wordSpacing * scale;
            float letterSpacing = this.letterSpacing * scale;

            float maxX = destination.X + maxWidth;

            if(maxWidth <= 0) {
                foreach(var word in words) {
                    foreach(var character in word) {
                        x += DrawGlyph(character,x,y,scale,color) + letterSpacing;
                    }
                    x = x - letterSpacing + wordSpacing;
                }
            } else {
                foreach(var word in words) {
                    if(x + MeasureWordWidth(word,scale,letterSpacing) > maxX) {
                        x = destination.X;
                        y += lineHeight;
                    }
                    foreach(var character in word) {
                        x += DrawGlyph(character,x,y,scale,color) + letterSpacing;
                    }
                    x = x - letterSpacing + wordSpacing;
                }
            }
        }

        private void DrawRight(Vector2 destination,float scale,Color color) {
            float wordSpacing = this.wordSpacing * scale;
            float letterSpacing = this.letterSpacing * scale;

            float totalWidth = 0;
            foreach(var word in words) {
                float width = MeasureWordWidth(word,scale,letterSpacing);
                totalWidth += width + wordSpacing;
            }
            totalWidth -= wordSpacing;

            float x = destination.X - totalWidth;
            float y = destination.Y;

            foreach(var word in words) {
                foreach(var character in word) {
                    x += DrawGlyph(character,x,y,scale,color) + letterSpacing;
                }
                x = x - letterSpacing + wordSpacing;
            }
        }

        private FloatRectangle DrawCentered(Vector2 center,float scale,Color color) {
            float wordSpacing = this.wordSpacing * scale;
            float letterSpacing = this.letterSpacing * scale;

            float totalWidth = 0;
            foreach(var word in words) {
                float width = MeasureWordWidth(word,scale,letterSpacing);
                totalWidth += width + wordSpacing;
            }
            totalWidth -= wordSpacing;
            if(totalWidth % 2 == 1) {
                totalWidth += 1;
            }

            float characterHeight = lineHeight * scale;

            float x = center.X - totalWidth * 0.5f;
            float y = center.Y - characterHeight * 0.5f;

            FloatRectangle area = new(x,y,totalWidth,characterHeight);

            foreach(var word in words) {
                foreach(var character in word) {
                    x += DrawGlyph(character,x,y,scale,color) + letterSpacing;
                }
                x = x - letterSpacing + wordSpacing;
            }

            return area;
        }

        private static float ValidateScale(ref float scale) => scale = MathF.Round(scale);

        public void Draw(StringBuilder stringBuilder,Vector2 destination,float scale,Color? color = null,float maxWidth = 0) {
            ValidateScale(ref scale);
            AssertSpriteBatch();
            FillWordsQueue(stringBuilder);
            DrawLineBreaking(destination,scale,color ?? DefaultColor,maxWidth);
            EmptyWordsQueue();
        }

        public void Draw(string text,Vector2 destination,float scale,Color? color = null,float maxWidth = 0) {
            ValidateScale(ref scale);
            AssertSpriteBatch();
            FillWordsQueue(text);
            DrawLineBreaking(destination,scale,color ?? DefaultColor,maxWidth);
            EmptyWordsQueue();
        }

        public void DrawRight(StringBuilder stringBuilder,Vector2 destination,float scale,Color? color = null) {
            ValidateScale(ref scale);
            AssertSpriteBatch();
            FillWordsQueue(stringBuilder);
            DrawRight(destination,scale,color ?? DefaultColor);
            EmptyWordsQueue();
        }

        public void DrawRight(string text,Vector2 destination,float scale,Color? color = null) {
            ValidateScale(ref scale);
            AssertSpriteBatch();
            FillWordsQueue(text);
            DrawRight(destination,scale,color ?? DefaultColor);
            EmptyWordsQueue();
        }

        public FloatRectangle DrawCentered(StringBuilder stringBuilder,Vector2 center,float scale,Color? color = null) {
            ValidateScale(ref scale);
            AssertSpriteBatch();
            FillWordsQueue(stringBuilder);
            var area = DrawCentered(center,scale,color ?? DefaultColor);
            EmptyWordsQueue();
            return area;
        }

        public FloatRectangle DrawCentered(string text,Vector2 center,float scale,Color? color = null) {
            ValidateScale(ref scale);
            AssertSpriteBatch();
            FillWordsQueue(text);
            var area = DrawCentered(center,scale,color ?? DefaultColor);
            EmptyWordsQueue();
            return area;
        }
    }
}
