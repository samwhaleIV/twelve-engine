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
            debugWriter.Write(_t,"T");
        }

        private Effect effect;

        private EffectParameter timeParameter, aspectRatioParameter, scaleParameter, directionParameter;

        private float _t;

        public float ScrollTime { get; set; } = 10f;
        public Color Color { get; set; } = Color.White;
        public float Scale { get; set; } = 1f;
        public Vector2 Direction { get; set; } = new Vector2(1f,0f);

        public static float MaxOverMin(float x,float y) {

            x = MathF.Abs(x);
            y = MathF.Abs(y);

            if(x <= 0 || y <= 0) {
                return 1f;
            } else if(x > y) {
                return x / y;
            } else {
                return y / x;
            }

        }

        private void UpdateEffect() {
            timeParameter?.SetValue(_t);
            aspectRatioParameter?.SetValue(Game.Viewport.AspectRatio);
            scaleParameter?.SetValue(Scale);
            directionParameter?.SetValue(Direction);
        }

        private void ScrollingBackgroundTest_OnLoad() {
            effect = Game.Content.Load<Effect>("Shaders/ScrollingBackgroundEffect");
            aspectRatioParameter = effect.Parameters["AspectRatio"];
            timeParameter = effect.Parameters[nameof(Time)];
            scaleParameter = effect.Parameters[nameof(Scale)];
            directionParameter = effect.Parameters[nameof(Direction)];
        }

        private void UpdateTimeMask() {
            /* T has to be a factor of (max(x,y) / min(x,y)) or 1 */
            _t = (float)(Now / TimeSpan.FromSeconds(ScrollTime) % MaxOverMin(Direction.X,Direction.Y));
        }

        private void ScrollingBackgroundTest_OnRender() {
            Direction = new Vector2(1,0.5f);
            Scale = 1 + MathF.Sin(MathHelper.TwoPi * (float)(Now / TimeSpan.FromSeconds(40))) * 0.5f;
            UpdateTimeMask();
            SpriteBatch sb = Game.SpriteBatch;
            Texture2D texture = Textures.CheckerboardBackground;
            UpdateEffect();
            sb.Begin(SpriteSortMode.Immediate,null,SamplerState.LinearWrap,null,null,effect);
            sb.Draw(texture,Game.Viewport.Bounds,texture.Bounds,Color);
            sb.End();
        }
    }
}
