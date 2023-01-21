using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game3D.Entity.Types;
using Microsoft.Xna.Framework;

namespace Elves.Scenes.Carousel {
    public sealed class CarouselItem:TextureEntity {

        public RotationPosition OldRotationPosition { get; set; }
        public RotationPosition RotationPosition { get; set; }

        public Color TintColor { get; set; } = Color.White;

        public string DisplayName { get; set; } = Constants.Battle.NoName;

        public bool IsLocked { get; set; } = false;

        public int Index { get; set; } = -1;

        public CarouselItem(Texture2D texture,Rectangle source) : base(texture) {
            Billboard = true;
            SetUVArea(source);
            float width = (float)source.Width / source.Height;
            Position = new Vector3(0f,0f,Constants.Depth.Middle);
            Scale = new Vector3(width, 1f,1f);
            PixelSmoothing = false;
            OnUpdate += CarouselItem_OnUpdate;
            OnLoad += CarouselItem_OnLoad;
        }

        private void CarouselItem_OnLoad() {
            Owner.Entities.Add(new LockIcon(Texture,this));
        }

        private void CarouselItem_OnUpdate() {
            Color = IsLocked ? Color.Black : Color.White;
        }
    }

}
