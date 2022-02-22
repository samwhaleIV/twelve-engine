using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Physics;

namespace TwelveEngine.Game2D.Entity {
    public abstract class PlatformerPlayer:MovingEntity2D {

        private const float X_AXIS_MAX_VELOCITY = 8f;

        private const float X_AXIS_ACCEL = 8f;
        private const float X_AXIS_DECAY = 4f;
        private const float X_AXIS_REBOUND = 4f;

        private const float BASE_GRAVITY = 11f;

        private const float JUMP_VELOCITY = 24f;
        private const float JUMP_DECAY = 20f;
        private const float JUMP_REBOUND = 20f;

        private const float GROUND_COLLISION_DISTANCE = 0.1f;
        private const float JUMP_BUFFER_TIMEOUT = 133f;
        private const float IN_AIR_ACCEL_MULTIPLIER = 2.5f;

        private const float CEILING_HIT_DAMPENING = 2f;

        protected abstract float GetHorizontalForce();

        private TimeSpan? bufferedJumpTime;

        private TimeSpan GetCurrentTime() => Owner.Game.Time.TotalGameTime;

        private bool jumpIsPressed = false;

        protected void JumpStart(bool allowBuffer = true) {
            jumpIsPressed = true;
            if(TryJump() || !allowBuffer) {
                return;
            }
            bufferedJumpTime = GetCurrentTime();
        }

        protected void JumpEnd() => jumpIsPressed = false;

        private float GetJumpForce() => jumpIsPressed ? 0f : 1f;

        private bool IsJumping() => jumpPhysics.Velocity < 0f;

        private Vector2 GetGravityVelocity() {
            return new Vector2(0,BASE_GRAVITY);
        }

        private VelocityController movementPhysics = new VelocityController(new VelocityData() {
            Acceleration = X_AXIS_ACCEL,
            Decay = X_AXIS_DECAY,
            ReboundStrength = X_AXIS_REBOUND,
            MaxVelocity = X_AXIS_MAX_VELOCITY,
        });

        private VelocityController jumpPhysics = new VelocityController(new VelocityData() {
            Acceleration = JUMP_REBOUND, /* The jump didn't reach full height, this is how strong to decay */
            Decay = JUMP_DECAY,
            ReboundStrength = 1f,
            MaxVelocity = JUMP_VELOCITY,
        });

        private bool TryJump() {
            if(IsJumping() || !OnGround()) {
                return false;
            }
            jumpPhysics.Velocity = -JUMP_VELOCITY;
            return true;
        }

        private bool OnGround() {
            var oldPosition = Position;
            Position = new Vector2(oldPosition.X,oldPosition.Y + GROUND_COLLISION_DISTANCE);
            bool isOnGround = TryGetCollision(out _);
            Position = oldPosition;
            return isOnGround;
        }

        protected override Vector2 GetVelocity(float delta) {
            var velocity = GetGravityVelocity();

            float accelScale = 1f;
            if(!OnGround()) accelScale = IN_AIR_ACCEL_MULTIPLIER;

            movementPhysics.Force = GetHorizontalForce();
            movementPhysics.Update(delta,accelScale);

            jumpPhysics.Force = GetJumpForce();
            jumpPhysics.Update(delta,accelScale);

            velocity.X += movementPhysics.Velocity;
            var jumpVelocity = jumpPhysics.Velocity;
            velocity.Y += Math.Min(jumpVelocity,0f);

            return velocity * delta;
        }

        private bool ShouldDoAutoJump() {
            if(!bufferedJumpTime.HasValue) {
                return false;
            }
            var endTime = bufferedJumpTime.Value + TimeSpan.FromMilliseconds(JUMP_BUFFER_TIMEOUT);
            return endTime > GetCurrentTime();
        }

        protected override void OnCollideX() => movementPhysics.Velocity = 0f;

        private void OnJumpLanding() {
            bool doAutoJump = ShouldDoAutoJump();
            jumpPhysics.Velocity = 0f;
            if(doAutoJump) TryJump();
            bufferedJumpTime = null;
        }

        protected override void OnCollideY() {
            if(IsJumping()) jumpPhysics.Velocity /= CEILING_HIT_DAMPENING;
            if(!OnGround()) return;
            OnJumpLanding();
        }
    }
}
