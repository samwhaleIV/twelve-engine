namespace TwelveEngine.UI.Book {
    public class SpriteElement:BookElement {

        public Texture2D Texture { get; set; }
        public Rectangle TextureSource { get; set; } = Rectangle.Empty;
        public Color Color { get; set; } = Color.White;

        public float Depth { get; set; } = 0.5f;

        public Action<SpriteBatch> OnRender;

        public float HeightToWidth {
            get {
                if(TextureSource == Rectangle.Empty) {
                    return 0;
                } else {
                    Point size = TextureSource.Size;
                    return (float)size.X / size.Y;
                }
            }
        }

        public float WidthToHeight {
            get {
                if(TextureSource == Rectangle.Empty) {
                    return 0;
                } else {
                    Point size = TextureSource.Size;
                    return (float)size.Y / size.X;
                }
            }
        }

        public float GetWidth(float height) => height * HeightToWidth;
        public float GetHeight(float width) => width * WidthToHeight;

        public void SizeByPixels(float pixelSize) {
            SizeMode = CoordinateMode.Absolute;
            Size = SourceSize * pixelSize;
        }

        public float SourceWidth => TextureSource.Width;
        public float SourceHeight => TextureSource.Height;

        public Vector2 SourceSize => TextureSource.Size.ToVector2();

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
            spriteBatch.Draw(texture,(Rectangle)destination,sourceArea,Color,MathHelper.ToRadians(ComputedArea.Rotation),origin,SpriteEffects.None,Depth);
        }

        public void Render(SpriteBatch spriteBatch) {
            Draw(spriteBatch,Texture,TextureSource);
            OnRender?.Invoke(spriteBatch);
        }

        public Point GetComputedCenterPoint() {
            return ComputedArea.Destination.Center.ToPoint();
        }

        public Vector2 GetComputedCenter() {
            return ComputedArea.Destination.Center;
        }
    }
}
