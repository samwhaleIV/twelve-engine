using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using TwelveEngine.Input;

namespace TwelveEngine.Game2D.Entities {
    public sealed class Player:Entity, IUpdateable, IRenderable {

        public Rectangle getDestination(Hitbox hitbox) {
            var screenSpace = Grid.ScreenSpace;
            var tileSize = screenSpace.TileSize;

            Rectangle destination = new Rectangle();

            destination.X = (int)Math.Round((hitbox.X - screenSpace.X) * tileSize);
            destination.Y = (int)Math.Round((hitbox.Y - screenSpace.Y) * tileSize);

            destination.Width = (int)Math.Floor(hitbox.Width * tileSize);
            destination.Height = (int)Math.Floor(hitbox.Height * tileSize);

            return destination;
        }

        private void renderHitbox() {
            Game.SpriteBatch.Draw(hitboxTexture,getDestination(GetHitbox()),Color.White);
        }

        /* Hitbox coordinate system expects a symmetrical, center aligned hitbox! */

        private const float VERTICAL_HITBOX_X = 1 / 16f;
        private const float VERTICAL_HITBOX_WIDTH = 14 / 16f;

        private const float HORIZONTAL_HITBOX_X = 2 / 16f;
        private const float HORIZONTAL_HITBOX_WIDTH = 12 / 16f;

        private const float HITBOX_ORIENTATION_OFFSET = HORIZONTAL_HITBOX_X - VERTICAL_HITBOX_X;

        private double tilesPerSecond = 2.5;
        private double animationFrameTime = 300;

        private double blinkRate = 2900;
        private double blinkDuration = 200;

        private double frameJumpStart = 0.5;

        public Hitbox GetHitbox() {
            // TODO (if it ever is needed) Make hitbox math scalable with width and height properties
            var hitbox = new Hitbox();

            hitbox.Y = Y;
            hitbox.Height = 1;

            if(direction == Direction.Down || direction == Direction.Up) {
                hitbox.X = X + VERTICAL_HITBOX_X;
                hitbox.Width = VERTICAL_HITBOX_WIDTH;
            } else {
                hitbox.X = X + HORIZONTAL_HITBOX_X;
                hitbox.Width = HORIZONTAL_HITBOX_WIDTH;
            }

            return hitbox;
        }

        private readonly KeyboardHandler keyboardHandler;
        public Player() {
            keyboardHandler = new KeyboardHandler() {
                KeyDown = KeyDown,
                KeyUp = KeyUp
            };
        }

        private Direction direction = Direction.Down;
        public Direction Direcetion {
            get {
                return direction;
            }
            set {
                direction = value;
            }
        }

        private Texture2D playerTexure;
        private int animationRows = 4;

        private Texture2D hitboxTexture;

        public override void Load() {
            playerTexure = Game.Content.Load<Texture2D>(Constants.PlayerImage);
            hitboxTexture = new Texture2D(Game.GraphicsDevice,1,1);
            hitboxTexture.SetData(new Color[]{ Color.Red });
        }
        public override void Unload() {
            playerTexure.Dispose();
            hitboxTexture.Dispose();
        }

        public float Speed {
            get {
                return (float)tilesPerSecond;
            }
            set {
                tilesPerSecond = value;
            }
        }

        private int xDelta = 0;
        private int yDelta = 0;

        private void KeyDown(Keys key) {
            switch(key) {
                case Keys.W: yDelta--; break;
                case Keys.S: yDelta++; break;
                case Keys.A: xDelta--; break;
                case Keys.D: xDelta++; break;
            }
        }
        private void KeyUp(Keys key) {
            switch(key) {
                case Keys.W: yDelta++; break;
                case Keys.S: yDelta--; break;
                case Keys.A: xDelta++; break;
                case Keys.D: xDelta--; break;
            }
        }

        private float getLeftLimit(Hitbox self,Hitbox target) {
            return target.X + target.Width - (self.X - this.X);
        }
        private float getUpLimit(Hitbox self,Hitbox target) {
            return target.Y + target.Height - (self.Y - this.Y);
        }
        private float getRightLimit(Hitbox self,Hitbox target) {
            return target.X - self.Width - (self.X - this.X);
        }
        private float getDownLimit(Hitbox self,Hitbox target) {
            return target.Y - self.Height - (self.Y - this.Y);
        }

        private bool leftCollision(Hitbox self,Hitbox target) {
            if(Y == getUpLimit(self,target) || Y == getDownLimit(self,target)) {
                return false;
            }
            X = getLeftLimit(self,target);
            return true;
        }
        private bool upCollision(Hitbox self,Hitbox target) {
            if(X == getLeftLimit(self,target) || X == getRightLimit(self,target)) {
                return false;
            }
            Y = getUpLimit(self,target);
            return true;
        }
        private bool rightCollision(Hitbox self,Hitbox target) {
            if(Y == getUpLimit(self,target) || Y == getDownLimit(self,target)) {
                return false;
            }
            X = getRightLimit(self,target);
            return true;
        }
        private bool downCollision(Hitbox self,Hitbox target) {
            if(X == getLeftLimit(self,target) || X == getRightLimit(self,target)) {
                return false;
            }
            Y = getDownLimit(self,target);
            return true;
        }

        private void move(Direction direction,float distance) {

            switch(direction) {
                case Direction.Down: this.Y += distance; break;
                case Direction.Up: this.Y -= distance; break;
                case Direction.Left: this.X -= distance; break;
                case Direction.Right: this.X += distance; break;
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
            Direction direction = this.direction;
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

        private void updateMovement(GameTime gameTime) {
            if(xDelta == 0 && yDelta == 0) {
                movingStart = null;
                return;
            }
            double delta = gameTime.ElapsedGameTime.TotalSeconds;
            float distance = (float)(delta * tilesPerSecond);


            var newDirection = getDirection();
            if(newDirection != direction && isHorizontal(direction) && isVertical(newDirection)) {
                float difference = HITBOX_ORIENTATION_OFFSET;
                if(direction == Direction.Right) {
                    this.X -= difference;
                } else {
                    this.X += difference;
                }

            }
            direction = newDirection;

            handleDeltas(distance);
        }

        public void Update(GameTime gameTime) {
            keyboardHandler.Update();
            updateMovement(gameTime);
            var camera = Grid.Camera;
            camera.X = this.X; camera.Y = this.Y;
        }

        private int getRenderColumn() {
            return (int)direction;
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

        private TimeSpan? movingStart;

        private int getRenderRow(GameTime gameTime) {

            if(xDelta == 0 && yDelta == 0) {
                return isBlinking(gameTime) ? animationRows : 0;
            }

            var now = gameTime.TotalGameTime;

            if(!movingStart.HasValue) {
                movingStart = now - TimeSpan.FromMilliseconds(animationFrameTime * frameJumpStart);
            }

            var timeDifference = now - movingStart.Value;
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

            //renderHitbox();
        }
    }
}
