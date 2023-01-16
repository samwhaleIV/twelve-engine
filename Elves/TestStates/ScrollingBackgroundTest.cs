using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine.Shell;
using TwelveEngine.Shell.UI;

namespace Elves.TestStates {
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
        private float T {
            get {
                /* T has to be a factor of (max(x,y) / min(x,y)) or 1 */
                return (float)(Now / TimeSpan.FromSeconds(ScrollTime) % MaxOverMin(Direction.X,Direction.Y));
            }
        }

        private EffectParameter aspectRatioParameter;
        private float AspectRatio {
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
        }


        private void UpdateEffectParameters() {
            aspectRatioParameter.SetValue(AspectRatio);
            scaleParamter.SetValue(Scale);
            bulgeParameter.SetValue(Bulge);
            directionParameter.SetValue(Direction);
            bulgeOriginParameter.SetValue(BulgeOrigin);
            tParameter.SetValue(T);
        }

        private void ScrollingBackgroundTest_OnRender() {
            SpriteBatch sb = Game.SpriteBatch;
            Texture2D texture = Textures.CheckerboardSmall;
            UpdateEffectParameters();
            sb.Begin(SpriteSortMode.Immediate,null,SamplerState.PointWrap,null,null,effect);
            sb.Draw(texture,Game.Viewport.Bounds,texture.Bounds,Color);
            sb.End();
        }
    }
}
