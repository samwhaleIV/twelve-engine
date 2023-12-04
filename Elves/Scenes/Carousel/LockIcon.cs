using Microsoft.Xna.Framework;
using TwelveEngine.Game3D.Entity.Types;

namespace Elves.Scenes.Carousel {
    public sealed class LockIcon:TextureEntity {

        public LockIcon() : base(Program.Textures.Lock) {
            PixelSmoothing = false;
            SetUVArea(0,0,16,16);
            Scale = new Vector3(0.25f,0.25f,1f);
        }

        public void UpdatePosition(CarouselItem owner) {
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
