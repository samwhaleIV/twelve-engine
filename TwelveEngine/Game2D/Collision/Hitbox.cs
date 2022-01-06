using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D {
    public readonly struct Hitbox {
        public Hitbox(
            Vector2 position,
            Vector2 size
        ) {
            Position = position;
            Size = size;
        }

        public readonly Vector2 Position;
        public readonly Vector2 Size;

        public float X => Position.X;
        public float Y => Position.Y;
        public float Width => Size.X;
        public float Height => Size.Y;

        public bool Collides(Hitbox target) {
            return X <= target.X + target.Width &&
            X + Width >= target.X &&
            Y <= target.Y + target.Height &&
            Height + Y >= target.Y;
        }

        public static Hitbox GetInteractionArea(Entity2D entity) {

            var boxSize = Constants.Config.InteractSize;
            var halfSize = boxSize / 2f;

            var location = Vector2.Zero;

            var direction = entity.Direction;
            if(direction == Direction.Left || direction == Direction.Right) {
                location.Y = (entity.Y + entity.Height / 2) - halfSize;
                if(direction == Direction.Left) {
                    location.X = entity.X - boxSize;
                } else {
                    location.X = entity.X + entity.Width;
                }
            } else {
                location.X = (entity.X + entity.Width / 2) - halfSize;
                if(direction == Direction.Up) {
                    location.Y = entity.Y - boxSize;
                } else {
                    location.Y = entity.Y + entity.Height;
                }
            }

            return new Hitbox(location,new Vector2(boxSize));
        }
    }
}
