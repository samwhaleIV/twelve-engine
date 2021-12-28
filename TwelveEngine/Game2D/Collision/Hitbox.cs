namespace TwelveEngine.Game2D {
    public struct Hitbox {
        public Hitbox(
            float x, float y,
            float width, float height
        ) {
            X = x; Y = y;
            Width = width; Height = height;
        }

        public float X, Y, Width, Height;

        public bool Collides(Hitbox target) {
            return X <= target.X + target.Width &&
            X + Width >= target.X &&
            Y <= target.Y + target.Height &&
            Height + Y >= target.Y;
        }

        public static Hitbox GetInteractionArea(Entity2D entity) {

            var boxSize = Constants.InteractionBoxSize;
            var hitbox = new Hitbox() {
                Width = boxSize,Height = boxSize
            };

            var direction = entity.Direction;
            if(direction == Direction.Left || direction == Direction.Right) {
                hitbox.Y = (entity.Y + entity.Height / 2) - hitbox.Height / 2;
                if(direction == Direction.Left) {
                    hitbox.X = entity.X - hitbox.Width;
                } else {
                    hitbox.X = entity.X + entity.Width;
                }
            } else {
                hitbox.X = (entity.X + entity.Width / 2) - hitbox.Width / 2;
                if(direction == Direction.Up) {
                    hitbox.Y = entity.Y - hitbox.Height;
                } else {
                    hitbox.Y = entity.Y + entity.Height;
                }
            }
            return hitbox;
        }
    }
}
