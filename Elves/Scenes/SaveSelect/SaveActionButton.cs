using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TwelveEngine;

namespace Elves.Scenes.SaveSelect {
    public sealed class SaveActionButton {

        private readonly AnimationInterpolator hoverAnimator = new(Constants.AnimationTiming.SaveActionButtonHover);
        private readonly AnimationInterpolator pressAnimator = new(Constants.AnimationTiming.SaveActionButtonPress);

        private static readonly Dictionary<SaveButtonType,Rectangle> buttonSources = new() {
            { SaveButtonType.Back, new(0,123,16,16) },
            { SaveButtonType.Play, new(17,123,16,16) },
            { SaveButtonType.Yes, new(0,140,16,16) },
            { SaveButtonType.Delete, new(17,140,16,16) },
        };

        public SaveButtonType Type { get; set; }

        private static Rectangle GetButtonSource(SaveButtonType type) {
            if(!buttonSources.TryGetValue(type,out Rectangle source)) {
                return Rectangle.Empty;
            }
            return source;
        }

        public Rectangle Destination { get; private set; } = Rectangle.Empty;
        public Rectangle HitTestArea { get; private set; } = Rectangle.Empty;

        public int Index { get; set; } = 0;

        private bool hovered = false, pressed = false;

        private float GetHoverT() {
            if(hovered) {
                return hoverAnimator.Value;
            } else {
                return 1 - hoverAnimator.Value;
            }
        }

        private float GetPressedT() {
            if(pressed) {
                return pressAnimator.Value;
            } else {
                return 1 - pressAnimator.Value;
            }
        }

        public void SetHover(TimeSpan now) {
            hoverAnimator.ResetCarryOver(now);
            hovered = true;
        }

        public void SetUnHover(TimeSpan now) {
            hoverAnimator.ResetCarryOver(now);
            hovered = false;
        }

        public void SetPress(TimeSpan now) {
            pressAnimator.ResetCarryOver(now);
            pressed = true;
        }

        public void SetUnPress(TimeSpan now) {
            pressAnimator.ResetCarryOver(now);
            pressed = false;
        }

        public float Rotation { get; set; } = 0f;
        public Texture2D Texture { get; set; }

        public bool IsEvenNumbered { get; set; } = false;

        public void Update(TimeSpan now,Vector2 screenOrigin,float height) {
            hoverAnimator.Update(now);
            pressAnimator.Update(now);
            if(Scale <= 0) {
                return;
            }
            Rotation = MathHelper.ToRadians(GetHoverT() * 10 * (IsEvenNumbered ? -1 : 1));
            Vector2 size = new Vector2(height) * Scale * (1 + GetHoverT() * 0.05f) * (1 + GetPressedT() * -0.08f);
            Destination = new(screenOrigin.ToPoint(),size.ToPoint());

            HitTestArea = new((screenOrigin-size*0.5f).ToPoint(),size.ToPoint());
        }

        public float Scale { get; set; } = 1f;

        public void Render(SpriteBatch spriteBatch) {
            if(Scale <= 0) {
                return;
            }
            Rectangle source = GetButtonSource(Type);
            spriteBatch.Draw(Texture,Destination,source,Color.White,Rotation,source.Size.ToVector2()*0.5f,SpriteEffects.None,1f);
        }
    }
}
