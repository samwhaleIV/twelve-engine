using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game3D.Entity.Types;

namespace Elves.Scenes.Carousel {
    public sealed class LockIcon:TextureEntity {
        private readonly CarouselItem owner;

        public LockIcon(CarouselItem owner) : base(Program.Textures.Lock) {
            PixelSmoothing = false;
            this.owner = owner;
            owner.OnUpdate += UpdatePosition;
            SetUVArea(0,0,16,16);
            Scale = new Vector3(0.25f,0.25f,1f);
        }

        private void UpdatePosition() {
            IsVisible = owner.IsLocked && owner.IsVisible;
            if(!IsVisible) {
                return;
            }
            var position = owner.Position;
            position.Z += 0.05f;
            Position = position;
            Alpha = owner.Alpha;
        }

    }
}
