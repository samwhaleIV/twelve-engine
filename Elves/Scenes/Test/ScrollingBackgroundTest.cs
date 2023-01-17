using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine.Shell;
using TwelveEngine.Shell.UI;

namespace Elves.Scenes.Test {
    public sealed class ScrollingBackgroundTest:GameState {

        public ScrollingBackgroundTest() {
            OnRender += ScrollingBackgroundTest_OnRender;
            OnLoad += ScrollingBackgroundTest_OnLoad;
            OnWriteDebug += ScrollingBackgroundTest_OnWriteDebug;
            OnUnload += ScrollingBackgroundTest_OnUnload;
        }

        private void ScrollingBackgroundTest_OnUnload() {
            effect?.Dispose();
            effect = null;
        }

        private void ScrollingBackgroundTest_OnWriteDebug(DebugWriter debugWriter) {
            debugWriter.ToTopLeft();
            debugWriter.Write(Scale,nameof(Scale));
            debugWriter.Write(Direction,nameof(Direction));
            debugWriter.Write(T,nameof(T));
        }

        private Effect effect;

        private EffectParameter tParameter;

        public float T {
            get {
                /* T has to be a factor of (max(x,y) / min(x,y)) or 1 */
                return (float)(Now / TimeSpan.FromSeconds(ScrollTime) % MaxOverMin(Direction.X,Direction.Y));
            }
        }

        private EffectParameter aspectRatioParameter;
        public float AspectRatio {
            get {
                return Game.Viewport.AspectRatio;
            }
        }

        public float ScrollTime { get; set; } = 10f;
        public Color Color { get; set; } = Color.White;

        private EffectParameter scaleParamter;
        public float Scale { get; set; } = 1f;

        private EffectParameter bulgeParameter;
        public float Bulge { get; set; } = -1f;

        private EffectParameter directionParameter;
        public Vector2 Direction { get; set; } = new Vector2(1f,0f);

        private EffectParameter bulgeOriginParameter;
        public Vector2 BulgeOrigin { get; set; } = new Vector2(0.5f,0.5f);

        private EffectParameter colorAParameter;
        public Color ColorA { get; set; } = Color.FromNonPremultiplied(new Vector4(new Vector3(0.41f),1));

        private EffectParameter colorBParameter;
        public Color ColorB { get; set; } = Color.FromNonPremultiplied(new Vector4(new Vector3(0.66f),1));

        private EffectParameter tileScaleParameter;
        public float TileScale { get; set; } = 4f;

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

        private void ScrollingBackgroundTest_OnLoad() {
            effect = Game.Content.Load<Effect>("Shaders/ScrollingBackgroundEffect");
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

        private float GetLoopTime(float seconds) {
            return (float)(Now / TimeSpan.FromSeconds(seconds) % 1);
        }

        private float GetSinTime(float seconds,float strength) {
            return MathF.Sin(GetLoopTime(seconds) * MathHelper.TwoPi) * strength;
        }

        private void ScrollingBackgroundTest_OnRender() {
            SpriteBatch sb = Game.SpriteBatch;
            Texture2D texture = Textures.Nothing;
            UpdateEffectParameters();

            Scale = 1 + GetSinTime(5,0.25f) * 0.5f;
            Bulge = GetSinTime(40,1);
            Direction = Vector2.One;

            sb.Begin(SpriteSortMode.Immediate,null,SamplerState.PointWrap,null,null,effect);
            sb.Draw(texture,Game.Viewport.Bounds,texture.Bounds,Color.Red);
            sb.End();
        }
    }
}
