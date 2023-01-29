using TwelveEngine.Font;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using TwelveEngine;

namespace Elves.Scenes.Battle.UI {
    public sealed class Button:UIElement {

        public Rectangle TextureSource { get; set; }
        public int ID { get; set; }

        protected override bool GetInputPaused() {
            /* Might need to ignore for turbo button pressing... */
            return !interpolator.IsFinished;
        }

        protected override void SetInputPaused(bool value) {
            throw new InvalidOperationException("Input pause property is readonly for button elements.");
        }

        private readonly AnimationInterpolator interpolator = new(Constants.AnimationTiming.ActionButtonMovement);

        private static readonly Rectangle DefaultTexture = new(0,16,32,16);
        private static readonly Rectangle SelectedTexture = new(0,32,32,16);
        private static readonly Rectangle PressedTexture = new(0,48,32,16);

        public Button() => Texture = Program.Textures.Panel;

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

        public bool IsOffscreen => !currentState.OnScreen && interpolator.IsFinished;

        public void Update(TimeSpan now,ButtonRenderData buttonRenderData) {
            interpolator.Update(now);

            VectorRectangle startPosition = buttonRenderData.GetPosition(oldState);
            VectorRectangle endPosition = buttonRenderData.GetPosition(currentState);

            ScreenArea = interpolator.Interpolate(startPosition,endPosition);
        }

        public void DrawText(UVSpriteFont spriteFont,int scale,Color color) {
            if(IsOffscreen) {
                return;
            }
            Vector2 center = ScreenArea.Center;
            if(Pressed) {
                center.Y += ScreenArea.Height / 16;
            }
            spriteFont.DrawCentered(Label,center.ToPoint(),scale,color);
        }

        private Rectangle GetTextureSource() {
            if(Pressed) {
                return PressedTexture;
            } else if(Selected) {
                return SelectedTexture;
            } else {
                return DefaultTexture;
            }
        }

        public override void Draw(SpriteBatch spriteBatch,Color? color = null) {
            if(IsOffscreen) {
                return;
            }
            spriteBatch.Draw(Texture,(Rectangle)ScreenArea,GetTextureSource(),color ?? Color.White);
        }
    }
}
