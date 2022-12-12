using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Shell.States {
    public readonly struct PixelShaderTestData {
        public PixelShaderTestData(string shader,string texture) {
            Shader = shader;
            Texture = texture;
        }

        public readonly string Shader;
        public readonly string Texture;

        internal void Deconstruct(out string shader,out string texture) {
            shader = Shader;
            texture = Texture;
        }
    }

    public sealed class PixelShaderTest:DataGameState<PixelShaderTestData> {

        public PixelShaderTest() {
            OnLoad += ShaderTest_OnLoad;
            OnRender += ShaderTest_OnRender;
        }

        private Effect effect;
        private Texture2D texture2D;

        private void ShaderTest_OnLoad() {
            var (shader, texture) = Data;
            effect = Game.Content.Load<Effect>(shader);
            texture2D = Game.Content.Load<Texture2D>(texture);
        }

        private void ShaderTest_OnRender() {

            Game.GraphicsDevice.Clear(Color.Black);

            var spriteBatch = Game.SpriteBatch;

            var fillArea = GetFillArea(texture2D.Bounds.Size,Game.Viewport.Bounds.Size);

            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                effect: effect
            );

            spriteBatch.Draw(texture2D,fillArea,Color.White);

            spriteBatch.End();
        }

        private static Rectangle GetFillArea(Point sourceSize,Point targetSize) {
            var scale = Math.Min(
                targetSize.X / (float)sourceSize.X,
                targetSize.Y / (float)sourceSize.Y
            );

            var size = sourceSize.ToVector2() * scale;
            var location = targetSize.ToVector2() * 0.5f - size * 0.5f;

            return new Rectangle(location.ToPoint(),size.ToPoint());
        }

    }
}
