using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game2D.Entity.Types {
    public sealed class Player:MovingEntity2D, IUpdateable, IRenderable {

        public Player() {
            MaxSpeed = Constants.Config.PlayerSpeed;
            AccelRate = Constants.Config.PlayerAccel;
            DeaccelRate = Constants.Config.PlayerDeaccel;

            OrientationOffset = ORIENTATION_OFFSET;

            OnLoad += Player_OnLoad;
            OnUnload += Player_OnUnload;

            OnMovementStopped += () => movingRenderStart = null;
        }

        private const float FRAME_TIME = 300, BLINK_RATE = 2900, BLINK_TIME = 200;

        /* Notice: Hitbox math expects a symmetrical, entity-center aligned hitbox */
        private const float VERTICAL_HITBOX_X = 1 / 16f;
        private const float VERTICAL_HITBOX_WIDTH = 14 / 16f;

        private const float HORIZONTAL_HITBOX_X = 2 / 16f;
        private const float HORIZONTAL_HITBOX_WIDTH = 12 / 16f;

        private const float ORIENTATION_OFFSET = HORIZONTAL_HITBOX_X - VERTICAL_HITBOX_X;

        protected override int GetEntityType() => Entity2DType.Player;

        private void Player_OnLoad() {
            playerTexure = Game.Content.Load<Texture2D>(Constants.Config.PlayerImage);
            Game.ImpulseHandler.OnAcceptDown += QueueInteraction;
        }

        public void Player_OnUnload() {
            Game.ImpulseHandler.OnAcceptDown -= QueueInteraction;
        }

        private const float ANIM_JUMP_START = 0.5f;
        private const int ANIM_ROWS = 4;

        private TimeSpan lastBlink;
        private TimeSpan? movingRenderStart = null;

        private Texture2D playerTexure;

        protected override Hitbox GetHitbox() {
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

        protected override Point GetMovementDelta() => Game.ImpulseHandler.GetDirectionDelta();

        public override void Update(GameTime gameTime) {
            UpdateMovement(gameTime);
            var camera = Owner.Camera;
            camera.X = X; camera.Y = Y;
        }

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

        private int getRenderColumn() => (int)Direction;

        private int getRenderRow(GameTime gameTime) {

            if(MovementDelta == Point.Zero) {
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

        private Rectangle getTextureSource(GameTime gameTime) {
            var tileSize = Owner.TileSize;
            var source = new Rectangle() {
                X = getRenderColumn() * tileSize,
                Y = getRenderRow(gameTime) * tileSize,
                Width = tileSize,
                Height = tileSize
            };
            return source;
        }

        public void Render(GameTime gameTime) {
            if(!OnScreen()) {
                return;
            }
            Draw(playerTexure,getTextureSource(gameTime));
        }
    }
}
