using Elves.UI.Font;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using TwelveEngine;

namespace Elves.UI.Battle {
    public sealed class SpeechBox:UIElement {

        private readonly AnimationInterpolator interpolator = new(Constants.AnimationTiming.SpeechBoxMovement);

        public SpeechBox() {
            Texture = UITextures.Panel;
        }

        public bool AnimationIsFinished => interpolator.IsFinished;

        public bool LeftSided { get; set; } = false;

        private bool _isShown = false;

        public bool IsShown => _isShown;

        public bool IsOffscreen {
            get {
                return !_isShown && interpolator.IsFinished;
            }
        }

        public void Show(TimeSpan now) {
            if(_isShown) {
                return;
            }
            interpolator.ResetCarryOver(now);
            _isShown = true;
        }

        public void Hide(TimeSpan now) {
            if(!_isShown) {
                return;
            }
            interpolator.ResetCarryOver(now);
            _isShown = false;
        }

        public readonly StringBuilder Text = new();

        public void Update(TimeSpan now,Rectangle viewport) {
            interpolator.Update(now);

            float height = viewport.Height * (2f/3f);
            float width = height / 3f * 4f;

            float x = viewport.Width * 0.5f - width * 0.5f;
            float y = viewport.Height * 0.5f - height * 0.5f;

            x += width * 0.25f * (LeftSided ? -1 : 1);

            var onscreenArea = new VectorRectangle(x,y,width,height);
            var offscreenArea = LeftSided ? new VectorRectangle(viewport.X-width,onscreenArea.Y,width,height) : new VectorRectangle(viewport.Right,onscreenArea.Y,width,height);
            Area = IsShown ? interpolator.Interpolate(offscreenArea,onscreenArea) : interpolator.Interpolate(onscreenArea,offscreenArea);
        }

        private static readonly Rectangle TextureSource = new(32,16,64,48);

        public override void Draw(SpriteBatch spriteBatch,Color? color = null) {
            if(IsOffscreen || Texture == null) {
                return;
            }
            spriteBatch.Draw(Texture,(Rectangle)Area,TextureSource,Color.White,0f,Vector2.Zero,LeftSided ? SpriteEffects.FlipHorizontally : SpriteEffects.None,1f);
        }

        public void DrawText(UVSpriteFont font) {
            if(IsOffscreen) {
                return;
            }
            int textScale = (int)(Area.Width / font.LineHeight / 16);

            float margin = Area.Width * 0.075F;
            Vector2 location = Area.Location + new Vector2(margin);
            location.X += Area.Width * (1/8F);
            font.Draw(Text,location.ToPoint(),textScale,Color.Black,(int)(Area.Right - location.X - margin));
        }
    }
}
