using TwelveEngine.Game2D.Collision.Poly;

namespace TwelveEngine.Game2D.Collision {
    public readonly struct CollisionResult {

        public readonly CollisionResultType Type { get; }

        public readonly Hitbox? Hitbox { get; }
        public readonly Line? ClipLine { get; }
        public readonly Line? Line { get; }

        public CollisionResult(CollisionResultType type) {
            Type = type;
            Hitbox = null;
            ClipLine = null;
            Line = null;
        }

        public CollisionResult(Hitbox hitbox) {
            Type = CollisionResultType.Hitbox;
            Hitbox = hitbox;
            ClipLine = null;
            Line = null;
        }

        public CollisionResult(Line line,Line clipLine) {
            Type = CollisionResultType.Line;
            Hitbox = null;
            Line = line;
            ClipLine = clipLine;
        }

        public readonly bool HasCollision => Type != CollisionResultType.None;
        public readonly bool IsHitbox => Type == CollisionResultType.Hitbox;
        public readonly bool IsLine => Type == CollisionResultType.Line;

        public static CollisionResult None => new CollisionResult(CollisionResultType.None);
    }
}
