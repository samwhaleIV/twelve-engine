using TwelveEngine.Game2D.Collision;
using Microsoft.Xna.Framework;
using System;
using TwelveEngine.Game2D.Collision.Poly;

namespace TwelveEngine.Game2D.Entity {
    public abstract class MovingEntity2D:Entity2D {

        protected abstract Vector2 GetVelocity(float delta);

        protected abstract void OnCollideX();
        protected abstract void OnCollideY();

        protected bool TryGetCollision(out CollisionResult result) {
            result = Owner.Collision.Collides(new Hitbox(Position,Size));
            return result.HasCollision;
        }

        private float HitboxCollisionX(float velocity,float oldPosition,Hitbox hitbox) {
            float x;
            if(velocity < 0) {
                x = hitbox.Right;
            } else {
                x = hitbox.X - Width;
            }
            return x;
        }

        private float HitboxCollisionY(float velocity,float oldPosition,Hitbox hitbox) {
            float y;
            if(velocity < 0) {
                y = hitbox.Bottom;
            } else {
                y = hitbox.Y - Height;
            }
            return y;
        }

        private float LineCollisionX(float velocity,float oldPosition,Line[] lines) {
            if(lines.Length > 1) {
                return oldPosition;
            }
            Line line = lines[0];
            float x;
            if(velocity < 0) {
                x = Math.Max(line.A.X,line.B.X);
            } else {
                x = Math.Min(line.A.X,line.B.X) - Width;
            }
            return x;
        }

        private float LineCollisionY(float velocity,float oldPosition,Line[] lines) {
            if(lines.Length > 1) {
                return oldPosition;
            } 
            Line line = lines[0];
            float y;
            if(velocity < 0) {
                y = Math.Max(line.A.Y,line.B.Y);
            } else {
                y = Math.Min(line.A.Y,line.B.Y) - Height;
            }
            return y;
        }

        private float CollisionPassX(CollisionResult result,float velocity,float oldPosition) {
            switch(result.Type) {
                case CollisionResultType.Hitbox:
                    return HitboxCollisionX(velocity,oldPosition,result.Hitbox.Value);
                case CollisionResultType.Line:
                    return LineCollisionX(velocity,oldPosition,result.ClipLines);
                default:
                    throw new NotImplementedException();
            }
        }

        private float CollisionPassY(CollisionResult result,float velocity,float oldPosition) {
            switch(result.Type) {
                case CollisionResultType.Hitbox:
                    return HitboxCollisionY(velocity,oldPosition,result.Hitbox.Value);
                case CollisionResultType.Line:
                    return LineCollisionY(velocity,oldPosition,result.ClipLines);
                default:
                    throw new NotImplementedException();
            }
        }

        public void UpdateMovement(GameTime gameTime) {
            Vector2 velocity = GetVelocity((float)gameTime.ElapsedGameTime.TotalSeconds);

            Vector2 oldPosition = Position;
            Position = new Vector2(oldPosition.X + velocity.X,oldPosition.Y);

            bool collideX = false, collideY = false;

            if(velocity.X != 0 && TryGetCollision(out CollisionResult result)) {
                float newX = CollisionPassX(result,velocity.X,oldPosition.X);
                Position = new Vector2(newX,oldPosition.Y);
                collideX = true;
            }

            oldPosition = Position;
            Position = new Vector2(oldPosition.X,oldPosition.Y + velocity.Y);

            if(velocity.Y != 0 && TryGetCollision(out result)) {
                float newY = CollisionPassY(result,velocity.Y,oldPosition.Y);
                Position = new Vector2(oldPosition.X,newY);
                collideY = true;
            }

            if(collideX) {
                OnCollideX();
            }
            if(collideY) {
                OnCollideY();
            }
        }
    }
}
