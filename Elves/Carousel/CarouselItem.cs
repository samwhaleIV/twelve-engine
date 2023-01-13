using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game3D.Entity.Types;
using Microsoft.Xna.Framework;

namespace Elves.Carousel {

    public sealed class CarouselItem:TextureEntity {

        public RotationPosition OldRotationPosition { get; set; }
        public RotationPosition RotationPosition { get; set; }

        public Color TintColor { get; set; } = Color.White;

        public string DisplayName { get; set; } = "<None>";

        public CarouselItem(Texture2D texture,Rectangle source) : base(texture) {
            Billboard = true;
            SetUVArea(source);
            float width = (float)source.Width / source.Height;
            Position = new Vector3(0f,0f,DepthConstants.Middle);
            Scale = new Vector3(width, 1f,1f);
            PixelSmoothing = false;
        }
    }

}
