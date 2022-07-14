using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Game2D.Collision;
using TwelveEngine.Game2D.Entity;
using TwelveEngine.Shell.Input;

namespace Porthole.PuzzleGame {
    internal class Player:TopDownPlayer {

        protected override int GetEntityType() => 1;

        private const float FRAME_TIME = 300, BLINK_RATE = 2900, BLINK_TIME = 200;

        private const float ANIM_JUMP_START = 0.25f;
        private const int ANIM_ROWS = 4;

        public Player() {
            OnUpdate += Player_OnUpdate;
            OnRender += Player_OnRender;
            OnLoad += Player_OnLoad;
            OnUnload += Player_OnUnload;
            OnMovementStarted += Player_OnMovementStarted;
        }

        private TimeSpan animationStartTime = TimeSpan.Zero;
        private TimeSpan lastBlink;

        private void Player_OnMovementStarted(GameTime gameTime) {
            animationStartTime = gameTime.TotalGameTime;
        }

        private Texture2D texture;

        private void Player_OnUnload() {
            Owner.Input.OnAcceptDown -= Input_OnAcceptDown;
        }

        private void Player_OnLoad() {
            texture = Owner.Game.Content.Load<Texture2D>("player");
            Owner.Input.OnAcceptDown += Input_OnAcceptDown;
        }

        private void Input_OnAcceptDown() {
            ((PuzzleGrid)Owner).TestHitTargets(this);
        }

        private void Player_OnRender(GameTime gameTime) {
            if(!OnScreen()) {
                return;
            }
            Draw(texture,getTextureSource(gameTime));
        }

        private void Player_OnUpdate(GameTime gameTime) {
            UpdateMovement(gameTime);
            Owner.Camera.Position = Position;
        }

        protected override Vector2 GetForce() {
            return Owner.Input.GetDelta2D();
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

            if(!IsMoving) {
                return isBlinking(gameTime) ? ANIM_ROWS : 0;
            }

            var now = gameTime.TotalGameTime;

            var timeDifference = now - animationStartTime;
            var frame = (int)Math.Floor(timeDifference.TotalMilliseconds / FRAME_TIME + ANIM_JUMP_START) % ANIM_ROWS;

            return isBlinking(gameTime) ? frame + ANIM_ROWS : frame;
        }

        private Rectangle getTextureSource(GameTime gameTime) {
            var tileSize = Owner.UnitSize;
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
