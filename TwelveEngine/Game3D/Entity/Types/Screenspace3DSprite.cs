namespace TwelveEngine.Game3D.Entity.Types {
    public class Screenspace3DSprite:TextureEntity {

        public Screenspace3DSprite() => Initialize();

        public Screenspace3DSprite(string textureName) : base(textureName) => Initialize();
        public Screenspace3DSprite(Texture2D texture) : base(texture) => Initialize();

        public FloatRectangle Area { get; set; } = FloatRectangle.Empty; /* Space in pixels */
        public Rectangle TextureSource { get; set; }

        private void Initialize() => OnPreRender += UpdateVerticesArea;

        public Vector2 RotationOrigin { get; set; } = new Vector2(0.5f,0.5f);

        private void UpdateVerticesArea() {
            /* This took 5 hours. I suck at math, apparently */
            SetUVArea(TextureSource);

            var orthoArea = Owner.Camera.OrthographicArea;
            var viewportSize = Owner.Viewport.Bounds.Size.ToVector2();

            float left = Area.Left / viewportSize.X * orthoArea.Width + orthoArea.Left;
            float right = Area.Right / viewportSize.X * orthoArea.Width + orthoArea.Left;

            float top = Area.Top / viewportSize.Y * orthoArea.Height + orthoArea.Top;
            float bottom = Area.Bottom / viewportSize.Y * orthoArea.Height + orthoArea.Top;

            float centerX = left + (right - left) * RotationOrigin.X;
            float centerY = top + (bottom - top) * RotationOrigin.Y;

            TopLeft = new Vector3(left - centerX,centerY - top,0f);
            BottomRight = new Vector3(right - centerX,centerY - bottom,0f);
            Position = new Vector3(centerX,-centerY,Position.Z);
        }
    }
}
