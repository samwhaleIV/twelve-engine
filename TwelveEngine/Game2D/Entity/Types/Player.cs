using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D.Entity.Types {
    public sealed class Player:Entity2D, IUpdateable, IRenderable {

        protected override int GetEntityType() => Entity2DType.Player;

        public Player() {
            OnLoad += Player_OnLoad;
            OnUnload += Player_OnUnload;
        }

        private void Player_OnLoad() {
            playerTexure = Game.Content.Load<Texture2D>(Constants.PlayerImage);
            Game.ImpulseHandler.OnAcceptDown += ImpulseHandler_OnAcceptDown;
        }

        public void Player_OnUnload() {
            Game.ImpulseHandler.OnAcceptDown -= ImpulseHandler_OnAcceptDown;
        }

        private void ImpulseHandler_OnAcceptDown() {
            shouldInteract = true;
        }

        private const float FRAME_TIME = 300;
        private const float BLINK_RATE = 2900;
        private const float BLINK_TIME = 200;

        private const float ANIM_JUMP_START = 0.5f;
        private const int ANIM_ROWS = 4;

        private float maxSpeed = Constants.DefaultPlayerSpeed;

        private int lastDeltaX = 0, lastDeltaY = 0;
        private int xDelta = 0, yDelta = 0;

        private TimeSpan lastBlink;
        private TimeSpan? movingRenderStart = null;

        private TimeSpan? movingStart = null;
        private TimeSpan? deacelStart = null;

        private int deacelDeltaX, deacelDeltaY;
        private float deacelStartValue;

        private bool shouldInteract = false;
        private Texture2D playerTexure;

        public event Action PositionChanged;

        /* Notice: Hitbox math expects a symmetrical, entity-center aligned hitbox */
        private const float VERTICAL_HITBOX_X = 1 / 16f;
        private const float VERTICAL_HITBOX_WIDTH = 14 / 16f;

        private const float HORIZONTAL_HITBOX_X = 2 / 16f;
        private const float HORIZONTAL_HITBOX_WIDTH = 12 / 16f;

        private const float HITBOX_ORIENTATION_OFFSET = HORIZONTAL_HITBOX_X - VERTICAL_HITBOX_X;

        public Hitbox GetHitbox() {
            /* Notice: This hitbox does not scale with Entity Width and Height properties */
            var hitbox = new Hitbox();

            hitbox.Y = Y;
            hitbox.Height = 1;

            if(Direction == Direction.Down || Direction == Direction.Up) {
                hitbox.X = X + VERTICAL_HITBOX_X;
                hitbox.Width = VERTICAL_HITBOX_WIDTH;
            } else {
                hitbox.X = X + HORIZONTAL_HITBOX_X;
                hitbox.Width = HORIZONTAL_HITBOX_WIDTH;
            }

            return hitbox;
        }

        private void keyHandler() {
            xDelta = 0; yDelta = 0;
            if(IsKeyDown(Impulse.Up)) {
                yDelta--;
            }
            if(IsKeyDown(Impulse.Down)) {
                yDelta++;
            }
            if(IsKeyDown(Impulse.Left)) {
                xDelta--;
            }
            if(IsKeyDown(Impulse.Right)) {
                xDelta++;
            }
        }

        private void interact() {
            shouldInteract = false;
            if(Owner.Interaction == null) {
                return;
            }
            Owner.Interaction.HitTest(this);
        }

        public float Speed {
            get => maxSpeed;
            set => maxSpeed = value;
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
            if(xDelta != 0) {
                if(xDelta < 0) {
                    direction = Direction.Left;
                } else {
                    direction = Direction.Right;
                }
            }
            if(yDelta != 0) {
                if(yDelta < 0) {
                    direction = Direction.Up;
                } else {
                    direction = Direction.Down;
                }
            }
            return direction;
        }

        private void handleDeltas(float distance) {
            if(xDelta != 0) {
                if(xDelta < 0) {
                    move(Direction.Left,distance);
                } else {
                    move(Direction.Right,distance);
                }
            }
            if(yDelta != 0) {
                if(yDelta < 0) {
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
                t = (float)Math.Min((totalTime - movingStart.Value).TotalMilliseconds / Constants.PlayerAccel,1);
                return maxSpeed * t;
            } else if(deacelStart.HasValue) {
                t = (float)Math.Min((totalTime - deacelStart.Value).TotalMilliseconds / Constants.PlayerDeaccel,1);
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
                X -= HITBOX_ORIENTATION_OFFSET;
            } else {
                X += HITBOX_ORIENTATION_OFFSET;
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
            if(xDelta == 0 && yDelta == 0) {
                movingRenderStart = null;
                if(deacelStart.HasValue) {
                    var timeDifference = deacelStart.Value - totalTime;
                    if(timeDifference.TotalMilliseconds >= Constants.PlayerDeaccel) {
                        deacelStart = null;
                    }
                } else if(movingStart.HasValue) {
                    deacelStartValue = getSpeed(totalTime);

                    deacelDeltaX = lastDeltaX;
                    deacelDeltaY = lastDeltaY;

                    deacelStart = totalTime;
                    movingStart = null;
                }
            } else if(!movingStart.HasValue) {
                deacelStart = null;
                movingStart = totalTime;
            }
            lastDeltaX = xDelta;
            lastDeltaY = yDelta;
        }

        private void handleDeaccelerationDistance(float distance) {
            var oldXDelta = xDelta;
            var oldYDelta = yDelta;

            xDelta = deacelDeltaX;
            yDelta = deacelDeltaY;

            updateDirection();
            handleDeltas(distance);

            xDelta = oldXDelta;
            yDelta = oldYDelta;
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

        public void Update(GameTime gameTime) {
            keyHandler();
            var startPosition = (X, Y);
            updateMovement(gameTime);
            var endPosition = (X, Y);
            if(startPosition.CompareTo(endPosition) == 0) {
                PositionChanged?.Invoke();
            }
            if(shouldInteract) {
                interact();
            }
            var camera = Owner.Camera;
            camera.X = X; camera.Y = Y;
        }

        private int getRenderColumn() => (int)Direction;

        private bool isBlinking(GameTime gameTime) {
            var currentTime = gameTime.TotalGameTime;
            var interval = currentTime - lastBlink;

            var duration = interval.TotalMilliseconds;

            if(duration > BLINK_RATE) {
                if(duration > BLINK_RATE + BLINK_TIME) {
                    lastBlink = currentTime;
                    return false;
                } else {
                    return true;
                }
            } else {
                return false;
            }
        }

        private int getRenderRow(GameTime gameTime) {

            if(xDelta == 0 && yDelta == 0) {
                return isBlinking(gameTime) ? ANIM_ROWS : 0;
            }

            var now = gameTime.TotalGameTime;

            if(!movingRenderStart.HasValue) {
                movingRenderStart = now - TimeSpan.FromMilliseconds(FRAME_TIME * ANIM_JUMP_START);
            }

            var timeDifference = now - movingRenderStart.Value;
            var frame = (int)Math.Floor(timeDifference.TotalMilliseconds / FRAME_TIME) % ANIM_ROWS;

            return isBlinking(gameTime) ? frame + ANIM_ROWS : frame;
        }

        public void Render(GameTime gameTime) {
            if(!OnScreen()) {
                return;
            }

            var tileSize = Owner.TileSize;

            var destination = GetDestination();
            var source = new Rectangle();

            source.X = getRenderColumn() * tileSize;
            source.Y = getRenderRow(gameTime) * tileSize;
            source.Width = tileSize;
            source.Height = tileSize;

            var depth = 1 - Math.Max(destination.Y / (float)Owner.Viewport.Height,0);
            Game.SpriteBatch.Draw(playerTexure,destination,source,Color.White,0f,Vector2.Zero,SpriteEffects.None,depth);
        }
    }
}
