using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine.Effects {
    public sealed class Static {

        public Texture2D Texture { get; set; }

        private readonly Random random;

        public Static(Random random) => this.random = random;
        public Static(int randomSeed) => random = new(randomSeed);
        public Static() => random = new();

        private Point GetRandomTextureSourceLocation() {
            return Texture is not null ? new(random.Next(0,Texture.Width),random.Next(0,Texture.Height)) : Point.Zero;
        }

        public float Scale { get; set; } = 12;
        public float FramesPerSecond { get; set; } = 29.97f;

        public TimeSpan FrameDuration => TimeSpan.FromSeconds(1) / FramesPerSecond;

        private Point textureSourceLocation;
        private TimeSpan lastUpdatedTime = TimeSpan.FromHours(-1);

        /// <summary>
        /// Render the static effect from <c>Texture</c>.
        /// </summary>
        /// <param name="spriteBatch">Requires a wrapping sampler state to be active.</param>
        /// <param name="now">Current, total time.</param>
        /// <param name="bounds">Area to render the static effect onto.</param>
        public void Render(TimeSpan now,SpriteBatch spriteBatch,Rectangle bounds,Color? color = null) {
            if(now >= lastUpdatedTime + FrameDuration) {
                textureSourceLocation = GetRandomTextureSourceLocation();
                lastUpdatedTime = now;
            }
            if(Texture is null) {
                return;
            }
            float scale = bounds.Height * 0.01f;
            Rectangle source = new(textureSourceLocation,(bounds.Size.ToVector2() / scale).ToPoint());
            spriteBatch.Draw(Texture,bounds,source,color ?? Color.White);
        }
    }
}
