using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D.Collision {
    public readonly struct CollisionType {
        public CollisionType(
            Point location,Point size,float tileSize
        ) {
            Location = location.ToVector2() / tileSize;
            Size = size.ToVector2() / tileSize;
        }
        public readonly Vector2 Location;
        public readonly Vector2 Size;
    }
}
