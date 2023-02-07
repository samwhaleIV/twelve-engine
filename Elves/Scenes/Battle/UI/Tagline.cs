using TwelveEngine.Font;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using TwelveEngine;

namespace Elves.Scenes.Battle.UI {
    public sealed class Tagline:UIElement {

        private const int DEFAULT_BUFFER_SIZE = 128;

        private readonly Interpolator textPositionInterpolator = new(Constants.BattleUI.TagTextMovement);
        private readonly Interpolator elementDisplayInterpolator = new(Constants.BattleUI.TagMovement);

        private StringBuilder _oldText = new(DEFAULT_BUFFER_SIZE);
        private StringBuilder _currentText = new(DEFAULT_BUFFER_SIZE);

        public StringBuilder OldText => _oldText;
        public StringBuilder CurrentText => _currentText;

        private void SwapTexts() {
            (_currentText, _oldText) = (_oldText, _currentText);
        }

        public void Show(TimeSpan now) {
            if(IsShown) {
                return;
            }
            IsShown = true;
            elementDisplayInterpolator.ResetCarryOver(now);
        }

        public void Hide(TimeSpan now) {
            if(!IsShown) {
                return;
            }
            IsShown = false;
            elementDisplayInterpolator.ResetCarryOver(now);
        }

        public void CycleText(TimeSpan now) {
            SwapTexts();
            textPositionInterpolator.Reset(now);
        }

        public bool TextAnimationIsFinished => textPositionInterpolator.IsFinished;
        public bool AnimationIsFinished => elementDisplayInterpolator.IsFinished;

        public bool IsShown { get; private set; } = false;

        public bool IsOffscreen {
            get {
                return !IsShown && elementDisplayInterpolator.IsFinished;
            }
        }

        private Vector2 oldTextPosition, currentTextPosition;

        public void Update(TimeSpan now,FloatRectangle viewport,float scale) {
            //todo use scale
            textPositionInterpolator.Update(now);
            elementDisplayInterpolator.Update(now);

            float height = Fonts.RetroFont.LineHeight * MathF.Round(scale * Constants.BattleUI.TagTextScale) * Constants.BattleUI.TagBackgroundScale;

            Vector2 center = viewport.Center;

            float halfHeight = height * 0.5f;

            float y = IsShown ? elementDisplayInterpolator.Interpolate(viewport.Bottom,center.Y-halfHeight) : elementDisplayInterpolator.Interpolate(center.Y-halfHeight,viewport.Bottom);

            ScreenArea = new FloatRectangle(0,y,viewport.Width,height);

            float textY = y + halfHeight;

            oldTextPosition = new Vector2(textPositionInterpolator.Interpolate(center.X,center.X-viewport.Width),textY);
            currentTextPosition = new Vector2(textPositionInterpolator.Interpolate(center.X+viewport.Width,center.X),textY);
        }

        public override void Draw(SpriteBatch spriteBatch,Color? color = null) {
            if(IsOffscreen) {
                return;
            }
            base.Draw(spriteBatch,Color.Black);
        }

        public void DrawText(UVSpriteFont font,float scale) {
            if(IsOffscreen) {
                return;
            }
            if(!TextAnimationIsFinished) {
                font.DrawCentered(_oldText,Vector2.Floor(oldTextPosition),scale,Color.White);
            }
            font.DrawCentered(_currentText,Vector2.Floor(currentTextPosition),scale,Color.White);
        }
    }
}
