using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Effects {
    public class InfinityBackground {

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

        private EffectParameter positionParameter;
        private Vector2 _position = new(0f,0f);
        public Vector2 Position {
            get => _position;
            set {
                if(value == _position) {
                    return;
                }
                _position = value;
                positionParameter?.SetValue(value);
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

        private EffectParameter rotationParameter;
        private float _rotation = 0f;
        public float Rotation {
            get => _rotation;
            set {
                if(value == _rotation) {
                    return;
                }
                _rotation = value;
                rotationParameter?.SetValue(MathHelper.ToRadians(value));
            }
        }

        public Texture2D Texture { get; set; } = null;

        /* Global color, multiplies both tile color and texture color */
        public Color Color { get; set; } = Color.White;

        private Effect effect;

        public void Load(ContentManager content) {
            effect = content.Load<Effect>("Shaders/ScrollingBackgroundEffect");
            aspectRatioParameter = effect.Parameters[nameof(AspectRatio)];
            scaleParamter = effect.Parameters[nameof(Scale)];
            bulgeParameter = effect.Parameters[nameof(Bulge)];
            positionParameter = effect.Parameters[nameof(Position)];
            bulgeOriginParameter = effect.Parameters[nameof(BulgeOrigin)];
            colorAParameter = effect.Parameters[nameof(ColorA)];
            colorBParameter = effect.Parameters[nameof(ColorB)];
            tileScaleParameter = effect.Parameters[nameof(TileScale)];
            rotationParameter = effect.Parameters[nameof(Rotation)];
            UpdateEffectParameters();
        }

        private void UpdateEffectParameters() {
            aspectRatioParameter?.SetValue(AspectRatio);
            scaleParamter?.SetValue(Scale);
            positionParameter?.SetValue(Position);
            bulgeParameter?.SetValue(Bulge);
            bulgeOriginParameter?.SetValue(BulgeOrigin);
            colorAParameter?.SetValue(ColorA.ToVector4());
            colorBParameter?.SetValue(ColorB.ToVector4());
            tileScaleParameter?.SetValue(TileScale);
            rotationParameter?.SetValue(MathHelper.ToRadians(Rotation));
        }

        public void Render(SpriteBatch spriteBatch,Viewport viewport) {
            AspectRatio = viewport.AspectRatio;
            spriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointWrap,null,null,effect);
            spriteBatch.Draw(Texture,viewport.Bounds,Texture.Bounds,Color);
            spriteBatch.End();
        }
    }
}
