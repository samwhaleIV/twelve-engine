using Elves.UI.Font;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using TwelveEngine;

namespace Elves.UI.Battle {
    public sealed class SpeechBox:UIElement, IBattleUIAnimated {

        private readonly AnimationInterpolator interpolator = new AnimationInterpolator(TimeSpan.FromMilliseconds(200));

        public SpeechBox() {
            Texture = UITextures.Panel;
        }

        bool IBattleUIAnimated.GetAnimationCompleted() {
            return interpolator.IsFinished;
        }

        public bool LeftSided { get; set; } = false;

        private bool _isShown = false;

        public bool IsShown => _isShown;

        public bool IsOffscreen {
            get {
                return !_isShown && interpolator.IsFinished;
            }
        }

        public void Show(TimeSpan now) {
            interpolator.Reset(now);
            _isShown = true;
        }

        public void Hide(TimeSpan now) {
            interpolator.Reset(now);
            _isShown = false;
        }

        public readonly StringBuilder StringBuilder = new StringBuilder();

        private float margin;

        public void Update(TimeSpan now,Rectangle viewport,float margin) {
            interpolator.Update(now);
            this.margin = margin;

            float height = viewport.Height * (2f/3f);
            float width = height / 3f * 4f;

            float x = viewport.Width * 0.5f - width * 0.5f;
            float y = viewport.Height * 0.5f - height * 0.5f;

            x += width * 0.25f * (LeftSided ? -1 : 1);

            var onscreenArea = new VectorRectangle(x,y,width,height);
            var offscreenArea = new VectorRectangle(viewport.X,onscreenArea.Y,width,height);
            Area = IsShown ? interpolator.Interpolate(offscreenArea,onscreenArea) : interpolator.Interpolate(offscreenArea,onscreenArea);
        }

        private static readonly Rectangle TextureSource = new Rectangle(32,16,64,48);

        public override void Draw(SpriteBatch spriteBatch,Color? color = null) {
            if(!IsOffscreen || Texture == null) {
                return;
            }
            spriteBatch.Draw(Texture,(Rectangle)Area,TextureSource,Color.White,0f,Vector2.Zero,LeftSided ? SpriteEffects.FlipHorizontally : SpriteEffects.None,1f);
        }

        public void DrawText(UVSpriteFont font) {
            if(!IsOffscreen) {
                return;
            }
            int textScale = (int)(Area.Height/(font.LineHeight*4));
            font.Draw(StringBuilder,(Area.Location+new Vector2(margin)).ToPoint(),textScale,Color.Black,(int)(Area.Width-margin));
        }
    }
}
