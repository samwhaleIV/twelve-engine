using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using TwelveEngine.Input;

namespace TwelveEngine.Game2D.Entities {
    public sealed class Player:Entity, IUpdateable, IRenderable {

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

        public override void Load() {
            playerTexure = Game.Content.Load<Texture2D>(Constants.PlayerImage);
        }
        public override void Unload() {
            playerTexure.Dispose();
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

        private void updateMovement(GameTime gameTime) {
            var timeDelta = gameTime.ElapsedGameTime.TotalSeconds;
            if(xDelta == 0 && yDelta == 0) {
                movingStart = null;
                return;
            }
            if(xDelta != 0) {
                if(xDelta < 0) {
                    setDirection(Direction.Left);
                } else {
                    setDirection(Direction.Right);
                }
                this.X += (float)(timeDelta * tilesPerSecond) * xDelta;
            }
            if(yDelta != 0) {
                if(yDelta < 0) {
                    setDirection(Direction.Up);
                } else {
                    setDirection(Direction.Down);
                }
                this.Y += (float)(timeDelta * tilesPerSecond) * yDelta;
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
        }
    }
}
