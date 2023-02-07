using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine;

namespace Elves.Scenes.Battle.UI {
    public enum HealthBarAlignment { Left, Right };

    public sealed class HealthBar:UIElement {

        public HealthBar() => Texture = Program.Textures.Panel;

        public Rectangle TextureSource { get; set; }

        public FloatRectangle UVArea {
            get {
                Vector2 textureSize = new(Texture.Width,Texture.Height);
                return new FloatRectangle(TextureSource.X / textureSize.X,TextureSource.Y / textureSize.Y,TextureSource.Size.ToVector2() / textureSize);
            }
        }

        private readonly Interpolator impactAnimator = new(Constants.BattleUI.HealthImpact);

        public HealthBarAlignment Alignment { get; set; } = HealthBarAlignment.Left;

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

        public (float Start,float End) GetOffColorRange() {
            if(Alignment == HealthBarAlignment.Left) {
                return (Value,1);
            } else {
                return (0,1-Value);
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
