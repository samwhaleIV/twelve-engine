using TwelveEngine.Game2D.Collision.Poly;

namespace TwelveEngine.Game2D.Collision {
    public readonly struct CollisionResult {

        public readonly CollisionResultType Type { get; }

        public readonly Hitbox? Hitbox { get; }
        public readonly Line[] ClipLines { get; }

        public CollisionResult(CollisionResultType type) {
            Type = type;
            Hitbox = null;
            ClipLines = null;
        }

        public CollisionResult(Hitbox hitbox) {
            Type = CollisionResultType.Hitbox;
            Hitbox = hitbox;
            ClipLines = null;
        }

        public CollisionResult(Line[] clipLines) {
            Type = CollisionResultType.Line;
            Hitbox = null;
            ClipLines = clipLines;
        }

        public readonly bool HasCollision => Type != CollisionResultType.None;
        public readonly bool IsHitbox => Type == CollisionResultType.Hitbox;
        public readonly bool IsLine => Type == CollisionResultType.Line;

        public static CollisionResult None => new CollisionResult(CollisionResultType.None);
    }
}
