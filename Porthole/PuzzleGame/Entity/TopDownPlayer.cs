using Microsoft.Xna.Framework;
using System;
using Porthole.Physics;
using TwelveEngine.Shell.Input;
using TwelveEngine.Serial;
using Porthole.Collision;

namespace Porthole.PuzzleGame.Entity {
    public abstract class TopDownPlayer:MovingEntity2D {

        protected bool TryInteract() {
            var interactionBox = Hitbox.GetInteractionArea(Position,Size,Direction);
            foreach(var target in Owner.Entities.GetByType<IInteract>()) {
                if(interactionBox.Collides(target.GetHitbox())) {
                    target.Interact(this);
                    return true;
                }
            }
            return false;
        }

        private const float MAX_SPEED = 2.5f;

        private VelocityController2D movementPhysics = new VelocityController2D() {
            MaxVelocity = MAX_SPEED,
            ReboundStrength = 8f,
            Acceleration = 16f,
            Decay = 32f,
        };

        protected event Action<GameTime> OnMovementStopped, OnMovementStarted;

        public bool IsMoving => movementPhysics.X.Velocity != 0f || movementPhysics.Y.Velocity != 0f;

        protected abstract Vector2 GetForce();

        private void UpdateFacingDirection() {
            float x = movementPhysics.X.Velocity, y = movementPhysics.Y.Velocity;

            if(Math.Abs(x) == Math.Abs(y)) {
                return;
            }

            if(Math.Abs(x) > Math.Abs(y)) {
                Direction = x < 0 ? Direction.Left : Direction.Right;
            } else {
                Direction = y < 0 ? Direction.Up : Direction.Down;
            }
        }

        protected override Vector2 GetVelocity(float delta) {

            var startVelocity = movementPhysics.Velocity;

            movementPhysics.Force = GetForce();
            movementPhysics.Update(delta);

            var endVelocity = movementPhysics.Velocity;

            var velocity = movementPhysics.Velocity * delta;

            UpdateFacingDirection();

            if(startVelocity == endVelocity) {
                return velocity;
            }

            if(startVelocity == Vector2.Zero) {
                OnMovementStarted?.Invoke(Owner.Game.Time);
            } else if(endVelocity == Vector2.Zero) {
                OnMovementStopped?.Invoke(Owner.Game.Time);
            }

            return velocity;
        }

        protected override void OnCollideX() {
            movementPhysics.X.Velocity = 0f;
        }

        protected override void OnCollideY() {
            movementPhysics.Y.Velocity = 0f;
        }
    }
}