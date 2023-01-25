using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Elves.FX {
    public sealed class WillTheDVDHitTheCornerSimulator {

        private readonly Random random;

        /* Not sure what your use case will be... So going ham with the constructors.
         * Point is, you need a random object of some kind to run the simulation. */

        public WillTheDVDHitTheCornerSimulator(Random random) {
            this.random = random;
            Position = GetRandomPosition();
        }

        public WillTheDVDHitTheCornerSimulator(int randomSeed) {
            random = new(randomSeed);
            Position = GetRandomPosition();
        }

        public WillTheDVDHitTheCornerSimulator() {
            random = new();
            Position = GetRandomPosition();
        }

        private Vector2 GetRandomPosition() => new(random.NextSingle(),random.NextSingle());

        /// <summary>
        /// A normalized coordinate between <c>(0,0)</c> and <c>(1,1)</c>.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Expected to be in range of <c>(0,0)</c> to <c>(1,1)</c>. A velocity of <c>1</c> is equal to <c>DistancePerSecond</c>. Can be set manually, but will be overriden when a corner intersection occurs.
        /// </summary>
        public Vector2 Velocity { get; set; } = new Vector2(1f,1f);

        /// <summary>
        /// How far the object will travel in a given second for a single dimension, assuming a velocity value of <c>1</c>. Actual distances will vary.
        /// </summary>
        public float DistancePerSecond { get; set; } = 0.2f;

        /// <summary>
        /// The lower bound of a random velocity. The upper bound is always <c>1</c>. Higher values will seem more predictable, lower values will seem more slow.
        /// </summary>
        public float MinimumRandomVelocity { get; set; } = 0.75f;

        private float GetRandomVelocity() => random.NextSingle() * (1 - MinimumRandomVelocity) + MinimumRandomVelocity;

        /// <summary>
        /// Update the bouncing sprite simulation each frame that it is rendered.
        /// </summary>
        /// <param name="elapsedTime">How much time has elapsed since the last frame.</param>
        public void Update(TimeSpan elapsedTime) {
            float delta = (float)elapsedTime.Ticks / TimeSpan.TicksPerSecond;
            Vector2 position = Position, velocity = Velocity;
            position += Velocity * delta * DistancePerSecond;

            /* Not the most elegant physics solution.. but that's probably not why you're here. This prevents the coordinate getting lost in the void forever. */
            if(position.X < 0) {
                position.X = 0;
                velocity.X = GetRandomVelocity();
            } else if(position.X > 1) {
                position.X = 1;
                velocity.X = -GetRandomVelocity();
            }
            if(position.Y < 0) {
                position.Y = 0;
                velocity.Y = GetRandomVelocity();
            } else if(position.Y > 1) {
                position.Y = 1;
                velocity.Y = -GetRandomVelocity();
            }
            Position = position;
            Velocity = velocity;
        }

        private Vector2 GetTextureSize(Texture2D texture,Rectangle? textureSource) {
            if(textureSource.HasValue) {
                return textureSource.Value.Size.ToVector2();
            } else {
                return new Vector2(texture.Width,texture.Height);
            }
        }

        /* Sorry for the heavy annotation here. It is supposed to educational because graphics code is confusing. */

        /// <summary>
        /// Render the object to the screen. Coordinates are automatically corrected to minimize overdraw but cannot completely eliminate them.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to render with. This method does not call begin or end methods on its own: Use appropriately.</param>
        /// <param name="texture">The texture which contains the sprite to bounce around the screen.</param>
        /// <param name="scale">Scale to apply to <c>textureSource</c> into <c>bounds</c> relative coordinates. Supports realtime scale adjustment.</param>
        /// <param name="bounds">The screen area to bounce the sprite around in. For total viewport use <c>Viewport.Bounds</c>.</param>
        /// <param name="textureSource">The texture source area that contains the sprite.</param>
        /// <param name="rotation">The amount to rotation the sprite, around the center of its texture source.</param>
        public void Render(SpriteBatch spriteBatch,Texture2D texture,float scale,Rectangle bounds,Rectangle? textureSource = null,float rotation = 0) {
            Vector2 textureSize = GetTextureSize(texture,textureSource);
            Vector2 boundsSize = new Vector2(bounds.Width,bounds.Height);

            /* The coordinate simulation has no knowledge of the shape of the sprite. Edges are aligned by measuring the output sprite's relative size against the destination bounds. */
            Vector2 spriteToBoundsRatio = textureSize * scale / boundsSize;

            /* Scale the simulation position by the leftover bounds space. I.e, if a sprite is scaled to 40% of the bounds area, we multiply the simulation position by 60%. */
            Vector2 renderPosition = Position * (Vector2.One - spriteToBoundsRatio);

            /* Offset the renderPosition by half of the original sprite to bounds ratio. In the previous example of 40% width, this offset is 20%. Something like... [20% - 60% - 20%]. Make sense? */
            renderPosition += spriteToBoundsRatio * 0.5f;

            /* Now, scale our position into destination bounds space. */
            renderPosition *= boundsSize;

            /* Offset the position by the origin of the destination bounds. */
            renderPosition += new Vector2(bounds.X,bounds.Y);

            /* It's fine for 'textureSource' to be null here - 'GetTextureSize()' assumes the full texture size as well as SpriteBatch. */
            spriteBatch.Draw(texture,renderPosition,textureSource,Color.White,rotation,textureSize * 0.5f,scale,SpriteEffects.None,1);
        }
    }
}
