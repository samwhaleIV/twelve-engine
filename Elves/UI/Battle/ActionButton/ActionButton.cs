using TwelveEngine.Font;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using TwelveEngine;

namespace Elves.UI.Battle {
    public sealed class ActionButton:Button {

        public readonly AnimationInterpolator interpolator = new(Constants.AnimationTiming.ActionButtonMovement);

        public bool AnimationIsFinished => interpolator.IsFinished;

        private static readonly Rectangle PressedTextureSource = new(0,48,32,16);
        private static readonly Rectangle SelectedTextureSource = new(0,32,32,16);

        public ActionButton() {
            Texture = UITextures.Panel;
            TextureSource = new Rectangle(0,16,32,16);
        }

        public readonly StringBuilder Label = new();

        private ButtonState currentState = ButtonState.None, oldState = ButtonState.None;

        public ButtonState State {
            get => currentState;
            set {
                if(currentState == value) {
                    return;
                }
                currentState = value;
            }
        }

        public void SetState(TimeSpan now,ButtonState buttonState) {
            oldState = currentState;
            currentState = buttonState;
            interpolator.Reset(now);
        }

        public void Hide(TimeSpan now) {
            oldState = currentState;
            currentState = currentState.GetOffscreen();
            interpolator.Reset(now);
        }

        public void Show(TimeSpan now) {
            oldState = currentState;
            currentState = currentState.GetOnScreen();
            interpolator.Reset(now);
        }

        protected override bool GetIsEnabled() => currentState.OnScreen && interpolator.IsFinished;
        public bool IsOffscreen => !currentState.OnScreen && interpolator.IsFinished;

        public void Update(TimeSpan now,ButtonRenderData buttonRenderData) {
            interpolator.Update(now);

            VectorRectangle startPosition = buttonRenderData.GetPosition(oldState);
            VectorRectangle endPosition = buttonRenderData.GetPosition(currentState);

            Area = interpolator.Interpolate(startPosition,endPosition);
        }

        public void DrawText(UVSpriteFont spriteFont,int scale,Color color) {
            if(IsOffscreen) {
                return;
            }
            Vector2 center = Area.Center;
            if(Pressed) {
                center.Y += Area.Height / 16;
            }
            spriteFont.DrawCentered(Label,center.ToPoint(),scale,color);
        }

        private Rectangle GetTextureSource() {
            if(Pressed) {
                return PressedTextureSource;
            } else if(Selected) {
                return SelectedTextureSource;
            }
            return TextureSource;
        }

        public override void Draw(SpriteBatch spriteBatch,Color? color = null) {
            if(IsOffscreen) {
                return;
            }
            spriteBatch.Draw(Texture,(Rectangle)Area,GetTextureSource(),color ?? Color.White);
        }
    }
}
