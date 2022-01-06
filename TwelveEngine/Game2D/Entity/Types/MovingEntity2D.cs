using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine.Game2D.Entity.Types {
    public abstract class MovingEntity2D:Entity2D {

        protected abstract Point GetMovementDelta();
        protected abstract Hitbox GetHitbox();

        private bool shouldInteract = false;

        private int lastXDelta, lastYDelta;

        private TimeSpan? movingStart = null, deacelStart = null;

        private int deacelDeltaX, deacelDeltaY;
        private float deacelStartValue;

        protected float MaxSpeed { get; set; }
        protected float AccelRate { get; set; }
        protected float DeaccelRate { get; set; }
        protected float OrientationOffset { get; set; }

        protected int XDelta { get; private set; }
        protected int YDelta { get; private set; }

        protected Point MovementDelta => new Point(XDelta,YDelta);

        public event Action OnPositionChanged;
        protected Action OnMovementStopped;

        protected void QueueInteraction() => shouldInteract = true;

        private void interact() {
            shouldInteract = false;
            if(Owner.Interaction == null) {
                return;
            }
            Owner.Interaction.HitTest(this);
        }

        protected void UpdateMovement(GameTime gameTime) {
            var delta = GetMovementDelta();
            XDelta = delta.X; YDelta = delta.Y;

            var startPosition = new Vector2(X,Y);
            updateMovement(gameTime);
            var endPosition = new Vector2(X,Y);
            if(startPosition != endPosition) {
                OnPositionChanged?.Invoke();
            }
            if(shouldInteract) {
                interact();
            }
        }

        private float getLeftLimit(Hitbox self,Hitbox target) {
            return target.X + target.Width - (self.X - X);
        }
        private float getUpLimit(Hitbox self,Hitbox target) {
            return target.Y + target.Height - (self.Y - Y);
        }
        private float getRightLimit(Hitbox self,Hitbox target) {
            return target.X - self.Width - (self.X - X);
        }
        private float getDownLimit(Hitbox self,Hitbox target) {
            return target.Y - self.Height - (self.Y - Y);
        }

        private bool leftCollision(Hitbox self,Hitbox target) {
            if(Y == getUpLimit(self,target) || Y == getDownLimit(self,target)) {
                return false;
            }
            if(self.X < target.X) {
                X = getRightLimit(self,target);
            } else {
                X = getLeftLimit(self,target);
            }
            return true;
        }
        private bool upCollision(Hitbox self,Hitbox target) {
            if(X == getLeftLimit(self,target) || X == getRightLimit(self,target)) {
                return false;
            }
            if(self.Y < target.Y) {
                Y = getDownLimit(self,target);
            } else {
                Y = getUpLimit(self,target);
            }
            return true;
        }
        private bool rightCollision(Hitbox self,Hitbox target) {
            if(Y == getUpLimit(self,target) || Y == getDownLimit(self,target)) {
                return false;
            }
            if(self.X > target.X) {
                X = getLeftLimit(self,target);
            } else {
                X = getRightLimit(self,target);
            }
            return true;
        }
        private bool downCollision(Hitbox self,Hitbox target) {
            if(X == getLeftLimit(self,target) || X == getRightLimit(self,target)) {
                return false;
            }
            if(self.Y > target.Y) {
                Y = getUpLimit(self,target);
            } else {
                Y = getDownLimit(self,target);
            }
            return true;
        }

        private void move(Direction direction,float distance) {

            switch(direction) {
                case Direction.Down: Y += distance; break;
                case Direction.Up: Y -= distance; break;
                case Direction.Left: X -= distance; break;
                case Direction.Right: X += distance; break;
            }

            Hitbox self = GetHitbox();

            var collisionData = Owner.Collision.Collides(self);
            if(collisionData.Count == 0) {
                return;
            }
            foreach(var target in collisionData) { /* Switch case should be inverted with loop */
                switch(direction) {
                    case Direction.Down: if(downCollision(self,target)) return; break;
                    case Direction.Up: if(upCollision(self,target)) return; break;
                    case Direction.Left: if(leftCollision(self,target)) return; break;
                    case Direction.Right: if(rightCollision(self,target)) return; break;
                }
            }
        }

        private Direction getDirection() {
            Direction direction = Direction;
            if(XDelta != 0) {
                if(XDelta < 0) {
                    direction = Direction.Left;
                } else {
                    direction = Direction.Right;
                }
            }
            if(YDelta != 0) {
                if(YDelta < 0) {
                    direction = Direction.Up;
                } else {
                    direction = Direction.Down;
                }
            }
            return direction;
        }

        private void handleDeltas(float distance) {
            if(XDelta != 0) {
                if(XDelta < 0) {
                    move(Direction.Left,distance);
                } else {
                    move(Direction.Right,distance);
                }
            }
            if(YDelta != 0) {
                if(YDelta < 0) {
                    move(Direction.Up,distance);
                } else {
                    move(Direction.Down,distance);
                }
            }
        }

        private bool isHorizontal(Direction direction) {
            return direction == Direction.Left || direction == Direction.Right;
        }
        private bool isVertical(Direction direction) {
            return direction == Direction.Up || direction == Direction.Down;
        }

        private float getSpeed(TimeSpan totalTime) {
            float t;
            if(movingStart.HasValue) {
                t = (float)Math.Min((totalTime - movingStart.Value).TotalMilliseconds / AccelRate,1);
                return MaxSpeed * t;
            } else if(deacelStart.HasValue) {
                t = (float)Math.Min((totalTime - deacelStart.Value).TotalMilliseconds / DeaccelRate,1);
                return deacelStartValue * (1 - t);
            } else {
                return 0f;
            }
        }

        private void handleAutomaticOffset(Direction oldDirection) {
            Hitbox hitbox = GetHitbox();
            List<Hitbox> collisionResult = Owner.Collision.Collides(hitbox);

            var hadCollision = false;
            foreach(var target in collisionResult) {
                if(
                    (hitbox.Y > target.Y && Y == getUpLimit(hitbox,target)) ||
                    (hitbox.Y < target.Y && Y == getDownLimit(hitbox,target))
                ) {
                    continue;
                }
                hadCollision = true;
                break;
            }

            if(!hadCollision) {
                return;
            }

            if(oldDirection == Direction.Right) {
                X -= OrientationOffset;
            } else {
                X += OrientationOffset;
            }

            hitbox = GetHitbox();

            collisionResult = Owner.Collision.Collides(hitbox);
            if(collisionResult.Count <= 0) {
                return;
            }

            var firstHit = collisionResult[0];

            if(oldDirection == Direction.Right) {
                leftCollision(hitbox,firstHit);
            } else {
                rightCollision(hitbox,firstHit);
            }
        }

        private void updateDirection() {
            var newDirection = getDirection();
            var oldDirection = Direction;
            Direction = newDirection;

            if(newDirection != oldDirection && isHorizontal(oldDirection) && isVertical(newDirection)) {
                handleAutomaticOffset(oldDirection);
            }
        }

        private void processAccelerationTiming(TimeSpan totalTime) {
            if(XDelta == 0 && YDelta == 0) {
                OnMovementStopped?.Invoke();
                if(deacelStart.HasValue) {
                    var timeDifference = deacelStart.Value - totalTime;
                    if(timeDifference.TotalMilliseconds >= DeaccelRate) {
                        deacelStart = null;
                    }
                } else if(movingStart.HasValue) {
                    deacelStartValue = getSpeed(totalTime);

                    deacelDeltaX = lastXDelta;
                    deacelDeltaY = lastYDelta;

                    deacelStart = totalTime;
                    movingStart = null;
                }
            } else if(!movingStart.HasValue) {
                deacelStart = null;
                movingStart = totalTime;
            }
            lastXDelta = XDelta;
            lastYDelta = YDelta;
        }

        private void handleDeaccelerationDistance(float distance) {
            var oldXDelta = XDelta;
            var oldYDelta = YDelta;

            XDelta = deacelDeltaX;
            YDelta = deacelDeltaY;

            updateDirection();
            handleDeltas(distance);

            XDelta = oldXDelta;
            YDelta = oldYDelta;
        }

        private void updateMovement(GameTime gameTime) {
            TimeSpan totalTime = gameTime.TotalGameTime;

            processAccelerationTiming(totalTime);

            float speed = getSpeed(totalTime);
            if(speed <= 0.0) return;

            double delta = gameTime.ElapsedGameTime.TotalSeconds;
            float distance = (float)(delta * speed);

            if(!movingStart.HasValue) {
                handleDeaccelerationDistance(distance);
                return;
            }

            updateDirection();
            handleDeltas(distance);
        }

        public abstract void Update(GameTime gameTime);
    }
}
