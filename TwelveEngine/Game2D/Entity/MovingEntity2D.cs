using TwelveEngine.Game2D.Collision;
using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine.Game2D.Entity {
    public abstract class MovingEntity2D:Entity2D {

        protected abstract Vector2 GetVelocity(float delta);
        protected abstract void OnCollideX();
        protected abstract void OnCollideY();

        protected bool TryGetCollision(out Hitbox hitbox) {
            var result = Owner.Collision.Collides(new Hitbox(Position,Size));
            if(result.HasValue) {
                hitbox = result.Value;
                return true;
            }
            hitbox = new Hitbox();
            return false;
        }

        public void UpdateMovement(GameTime gameTime) {
            TimeSpan timeDelta = gameTime.ElapsedGameTime;
            float delta = (float)timeDelta.TotalSeconds;

            var velocity = GetVelocity(delta);

            var oldPosition = Position;
            Hitbox hitbox;
            Position = new Vector2(oldPosition.X + velocity.X,oldPosition.Y);
            if(TryGetCollision(out hitbox)) {
                var newPosition = oldPosition;
                if(velocity.X < 0) {
                    newPosition.X = hitbox.Right;
                    OnCollideX();
                } else {
                    newPosition.X = hitbox.X - Width;
                    OnCollideX();
                }
                Position = newPosition;
            }

            oldPosition = Position;
            Position = new Vector2(oldPosition.X,oldPosition.Y + velocity.Y);
            if(TryGetCollision(out hitbox)) {
                var newPosition = oldPosition;
                if(velocity.Y < 0) {
                    newPosition.Y = hitbox.Bottom;
                    OnCollideY();
                } else {
                    newPosition.Y = hitbox.Y - Height;
                    OnCollideY();
                }
                Position = newPosition;
            }
        }
    }
}
