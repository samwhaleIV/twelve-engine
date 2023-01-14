using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using TwelveEngine.Shell;
using TwelveEngine.Shell.UI;

namespace Elves.TestStates {
    public sealed class ScrollingBackgroundTest:GameState {
        public ScrollingBackgroundTest() {
            OnRender += ScrollingBackgroundTest_OnRender;
            OnLoad += ScrollingBackgroundTest_OnLoad;
            OnWriteDebug += ScrollingBackgroundTest_OnWriteDebug;
        }

        private void ScrollingBackgroundTest_OnWriteDebug(DebugWriter debugWriter) {
            debugWriter.ToTopLeft();
            debugWriter.Write(Scale,"Scale");
        }

        private Effect effect;

        private EffectParameter timeParameter, aspectRatioParameter, scaleParameter;

        private float T => (float)(Now / TimeSpan.FromSeconds(4) % 1.0);

        public Color Color { get; set; } = Color.White;

        public float Scale { get; set; } = 1f;

        private void UpdateEffect() {
            timeParameter.SetValue(T);
            aspectRatioParameter.SetValue(Game.Viewport.AspectRatio);
            scaleParameter.SetValue(Scale);
        }

        private void ScrollingBackgroundTest_OnLoad() {
            effect = Game.Content.Load<Effect>("Shaders/ScrollingBackgroundEffect");
            timeParameter = effect.Parameters["Time"];
            aspectRatioParameter = effect.Parameters["AspectRatio"];
            scaleParameter = effect.Parameters["Scale"];
        }

        private void ScrollingBackgroundTest_OnRender() {
            Scale = 1 + MathF.Sin(MathHelper.TwoPi * (float)(Now / TimeSpan.FromSeconds(20) % 1.0)) * 0.5f;
            SpriteBatch sb = Game.SpriteBatch;
            Texture2D texture = Textures.CheckerboardBackground;
            UpdateEffect();
            sb.Begin(SpriteSortMode.Immediate,null,SamplerState.LinearWrap,null,null,effect);
            sb.Draw(texture,Game.Viewport.Bounds,texture.Bounds,Color);
            sb.End();
        }
    }
}
