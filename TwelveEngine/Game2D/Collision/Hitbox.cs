using Microsoft.Xna.Framework;
using TwelveEngine.Game2D.Entity;
using TwelveEngine.Shell.Input;

namespace TwelveEngine.Game2D.Collision {
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

        public float Right => Position.X + Size.X;
        public float Bottom => Position.Y + Size.Y;

        public readonly bool Collides(Hitbox target) {
            return X < target.X + target.Width &&
                   X + Width > target.X &&
                   Y < target.Y + target.Height &&
                   Height + Y > target.Y;
        }

        public static Hitbox GetInteractionArea(Vector2 origin,Vector2 size,Direction direction) {

            var boxSize = Constants.Config.InteractSize;
            var halfSize = boxSize * 0.5f;

            var location = Vector2.Zero;

            if(direction == Direction.Left || direction == Direction.Right) {
                location.Y = (origin.Y + size.Y * 0.5f) - halfSize;
                if(direction == Direction.Left) {
                    location.X = origin.X - boxSize;
                } else {
                    location.X = origin.X + size.X;
                }
            } else {
                location.X = (origin.X + size.X * 0.5f) - halfSize;
                if(direction == Direction.Up) {
                    location.Y = origin.Y - boxSize;
                } else {
                    location.Y = origin.Y + size.Y;
                }
            }

            return new Hitbox(location,new Vector2(boxSize));
        }
    }
}
