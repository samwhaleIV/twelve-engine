using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Font;
using TwelveEngine;

namespace Elves.Scenes.ModeSelectMenu {
    using static Constants.Terminal;

    public class TerminalScene:Scene3D {

        public TerminalScene() {
            UIScaleModifier = Constants.UI.TerminalSceneScaleModifier;
            foreach(var character in SlowCharacters) {
                _slowCharacters.Add(character);
            }
            ClearColor = BackgroundColor;
            OnUpdate.Add(UpdateTerminal);
            OnRender.Add(RenderTerminal);
        }

        private TerminalLine GetLine() {
            int leaseID = _sbPool.Lease(out StringBuilder sb);
            return new TerminalLine(leaseID,sb);
        }

        private void ReturnLine(TerminalLine line) {
            _sbPool.Return(line.LeaseID);
        }

        private readonly StringBuilderPool _sbPool = new();

        public bool IsWritingText => textQueue.Count > 0;

        private readonly Queue<char> textQueue = new();
        private readonly HashSet<char> _slowCharacters = new();

        private TimeSpan LastLetterTime = TimeSpan.Zero;

        protected event Action OnTextQueueEmptied;

        protected void FlushTextQueue() {
            LastLetterTime = TimeSpan.Zero;
            while(textQueue.Count > 0) {
                AdvanceTextQueue();
            }
        }

        private void AdvanceTextQueue() {
            var character = textQueue.Dequeue();
            if(character == '\n') {
                Lines.Add(GetLine());
            } else {
                if(Lines.Count <= 0) {
                    Lines.Add(GetLine());
                }
                Lines[^1].StringBuilder.Append(character);
            }
            if(textQueue.Count != 0) {
                return;
            }
            OnTextQueueEmptied?.Invoke();
        }

        private void UpdateTerminal() {
            if(textQueue.Count <= 0 || LocalNow - LastLetterTime < TypeRate) {
                return;
            }
            LastLetterTime = _slowCharacters.Contains(textQueue.Peek()) ? LocalNow - TypeRate + TypeRateSlow : LocalNow;
            AdvanceTextQueue();
        }

        protected List<TerminalLine> Lines { get; private init; } = new();

        private int _lineCount = 0;

        public void RemoveLastLine() {
            if(Lines.Count <= 0) {
                return;
            }
            var lastIndex = Lines.Count - 1;
            var lastLine = Lines[lastIndex];
            ReturnLine(lastLine);
            Lines.RemoveAt(lastIndex);
            _lineCount--;
        }

        public void PrintLine(string message) {
            if(_lineCount >= 1) {
                textQueue.Enqueue('\n');
            }
            foreach(var character in message) {
                textQueue.Enqueue(character);
            }
            _lineCount++;
        }

        public void PrintLine() {
            if(_lineCount >= 1) {
                textQueue.Enqueue('\n');
            }
            _lineCount++;
        }

        private bool ShouldShowCaret {
            get {
                return !IsWritingText && (int)Math.Floor((LocalNow - LastLetterTime) / CaretBlinkRate) % 2 == 1;
            }
        }

        public FloatRectangle GetLineArea(int lineIndex) {
            return Lines[lineIndex].Area;
        }

        private void RenderTerminal() {

            float textScale = UIScale * TextScale;

            Vector2 screenCenter = Viewport.Bounds.Center.ToVector2();

            var font = Fonts.Retro;
            float rowHeight = font.LineHeight * textScale * 1.5f;

            float totalHeight = (Lines.Count - 1) * rowHeight;
            float y = screenCenter.Y - totalHeight / 2;

            font.Begin(SpriteBatch);
            for(int i = 0;i<Lines.Count;i++) {
                bool lastLine = i == Lines.Count - 1;
                var line = Lines[i];
                StringBuilder text = line.StringBuilder;
                Vector2 position = new(screenCenter.X,y);
                if(lastLine && ShouldShowCaret) {
                    position.X += (font.GlyphOrDefault(CaretSymbol).Source.Width + font.LetterSpacing) * MathF.Round(textScale) * 0.5f;
                    text.Append(CaretSymbol);
                    line.Area = font.DrawCentered(text,position,textScale,ForegroundColor);
                    text.Remove(text.Length-1,1);
                } else {
                    line.Area = font.DrawCentered(text,position,textScale,ForegroundColor);
                }
                Lines[i] = line;
                y += rowHeight;            
            }
            font.End();
        }
    }
}
