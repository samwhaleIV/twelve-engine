using Elves.UI.Font;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using TwelveEngine;

namespace Elves.UI.Battle {
    public sealed class Tagline:UIElement {

        private const int DEFAULT_BUFFER_SIZE = 128;

        private readonly AnimationInterpolator textPositionInterpolator = new AnimationInterpolator(TimeSpan.FromSeconds(0.5f));
        private readonly AnimationInterpolator elementDisplayInterpolator = new AnimationInterpolator(TimeSpan.FromSeconds(0.5f));

        private StringBuilder _oldText = new StringBuilder(DEFAULT_BUFFER_SIZE);
        private StringBuilder _currentText = new StringBuilder(DEFAULT_BUFFER_SIZE);

        public StringBuilder OldText => _oldText;
        public StringBuilder CurrentText => _currentText;

        public void SwapTexts() {
            StringBuilder sb = _oldText;
            _oldText = _currentText;
            _currentText = sb;
        }

        public void Show(TimeSpan now) {
            IsShown = true;
            elementDisplayInterpolator.Reset(now);
        }

        public void Hide(TimeSpan now) {
            IsShown = false;
            elementDisplayInterpolator.Reset(now);
        }

        public bool TextAnimationIsFinished => textPositionInterpolator.IsFinished;
        public bool AnimationIsFinished => elementDisplayInterpolator.IsFinished;

        public bool IsShown { get; private set; } = false;

        private Vector2 oldTextPosition, currentTextPosition;

        public void Update(TimeSpan now,Rectangle viewport,float margin) {
            textPositionInterpolator.Update(now);
            elementDisplayInterpolator.Update(now);

            float width = viewport.Width;
            float height = margin * 8;

            float centerX = viewport.X + viewport.Width * 0.5f;
            float centerY = viewport.Y + viewport.Height * 0.5f;

            float halfHeight = height * 0.5f;

            float x = 0;
            float y = elementDisplayInterpolator.Interpolate(viewport.Bottom,centerY-halfHeight);

            Area = new VectorRectangle(x,y,width,height);

            float textY = y + halfHeight;

            oldTextPosition = new Vector2(textPositionInterpolator.Interpolate(centerX,centerX-viewport.Width),textY);
            currentTextPosition = new Vector2(textPositionInterpolator.Interpolate(centerX+viewport.Width,centerX),textY);
        }

        public override void Draw(SpriteBatch spriteBatch,Color? color = null) {
            base.Draw(spriteBatch,Color.Black);
        }

        public void DrawText(UVSpriteFont font) {
            int textScale = (int)(Area.Height / font.LineHeight);
            if(!TextAnimationIsFinished) {
                font.DrawCentered(_oldText,oldTextPosition.ToPoint(),textScale,Color.White);
            }
            font.DrawCentered(_currentText,currentTextPosition.ToPoint(),textScale,Color.White);
        }
    }
}
