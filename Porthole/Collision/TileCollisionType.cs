using Microsoft.Xna.Framework;

namespace Porthole.Collision {
    public readonly struct TileCollisionType {
        public TileCollisionType(
            Point location,Point size,float tileSize
        ) {
            Location = location.ToVector2() / tileSize;
            Size = size.ToVector2() / tileSize;
        }
        public readonly Vector2 Location;
        public readonly Vector2 Size;
    }
}
