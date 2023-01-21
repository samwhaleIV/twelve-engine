using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine;

namespace Elves.UI.SpriteUI {
    public class SpriteElement:Element {

        public Texture2D Texture { get; set; }
        public Rectangle? TextureSource { get; set; } = null;
        public Color Color { get; set; } = Color.White;

        public float Depth { get; set; } = 0.5f;

        public void Render(SpriteBatch spriteBatch) {
            if(!TextureSource.HasValue || ComputedArea.Destination.Size == Vector2.Zero) {
                return;
            }
            VectorRectangle destination = ComputedArea.Destination;
            destination.Position += destination.Size * 0.5f;

            Vector2 origin = TextureSource.Value.Size.ToVector2() * 0.5f;
            spriteBatch.Draw(Texture,(Rectangle)destination,TextureSource,Color.White,MathHelper.ToRadians(ComputedArea.Rotation),origin,SpriteEffects.None,Depth);
        }
    }
}
