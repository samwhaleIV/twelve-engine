using Elves.Scenes.SaveSelect;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Font;
using TwelveEngine;

namespace Elves.Scenes.ModeSelectMenu {
    using static Constants.Terminal;

    public sealed class ModeSelectMenuScene:Scene3D {

        private readonly struct Line {
            public readonly int LeaseID;
            public readonly StringBuilder StringBuilder;

            public Line(int leaseID,StringBuilder stringBuilder) {
                LeaseID = leaseID;
                StringBuilder = stringBuilder;
            }
        }

        private Line GetLine() {
            int leaseID = _sbPool.Lease(out StringBuilder sb);
            return new Line(leaseID,sb);
        }

        private void ReturnLine(Line line) {
            _sbPool.Return(line.LeaseID);
        }

        private readonly StringBuilderPool _sbPool = new();

        public bool IsWritingMessage => _messageQueue.Count > 0;

        private readonly Queue<char> _messageQueue = new();
        private readonly HashSet<char> _slowCharacters = new();

        private TimeSpan LastLetterTime = TimeSpan.Zero;

        private TimeSpan lastLetterTimeReal;

        private void UpdateModeSelectMenu() {
            if(_messageQueue.Count <= 0 || LocalNow - LastLetterTime < TypeRate) {
                return;
            }

            var diff = lastLetterTimeReal - Now;
            lastLetterTimeReal = Now;
            Console.WriteLine(diff);

            var character = _messageQueue.Dequeue();
            LastLetterTime = _slowCharacters.Contains(character) ? LocalNow - TypeRate + TypeRateSlow : LocalNow;
            if(character == '\n') {
                _lines.Add(GetLine());
            } else {
                if(_lines.Count <= 0) {
                    _lines.Add(GetLine());
                }
                _lines[_lines.Count-1].StringBuilder.Append(character);
            }
        }

        private readonly List<Line> _lines = new();

        public ModeSelectMenuScene() {
            foreach(var character in SlowCharacters) {
                _slowCharacters.Add(character);
            }
            ClearColor = BackgroundColor;
            OnUpdate.Add(UpdateModeSelectMenu);
            OnRender.Add(RenderModeSelectMenu);
            Impulse.Router.OnDirectionDown += Router_OnDirectionDown;
        }

        private void RemoveLastLine() {
            if(_lines.Count <= 0) {
                return;
            }
            var lastIndex = _lines.Count - 1;
            var lastLine = _lines[lastIndex];
            ReturnLine(lastLine);
            _lines.RemoveAt(lastIndex);
        }

        public void PrintLine(string message) {
            if(_lines.Count >= 1) {
                _messageQueue.Enqueue('\n');
            }
            foreach(var character in message) {
                _messageQueue.Enqueue(character);
            }
        }

        private void Router_OnDirectionDown(Direction direction) {
            if(direction == Direction.Left) {
                PrintLine("Hello, world!");
            } else if(direction == Direction.Right) {
                RemoveLastLine();        
            }
        }

        private bool ShouldShowCaret {
            get {
                return !IsWritingMessage && (int)Math.Floor((LocalNow - LastLetterTime) / CaretBlinkRate) % 2 == 1;
            }
        }

        private void RenderModeSelectMenu() {

            float textScale = UIScale * TextScale;

            Vector2 screenCenter = Viewport.Bounds.Center.ToVector2();

            var font = Fonts.RetroFont;
            float rowHeight = font.LineHeight * textScale * 1.5f;

            float totalHeight = (_lines.Count - 1) * rowHeight;
            float y = screenCenter.Y - totalHeight / 2;

            font.Begin(SpriteBatch);
            for(int i = 0;i<_lines.Count;i++) {
                bool lastLine = i == _lines.Count - 1;
                StringBuilder text = _lines[i].StringBuilder;
                Vector2 position = new Vector2(screenCenter.X,y);
                if(lastLine) {
                    text.Append(ShouldShowCaret ? CaretSymbol : HiddenCaretSymbol);
                    font.DrawCentered(text,position,textScale,ForegroundColor);
                    text.Remove(text.Length-1,1);
                    continue;
                } else {
                    font.DrawCentered(text,position,textScale,ForegroundColor);
                }
                y += rowHeight;
            }
            font.End();
        }
    }
}
