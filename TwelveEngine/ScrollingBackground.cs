using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TwelveEngine {
    public sealed class ScrollingBackground {

        /* This is where legends were born... */
        public static ScrollingBackground GetCheckered() {
            return new ScrollingBackground() {
                TileScale = 4f,
                Scale = 2f,
                Bulge = -0.75f,
                ScrollTime = TimeSpan.FromSeconds(30),
                ColorA = Color.FromNonPremultiplied(new Vector4(new Vector3(0.41f),1)),
                ColorB = Color.FromNonPremultiplied(new Vector4(new Vector3(0.66f),1))
            };
        }

        /* max(x,y) / min(x,y) with some extra precautions */
        public static float MaxOverMin(float x,float y) {
            x = MathF.Abs(x);
            y = MathF.Abs(y);
            if(x == 0 || y == 0) {
                return 1f;
            } else if(x > y) {
                return x / y;
            } else {
                return y / x;
            }
        }

        /* T/Time value; should always end in a 1. Allows for infinite scrolling when 1 rolls into 0, 2 into 0, etc. */
        private EffectParameter tParameter;
        /* T has to be a factor of (max(x,y) / min(x,y)) or 1 */
        private float GetT(TimeSpan now) {
            return (float)(now / ScrollTime % MaxOverMin(Direction.X,Direction.Y));
        }
        private float _t;
        private float T {
            get => _t;
            set {
                if(value == _t) {
                    return;
                }
                _t = value;
                tParameter?.SetValue(value);
            }
        }

        /* Aspect ratio of the viewport we are rendering to */
        private EffectParameter aspectRatioParameter;
        private float _aspectRatio;
        private float AspectRatio {
            get => _aspectRatio;
            set {
                if(value == _aspectRatio) {
                    return;
                }
                _aspectRatio = value;
                aspectRatioParameter?.SetValue(value);
            }
        }

        /* Scale of the texture source and the UV tiling effect */
        private EffectParameter scaleParamter;
        private float _scale = 1f;
        public float Scale {
            get => _scale;
            set {
                if(value == _scale) {
                    return;
                }
                _scale = value;
                scaleParamter?.SetValue(value);
            }
        }

        /* Distortion bulge, -1 -> 1 for sane values */
        private EffectParameter bulgeParameter;
        private float _bulge = 0f;
        public float Bulge {
            get => _bulge;
            set {
                if(value == _bulge) {
                    return;
                }
                _bulge = value;
                bulgeParameter?.SetValue(value);
            }
        }

        /* Scroll direction, T parameter is inflated to allow for seamless rollover. Adjust scroll time to compenstate */
        private EffectParameter directionParameter;
        private Vector2 _direction = new(1f,0f);
        public Vector2 Direction {
            get => _direction;
            set {
                if(value == _direction) {
                    return;
                }
                _direction = value;
                directionParameter?.SetValue(value);
            }
        }

        /* 0 -> 1 range, covers the UV area */
        private EffectParameter bulgeOriginParameter;
        private Vector2 _bulgeOrigin = new(0.5f,0.5f);
        public Vector2 BulgeOrigin {
            get => _bulgeOrigin;
            set {
                if(value == _bulgeOrigin) {
                    return;
                }
                _bulgeOrigin = value;
                bulgeOriginParameter?.SetValue(value);
            }
        }

        /* Checker tile color 1 */
        private EffectParameter colorAParameter;
        private Color _colorA = Color.White;
        public Color ColorA {
            get => _colorA;
            set {
                if(value == _colorA) {
                    return;
                }
                _colorA = value;
                colorAParameter?.SetValue(value.ToVector4());
            }
        }

        /* Checker tile color 2 */
        private EffectParameter colorBParameter;
        private Color _colorB = Color.White;
        public Color ColorB {
            get => _colorB;
            set {
                if(value == _colorB) {
                    return;
                }
                _colorB = value;
                colorBParameter?.SetValue(value.ToVector4());
            }
        }

        /* The scale of the tile effect. Needs to be a multiple of 1 to get square tiles, but non-square tiles still repeat */
        private EffectParameter tileScaleParameter;
        private float _tileScale = 1f;
        public float TileScale {
            get => _tileScale;
            set {
                if(value == _tileScale) {
                    return;
                }
                _tileScale = value;
                tileScaleParameter?.SetValue(value);
            }
        }

        public Texture2D Texture { get; set; } = null;
        public TimeSpan ScrollTime { get; set; } = TimeSpan.FromSeconds(1);

        /* Global color, multiplies both tile color and texture color */
        public Color Color { get; set; } = Color.White;

        private Effect effect;

        public void Load(ContentManager content) {
            effect = content.Load<Effect>("Shaders/ScrollingBackgroundEffect");
            aspectRatioParameter = effect.Parameters[nameof(AspectRatio)];
            scaleParamter = effect.Parameters[nameof(Scale)];
            bulgeParameter = effect.Parameters[nameof(Bulge)];
            directionParameter = effect.Parameters[nameof(Direction)];
            bulgeOriginParameter = effect.Parameters[nameof(BulgeOrigin)];
            tParameter = effect.Parameters[nameof(T)];
            colorAParameter = effect.Parameters[nameof(ColorA)];
            colorBParameter = effect.Parameters[nameof(ColorB)];
            tileScaleParameter = effect.Parameters[nameof(TileScale)];
            UpdateEffectParameters();
        }

        public void Unload() {
            effect?.Dispose();
            effect = null;
        }

        private void UpdateEffectParameters() {
            aspectRatioParameter?.SetValue(AspectRatio);
            scaleParamter?.SetValue(Scale);
            tParameter?.SetValue(T);
            directionParameter?.SetValue(Direction);

            bulgeParameter?.SetValue(Bulge);
            bulgeOriginParameter?.SetValue(BulgeOrigin);

            colorAParameter?.SetValue(ColorA.ToVector4());
            colorBParameter?.SetValue(ColorB.ToVector4());
            tileScaleParameter?.SetValue(TileScale);
        }

        public void Render(SpriteBatch spriteBatch,TimeSpan now,Viewport viewport) {
            T = GetT(now);
            AspectRatio = viewport.AspectRatio;
            spriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointWrap,null,null,effect);
            spriteBatch.Draw(Texture,viewport.Bounds,Texture.Bounds,Color);
            spriteBatch.End();
        }
    }
}
