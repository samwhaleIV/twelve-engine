using TwelveEngine.Font;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using TwelveEngine;

namespace Elves.Scenes.Battle.UI {
    public sealed class SpeechBox:UIElement {

        private readonly Interpolator interpolator = new(Constants.BattleUI.SpeechBoxMovement);

        public Rectangle TextureSource { get; private init; } = Constants.BattleUI.SpeechBoxSource;

        public SpeechBox() {
            Texture = Program.Textures.Panel;
        }

        public bool LeftSided { get; set; } = false;
        public bool IsShown { get; private set; }

        public bool AnimationIsFinished => interpolator.IsFinished;
        public bool IsOffscreen => !IsShown && interpolator.IsFinished;

        public void Show(TimeSpan now) {
            if(IsShown) {
                return;
            }
            interpolator.ResetCarryOver(now);
            IsShown = true;
        }

        public void Hide(TimeSpan now) {
            if(!IsShown) {
                return;
            }
            interpolator.ResetCarryOver(now);
            IsShown = false;
        }

        public StringBuilder Text { get; private init; } = new();

        public void Update(TimeSpan now,FloatRectangle viewport,float scale) {
            interpolator.Update(now);

            Rectangle textureSource = TextureSource;

            float height = textureSource.Height * scale;
            float width = (float)textureSource.Width / textureSource.Height * height;

            Vector2 position = viewport.Center - new Vector2(width,height) * 0.5f;
            position.X += width * 0.25f * (LeftSided ? -1 : 1);

            FloatRectangle onscreenArea = new(position,width,height);
            FloatRectangle offscreenArea = LeftSided ? new(viewport.X-width,onscreenArea.Y,width,height) : new(viewport.Right,onscreenArea.Y,width,height);
            ScreenArea = IsShown ? interpolator.Interpolate(offscreenArea,onscreenArea) : interpolator.Interpolate(onscreenArea,offscreenArea);
        }

        public override void Draw(SpriteBatch spriteBatch,Color? color = null) {
            if(IsOffscreen || Texture == null) {
                return;
            }
            spriteBatch.Draw(Texture,(Rectangle)ScreenArea,TextureSource,Color.White,0f,Vector2.Zero,LeftSided ? SpriteEffects.FlipHorizontally : SpriteEffects.None,1f);
        }

        public void DrawText(UVSpriteFont font,float textScale,float margin) {
            if(IsOffscreen) {
                return;
            }
            Vector2 position = ScreenArea.Position + new Vector2(margin);
            position.X += ScreenArea.Width * (1/8f);
            font.Draw(Text,Vector2.Floor(position),textScale,Color.Black,(int)(ScreenArea.Right - position.X - margin));
        }
    }
}
