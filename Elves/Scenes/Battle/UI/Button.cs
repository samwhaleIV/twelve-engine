﻿using TwelveEngine.Font;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;
using TwelveEngine;
using TwelveEngine.UI;

namespace Elves.Scenes.Battle.UI {
    public sealed class Button:UIElement,IEndpoint<int> {

        public int GetEndPointValue() => ID;
        public void FireActivationEvent(int value) => OnActivated?.Invoke(value);
        public event Action<int> OnActivated;

        public Button() {
            Texture = Program.Textures.Panel;
            Endpoint = new Endpoint<int>(this);
        }

        public Rectangle TextureSource { get; private set; } = DefaultTextureSource;
        public int ID { get; set; }

        protected override bool GetInputPaused() {
            /* Might need to ignore for turbo button pressing... */
            return !interpolator.IsFinished || !currentState.OnScreen;
        }

        protected override void SetInputPaused(bool value) {
            throw new InvalidOperationException("Input pause property is readonly for button elements.");
        }

        private readonly Interpolator interpolator = new(Constants.BattleUI.ButtonMovement);

        private static readonly Rectangle DefaultTextureSource = new(0,16,32,16);
        private static readonly Rectangle SelectedTextureSource = new(0,32,32,16);
        private static readonly Rectangle PressedTextureSource = new(0,48,32,16);

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

        public void FinishAnimation(TimeSpan now) {
            interpolator.Update(now);
            interpolator.Finish();
        }

        public void Show(TimeSpan now) {
            oldState = currentState;
            currentState = currentState.GetOnScreen();
            interpolator.Reset(now);
        }

        public bool IsOffscreen => !currentState.OnScreen && interpolator.IsFinished;

        public void Update(TimeSpan now,ButtonRenderData buttonRenderData) {
            interpolator.Update(now);

            FloatRectangle startPosition = buttonRenderData.GetPosition(oldState);
            FloatRectangle endPosition = buttonRenderData.GetPosition(currentState);

            ScreenArea = interpolator.Interpolate(startPosition,endPosition);
        }

        public void DrawText(UVSpriteFont spriteFont,float scale,Color color) {
            if(IsOffscreen) {
                return;
            }
            Vector2 center = ScreenArea.Center;
            if(Pressed) {
                center.Y += ScreenArea.Height / 16;
            }
            spriteFont.DrawCentered(Label,Vector2.Floor(center),scale,color);
        }

        private Rectangle GetTextureSource() {
            if(Pressed) {
                return PressedTextureSource;
            } else if(Selected) {
                return SelectedTextureSource;
            } else {
                return DefaultTextureSource;
            }
        }

        public override void Draw(SpriteBatch spriteBatch,Color? color = null) {
            if(IsOffscreen) {
                return;
            }
            TextureSource = GetTextureSource();
            spriteBatch.Draw(Texture,(Rectangle)ScreenArea,TextureSource,color ?? Color.White);
        }
    }
}
