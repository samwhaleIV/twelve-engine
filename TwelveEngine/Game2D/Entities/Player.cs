using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using TwelveEngine.Input;

namespace TwelveEngine.Game2D.Entities {
    public sealed class Player:Entity, IUpdateable, IRenderable {
        /* Hitbox coordinate system requires a symmetrical, center aligned hitbox! */

        private const float VERTICAL_HITBOX_X = 1 / 16f;
        private const float VERTICAL_HITBOX_WIDTH = 14 / 16f;

        private const float HORIZONTAL_HITBOX_X = 2 / 16f;
        private const float HORIZONTAL_HITBOX_WIDTH = 12 / 16f;

        private const float HITBOX_ORIENTATION_OFFSET = HORIZONTAL_HITBOX_X - VERTICAL_HITBOX_X;

        private float maxSpeed = Constants.DefaultPlayerSpeed;
        private const float animationFrameTime = 300;

        private const float blinkRate = 2900;
        private const float blinkDuration = 200;

        private const float frameJumpStart = 0.5f;

        public Hitbox GetHitbox() {
            // If it ever is needed, make hitbox math scalable with width and height properties
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

        private Texture2D playerTexure;
        private int animationRows = 4;

        private void keyHandler(KeyboardState ks) {
            xDelta = 0;
            yDelta = 0;
            if(Game.IsKeyDown(KeyBind.Up,ks)) yDelta--;
            if(Game.IsKeyDown(KeyBind.Down,ks)) yDelta++;
            if(Game.IsKeyDown(KeyBind.Left,ks)) xDelta--;
            if(Game.IsKeyDown(KeyBind.Right,ks)) xDelta++;
        }

        private KeyWatcher interactionKeyWatcher;

        private void interact() {
            if(Grid.InteractionLayer == null) {
                return;
            }
            Grid.InteractionLayer.HitTest(this);
        }

        public override void Load() {
            FactoryID = "Player";
            playerTexure = Game.Content.Load<Texture2D>(Constants.PlayerImage);
            interactionKeyWatcher = new KeyWatcher(Game.KeyBinds.Interact,interact);
    }

        public float Speed {
            get {
                return maxSpeed;
            }
            set {
                maxSpeed = value;
            }
        }

        private int xDelta = 0;
        private int yDelta = 0;

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

            var collisionData = Grid.CollisionInterface.Collides(self);
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

        private TimeSpan? movingStart = null;
        private TimeSpan? deaccelerationStart = null;

        private float deaccelerationStartValue;

        private float getSpeed(TimeSpan totalTime) {
            float t;
            if(movingStart.HasValue) {
                t = (float)Math.Min((totalTime - movingStart.Value).TotalMilliseconds / Constants.PlayerAccel,1);
                return maxSpeed * t;
            } else if(deaccelerationStart.HasValue) {
                t = (float)Math.Min((totalTime - deaccelerationStart.Value).TotalMilliseconds / Constants.PlayerDeaccel,1);
                return deaccelerationStartValue * (1 - t);
            } else {
                return 0f;
            }
        }

        private int deaccelerationDeltaX;
        private int deaccelerationDeltaY;

        private int lastDeltaX = 0;
        private int lastDeltaY = 0;

        private void handleAutomaticOffset(Direction oldDirection) {
            Hitbox hitbox = GetHitbox();

            List<Hitbox> collisionResult = Grid.CollisionInterface.Collides(hitbox);

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
                this.X -= HITBOX_ORIENTATION_OFFSET;
            } else {
                this.X += HITBOX_ORIENTATION_OFFSET;
            }

            hitbox = GetHitbox();

            collisionResult = Grid.CollisionInterface.Collides(hitbox);
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
                if(deaccelerationStart.HasValue) {
                    var timeDifference = deaccelerationStart.Value - totalTime;
                    if(timeDifference.TotalMilliseconds >= Constants.PlayerDeaccel) {
                        deaccelerationStart = null;
                    }
                } else if(movingStart.HasValue) {
                    deaccelerationStartValue = getSpeed(totalTime);
                    deaccelerationDeltaX = lastDeltaX;
                    deaccelerationDeltaY = lastDeltaY;
                    deaccelerationStart = totalTime;
                    movingStart = null;
                }
            } else if(!movingStart.HasValue) {
                deaccelerationStart = null;
                movingStart = totalTime;
            }
            lastDeltaX = xDelta;
            lastDeltaY = yDelta;
        }

        private void handleDeaccelerationDistance(float distance) {
            var oldXDelta = xDelta;
            var oldYDelta = yDelta;

            xDelta = deaccelerationDeltaX;
            yDelta = deaccelerationDeltaY;

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
            var ks = Game.KeyboardState;
            keyHandler(ks);
            updateMovement(gameTime);
            interactionKeyWatcher.Process(ks,gameTime);
            var camera = Grid.Camera;
            camera.X = X; camera.Y = Y;
        }

        private int getRenderColumn() {
            return (int)Direction;
        }

        private TimeSpan lastBlink;
        private bool isBlinking(GameTime gameTime) {
            var currentTime = gameTime.TotalGameTime;
            var interval = currentTime - lastBlink;

            var duration = interval.TotalMilliseconds;

            if(duration > blinkRate) {
                if(duration > blinkRate + blinkDuration) {
                    lastBlink = currentTime;
                    return false;
                } else {
                    return true;
                }
            } else {
                return false;
            }
        }

        private TimeSpan? movingRenderStart;

        private int getRenderRow(GameTime gameTime) {

            if(xDelta == 0 && yDelta == 0) {
                return isBlinking(gameTime) ? animationRows : 0;
            }

            var now = gameTime.TotalGameTime;

            if(!movingRenderStart.HasValue) {
                movingRenderStart = now - TimeSpan.FromMilliseconds(animationFrameTime * frameJumpStart);
            }

            var timeDifference = now - movingRenderStart.Value;
            var frame = (int)Math.Floor(timeDifference.TotalMilliseconds / animationFrameTime) % animationRows;

            return isBlinking(gameTime) ? frame + animationRows : frame;
        }

        public void Render(GameTime gameTime) {

            if(!Grid.OnScreen(this)) {
                return;
            }

            var tileSize = Grid.TileSize;

            var destination = Grid.GetDestination(this);
            var source = new Rectangle();

            source.X = getRenderColumn() * tileSize;
            source.Y = getRenderRow(gameTime) * tileSize;
            source.Width = tileSize;
            source.Height = tileSize;

            var depth = 1 - Math.Max(destination.Y / (float)Grid.Viewport.Height,0);
            Game.SpriteBatch.Draw(playerTexure,destination,source,Color.White,0f,Vector2.Zero,SpriteEffects.None,depth);
        }
    }
}
