using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine;
using static Elves.Constants.BattleUI;

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

        private sealed class InertiaBody {

            private float _acceleration = 0f, _velocity = 0f, _counterAcceleration = 0f;
            private TimeSpan _accelerationDuration;

            private TimeSpan? _accelerationStart = null;

            public void Update(TimeSpan now,float delta) {
                bool reset;
                if(_acceleration == 0f) {
                    reset = true;
                } else {
                    if(!_accelerationStart.HasValue) {
                        _accelerationStart = now;
                    }
                    if(now - _accelerationStart < _accelerationDuration) {
                        _velocity += delta * _acceleration;
                        return;
                    }
                    _acceleration = 0f;
                    reset = true;
                }
                /* The reset clause is amended to the bottom so there isn't a 1-frame delay when we start counter acceleration. */
                if(!reset) {
                    return;
                }
                _accelerationStart = null;

                /* Restore zero velocity - can you make it bouncy? */
                float startVelocity = _velocity;
                _velocity += delta * MathF.Abs(_counterAcceleration) * -MathF.Sign(_velocity);
                if(MathF.Sign(startVelocity) == MathF.Sign(_velocity)) {
                    return;
                }
                _velocity = 0f;
            }

            public void SetAcceleration(TimeSpan duration,float acceleration,float? counterAcceleration = null) {
                _accelerationStart = null; /* Recalcualte acceleration start in update */
                _acceleration = acceleration;
                _accelerationDuration = duration;
                if(acceleration == 0f) {
                    return;
                }
                _counterAcceleration = counterAcceleration ?? _acceleration;
            }

            public float Velocity => _velocity;
        }

        private readonly InertiaBody impactAnimator = new();

        private float _value = -1;

        public float Value {
            get => _value;
            set {
                if(_value < 0) {
                    _value = value;
                    return;
                }
                if(value < _value) {
                    /* Injure */
                    impactAnimator.SetAcceleration(HurtImpactDuration,HurtImpactAccel,HurtImpactCounterAccel);
                } else if(value > _value) {
                    /* Heal */
                    impactAnimator.SetAcceleration(HealImpactDuration,HealImpactAccel,HealImpactCounterAccel);
                }
                _value = value;
            }
        }

        public bool IsDead => Value <= 0f;

        public float GetYOffset() {
            return impactAnimator.Velocity;
        }

        private void ApplyDeathWobble(TimeSpan now) {
            /* Shake the bar when the owner's health is zero (aka, dead) */
            int shakeStage = (int)(now / DeadWobbleDuration) % 4;
            /* Needs empty stages so we are centered on the right spot */
            (TimeSpan Duration, float Strength) impact = shakeStage switch {
                0 => (DeadWobbleDuration, -DeathWobbleStrength),
                2 => (DeadWobbleDuration, DeathWobbleStrength),
                _ => (TimeSpan.Zero, 0), /* 1 or 3 */
            };

            impactAnimator.SetAcceleration(impact.Duration,impact.Strength);
        }

        public void Update(TimeSpan now,float delta) {
            if(_value <= 0) {
                ApplyDeathWobble(now);
            }
            impactAnimator.Update(now,delta);
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
