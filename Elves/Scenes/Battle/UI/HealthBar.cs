using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine;

namespace Elves.Scenes.Battle.UI {
    public enum HealthBarAlignment { Left, Right };

    public sealed class HealthBar:UIElement {

        public Rectangle TextureSource { get; private init; } = Constants.BattleUI.HealthBarSource;
        public HealthBarAlignment Alignment { get; private init; }
        public FloatRectangle UVArea { get; private init; }

        public HealthBar(HealthBarAlignment alignment) {
            Alignment = alignment;
            Texture = Program.Textures.Panel;
            UVArea = GetUVArea();
        }

        private FloatRectangle GetUVArea() {
            Rectangle textureSource = TextureSource;
            Vector2 textureSize = new(Texture.Width,Texture.Height);
            return new(textureSource.Location.ToVector2() / textureSize,textureSource.Size.ToVector2() / textureSize);
        }

        private readonly Interpolator impactAnimator = new(Constants.BattleUI.HealthImpact);

        private float _value;

        public float Value {
            get => _value;
            set {
                if(value < _value) {
                    AnimateHealthDrop();
                }
                _value = value;
            }
        }

        public bool IsDead => Value <= 0f;

        public float GetYOffset() {
            var t = 1 - impactAnimator.Value;
            return MathF.Sin(MathF.PI * t);
        }

        public void Update(TimeSpan now) => impactAnimator.Update(now);

        private void AnimateHealthDrop() {
            impactAnimator.Reset();
        }

        public (float Start, float End) GetOffColorRange() {
            if(Alignment == HealthBarAlignment.Left) {
                return (Value, 1);
            } else {
                return (0, 1-Value);
            }
        }

        public override void Draw(SpriteBatch spriteBatch,Color? color = null) {
            if(Texture == null) {
                return;
            }

            spriteBatch.Draw(Texture,ScreenArea.ToRectangle(),TextureSource,color ?? Color.White);
        }
    }
}
