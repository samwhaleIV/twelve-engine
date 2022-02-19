using System;
using TwelveEngine.Game2D.Collision;
using TwelveEngine.Time;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D.Entity.Types {
    public abstract class MovingEntity2D:Entity2D {

        public abstract Point GetMovementDelta();
        protected abstract CollisionData GetCollisionData();
        protected abstract void ResolveCollision(CollisionData collisionData);

        private readonly VelocityController horizontalVelocity, verticalVelocity;

        public MovingEntity2D() {
            horizontalVelocity = new VelocityController();
            verticalVelocity = new VelocityController();
        }

        private float _accel, _decay, _maxVelocity;

        public float Accel {
            get => _accel;
            set {
                horizontalVelocity.Accel = value;
                verticalVelocity.Accel = value;
                _accel = value;
            }
        }
        public float Decay {
            get => _decay;
            set {
                horizontalVelocity.Decay = value;
                verticalVelocity.Decay = value;
                _decay = value;
            }

        }
        public float MaxVelocity {
            get => _maxVelocity;
            set {
                horizontalVelocity.MaxVelocity = value;
                verticalVelocity.MaxVelocity = value;
                _maxVelocity = value;
            }
        }

        private Point currentDelta = Point.Zero;

        private void UpdateHorizontalMovement(int value) {
            if(currentDelta.X == value) return;
            if(value == 0 && currentDelta.X != 0) {
                horizontalVelocity.SoftDecay();
            } else if(value != 0 && currentDelta.X == 0) {
                horizontalVelocity.SoftAccel(Math.Sign(value));
            } else if(value != 0 && currentDelta.X != 0) {
                horizontalVelocity.SoftAccel(Math.Sign(value));
            }
            currentDelta.X = value;
        }

        private void UpdateVerticalMovement(int value) {
            if(currentDelta.Y == value) return;
            if(value == 0 && currentDelta.Y != 0) {
                verticalVelocity.SoftDecay();
            } else if(value != 0 && currentDelta.Y == 0) {
                verticalVelocity.SoftAccel(Math.Sign(value));
            } else if(value != 0 && currentDelta.Y != 0) {
                verticalVelocity.SoftAccel(Math.Sign(value));
            }
            currentDelta.Y = value;
        }

        private void TestCollision() {
            var data = GetCollisionData();

            var direction = data.Direction;
            if(direction == Point.Zero) return;

            ResolveCollision(data);

            if(direction.X != 0) {
                horizontalVelocity.HardDecay();
                currentDelta.X = 0;
            }

            if(direction.Y != 0) {
                verticalVelocity.HardDecay();
                currentDelta.Y = 0;
            }
        }

        public void UpdateMovement(GameTime gameTime) {
            var movementDelta = GetMovementDelta();

            UpdateHorizontalMovement(movementDelta.X);
            UpdateVerticalMovement(movementDelta.Y);

            var timeDelta = gameTime.ElapsedGameTime;
            horizontalVelocity.Update(timeDelta);
            verticalVelocity.Update(timeDelta);

            Position += new Vector2(
                horizontalVelocity.Velocity,
                verticalVelocity.Velocity
            ) * (float)timeDelta.TotalSeconds;
            TestCollision();
        }
    }
}
