using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using TwelveEngine.Shell;
using TwelveEngine.Shell.UI;
using static System.Formats.Asn1.AsnWriter;

namespace Elves.TestStates {
    public sealed class BulgeEffectTest:GameState {

        public BulgeEffectTest() {
            OnRender += BulgeEffectTest_OnRender;
            OnLoad += BulgeEffectTest_OnLoad;
            OnWriteDebug += BulgeEffectTest_OnWriteDebug;
            OnUnload += BulgeEffectTest_OnUnload;
        }

        private void BulgeEffectTest_OnUnload() {
            effect?.Dispose();
            effect = null;
        }

        private void BulgeEffectTest_OnWriteDebug(DebugWriter debugWriter) {
            debugWriter.ToTopLeft();
            debugWriter.Write(Amount,nameof(Amount));
        }

        private Effect effect;

        private EffectParameter originParameter, amountParameter;

        public Vector2 Origin { get; set; } = Vector2.Zero;
        public float Amount { get; set; } = 1f;
        public Color Color { get; set; } = Color.White;

        private void UpdateEffect() {
            originParameter?.SetValue(Origin);
            amountParameter?.SetValue(Amount);
        }

        private void BulgeEffectTest_OnLoad() {
            effect = Game.Content.Load<Effect>("Shaders/BulgeEffect");
            originParameter = effect.Parameters[nameof(Origin)];
            amountParameter = effect.Parameters[nameof(Amount)];
        }

        private void BulgeEffectTest_OnRender() {
            SpriteBatch sb = Game.SpriteBatch;
            Texture2D texture = Textures.CheckerboardSmall;
            Origin = new Vector2(0.5f,0.5f);
            Amount = MathF.Sin(MathHelper.TwoPi * (float)(Now / TimeSpan.FromSeconds(4) % 1));
            UpdateEffect();
            sb.Begin(SpriteSortMode.Immediate,null,SamplerState.PointWrap,null,null,effect);
            sb.Draw(texture,Game.Viewport.Bounds,texture.Bounds,Color);
            sb.End();
        }
    }
}
