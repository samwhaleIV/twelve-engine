using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TwelveEngine.UI {
    public class SpriteElement:Element {

        public Texture2D Texture { get; set; }
        public Rectangle? TextureSource { get; set; } = null;
        public Color Color { get; set; } = Color.White;

        public float Depth { get; set; } = 0.5f;

        protected void UpdateScaleForInteraction(TimeSpan now) {
            float newScale = 1f;
            if(Selected) {
                newScale = 1.05f;
            }
            if(Pressed) {
                newScale *= 0.95f;
            }
            if(Scale == newScale) {
                return;
            }
            KeyAnimation(now);
            Scale = newScale;
        }

        public void Render(SpriteBatch spriteBatch) {
            VectorRectangle destination = ComputedArea.Destination;
            destination.Position += destination.Size * 0.5f;

            Vector2 origin = TextureSource.Value.Size.ToVector2() * 0.5f;
            spriteBatch.Draw(Texture,(Rectangle)destination,TextureSource,Color.White,MathHelper.ToRadians(ComputedArea.Rotation),origin,SpriteEffects.None,Depth);
        }
    }
}
