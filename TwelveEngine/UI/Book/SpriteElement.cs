using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TwelveEngine.UI.Book {
    public class SpriteElement:BookElement {

        public Texture2D Texture { get; set; }
        public Rectangle? TextureSource { get; set; } = null;
        public Color Color { get; set; } = Color.White;

        public float Depth { get; set; } = 0.5f;

        public Action<SpriteBatch> OnRender;

        public float HeightToWidth {
            get {
                if(!TextureSource.HasValue) {
                    return 0;
                } else {
                    Point size = TextureSource.Value.Size;
                    return (float)size.X / size.Y;
                }
            }
        }

        public float WidthToHeight {
            get {
                if(!TextureSource.HasValue) {
                    return 0;
                } else {
                    Point size = TextureSource.Value.Size;
                    return (float)size.Y / size.X;
                }
            }
        }

        public float GetWidth(float height) => height * HeightToWidth;
        public float GetHeight(float width) => width * WidthToHeight;

        public float SourceWidth => TextureSource.HasValue ? TextureSource.Value.Size.X : 0;
        public float SourceHeight => TextureSource.HasValue ? TextureSource.Value.Size.Y : 0;

        public Vector2 SourceSize => TextureSource.HasValue ? TextureSource.Value.Size.ToVector2() : Vector2.Zero;

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

        protected void Draw(SpriteBatch spriteBatch,Texture2D texture,Rectangle sourceArea) {
            FloatRectangle destination = ComputedArea.Destination;
            destination.Position += destination.Size * 0.5f;

            Vector2 origin = sourceArea.Size.ToVector2() * 0.5f;
            spriteBatch.Draw(texture,(Rectangle)destination,sourceArea,Color.White,MathHelper.ToRadians(ComputedArea.Rotation),origin,SpriteEffects.None,Depth);
        }

        public void Render(SpriteBatch spriteBatch) {
            Draw(spriteBatch,Texture,TextureSource.Value);
            OnRender?.Invoke(spriteBatch);
        }
    }
}
