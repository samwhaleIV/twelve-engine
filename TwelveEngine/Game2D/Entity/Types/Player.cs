using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Shell.Input;

namespace TwelveEngine.Game2D.Entity.Types {
    public sealed class Player:MovingEntity2D {

        private const float FRAME_TIME = 300, BLINK_RATE = 2900, BLINK_TIME = 200;

        private const float VERTICAL_HITBOX_X = 1 / 16f; /* Notice: Hitbox math expects a symmetrical, entity-center aligned hitbox */
        private const float VERTICAL_HITBOX_WIDTH = 14 / 16f;

        private const float HORIZONTAL_HITBOX_X = 2 / 16f;
        private const float HORIZONTAL_HITBOX_WIDTH = 12 / 16f;

        private const float ORIENTATION_OFFSET = HORIZONTAL_HITBOX_X - VERTICAL_HITBOX_X;

        private const float ANIM_JUMP_START = 0.5f;
        private const int ANIM_ROWS = 4;

        private TimeSpan lastBlink;
        private TimeSpan? movingRenderStart = null;

        private Texture2D playerTexure;

        public Player() {
            MaxSpeed = Constants.Config.PlayerSpeed;
            AccelRate = Constants.Config.PlayerAccel;
            DeaccelRate = Constants.Config.PlayerDeaccel;

            OrientationOffset = ORIENTATION_OFFSET;

            OnLoad += Player_OnLoad;
            OnUnload += Player_OnUnload;

            OnMovementStopped += () => movingRenderStart = null;

            OnUpdate += Player_OnUpdate;
            OnRender += Player_OnRender;
        }

        private void Player_OnRender(GameTime gameTime) {
            if(!OnScreen()) {
                return;
            }
            Draw(playerTexure,getTextureSource(gameTime));
        }

        private void Player_OnUpdate(GameTime gameTime) {
            Owner.Camera.Position = Position;
        }

        protected override int GetEntityType() => Entity2DType.Player;

        private void Player_OnLoad() {
            playerTexure = Game.Content.Load<Texture2D>(Constants.Config.PlayerImage);
            Owner.Input.OnAcceptDown += QueueInteraction;
        }

        public void Player_OnUnload() {
            Owner.Input.OnAcceptDown -= QueueInteraction;
        }

        protected override Hitbox GetHitbox() {
            /* Notice: This hitbox does not scale with Entity Width and Height properties */
            var location = new Vector2(0f,Y);
            var size = new Vector2(0,1f);

            if(Direction == Direction.Down || Direction == Direction.Up) {
                location.X = X + VERTICAL_HITBOX_X;
                size.X = VERTICAL_HITBOX_WIDTH;
            } else {
                location.X = X + HORIZONTAL_HITBOX_X;
                size.X = HORIZONTAL_HITBOX_WIDTH;
            }

            return new Hitbox(location,size);
        }

        protected override Point GetMovementDelta() => Owner.Input.GetDelta2D();

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

    }
}
