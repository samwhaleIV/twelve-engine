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

        private const float HITBOX_X = 1 / 16f;
        private const float HITBOX_WIDTH = 14 / 16f;
        private const float HITBOX_HEIGHT = 1;
        private const float HITBOX_Y = 0;

        // TODO (if it ever is needed) Make hitbox math scalable with width and height properties

        public Hitbox GetHitbox() {
            var hitbox = new Hitbox();
            hitbox.X = X + HITBOX_X;
            hitbox.Y = Y + HITBOX_Y;
            hitbox.Width = HITBOX_WIDTH;
            hitbox.Height = HITBOX_HEIGHT;
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

        private double tilesPerSecond = 2.5;
        private double animationFrameTime = 300;

        private double blinkRate = 2900;
        private double blinkDuration = 200;

        private double frameJumpStart = 0.5;

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

        private Direction? pendingDirection = null;
        private void setDirection(Direction newDirection) {
            pendingDirection = newDirection;
        }
        private void finalizeDirection() {
            if(!pendingDirection.HasValue) {
                return;
            }
            var newDirection = pendingDirection.Value;
            if(newDirection != direction) {
                movingStart = null;
                direction = newDirection;
            }
            pendingDirection = null;
        }

        private float getLeftLimit(Hitbox target) {
            return target.X + target.Width - HITBOX_X;
        }
        private float getUpLimit(Hitbox target) {
            return target.Y + target.Height - HITBOX_Y;
        }
        private float getRightLimit(Hitbox target) {
            return target.X - HITBOX_WIDTH - HITBOX_X;
        }
        private float getDownLimit(Hitbox target) {
            return target.Y - HITBOX_HEIGHT - HITBOX_Y;
        }

        private bool resolveLeftCollision(Hitbox target) {
            if(Y == getUpLimit(target) || Y == getDownLimit(target)) {
                return false;
            }
            X = getLeftLimit(target);
            return true;
        }
        private bool resolveUpCollision(Hitbox target) {
            if(X == getLeftLimit(target) || X == getRightLimit(target)) {
                return false;
            }
            Y = getUpLimit(target);
            return true;
        }
        private bool resolveRightCollision(Hitbox target) {
            if(Y == getUpLimit(target) || Y == getDownLimit(target)) {
                return false;
            }
            X = getRightLimit(target);
            return true;
        }
        private bool resolveDownCollision(Hitbox target) {
            if(X == getLeftLimit(target) || X == getRightLimit(target)) {
                return false;
            }
            Y = getDownLimit(target);
            return true;
        }

        private void move(Direction direction,float distance) {

            switch(direction) {
                case Direction.Down: this.Y += distance; break;
                case Direction.Up: this.Y -= distance; break;
                case Direction.Left: this.X -= distance; break;
                case Direction.Right: this.X += distance; break;
            }

            Hitbox hitbox = GetHitbox();

            var collisionData = Grid.CollisionInterface.Collides(hitbox);
            if(collisionData.Count == 0) {
                return;
            }
            foreach(var target in collisionData) {
                switch(direction) {
                    case Direction.Down:
                        if(resolveDownCollision(target)) {
                            return;
                        }
                        break;
                    case Direction.Up:
                        if(resolveUpCollision(target)) {
                            return;
                        }
                        break;
                    case Direction.Left:
                        if(resolveLeftCollision(target)) {
                            return;
                        }
                        break;
                    case Direction.Right:
                        if(resolveRightCollision(target)) {
                            return;
                        }
                        break;
                }
            }
        }
        private void updateMovement(GameTime gameTime) {
            System.Diagnostics.Debug.WriteLine($"X {X} Y {Y}");
            if(xDelta == 0 && yDelta == 0) {
                movingStart = null;
                return;
            }
            double delta = gameTime.ElapsedGameTime.TotalSeconds;
            float distance = (float)(delta * tilesPerSecond);
            if(xDelta != 0) {
                if(xDelta < 0) {
                    setDirection(Direction.Left);
                    move(Direction.Left,distance);
                } else {
                    setDirection(Direction.Right);
                    move(Direction.Right,distance);
                }
            }
            if(yDelta != 0) {
                if(yDelta < 0) {
                    setDirection(Direction.Up);
                    move(Direction.Up,distance);
                } else {
                    setDirection(Direction.Down);
                    move(Direction.Down,distance);
                }
            }
            finalizeDirection();
        }

        public void Update(GameTime gameTime) {
            keyboardHandler.Update();
            updateMovement(gameTime);
            var camera = Grid.Camera;
            camera.X = this.X;
            camera.Y = this.Y;
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
