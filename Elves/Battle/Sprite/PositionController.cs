using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Elves.Battle.Sprite {
    public sealed class PositionController {

        private const float SCREEN_EDGE_MARGIN = 0.015f;
        private const double POSITION_CHANGE_DURATION = 0.1f;

        private static readonly Dictionary<SpritePosition,Vector3> positionTable = new Dictionary<SpritePosition,Vector3>() {
            {SpritePosition.Left,new Vector3(-0.4f,0f,DepthConstants.MiddleFar)},
            {SpritePosition.Right,new Vector3(0.4f,0f,DepthConstants.MiddleClose)},
            {SpritePosition.Center,new Vector3(-0f,0f,DepthConstants.Middle)},
            {SpritePosition.CenterLeft,new Vector3(-0.2f,0f,DepthConstants.MiddleFar)},
            {SpritePosition.CenterRight,new Vector3(0.2f,0f,DepthConstants.MiddleClose)},
        };

        private readonly BattleSprite sprite;
        public PositionController(BattleSprite battleSprite) {
            sprite = battleSprite;
        }

        private Vector3 GetPosition(SpritePosition spritePosition,float screenWidth) {
            var position = positionTable[spritePosition];
            if(spritePosition != SpritePosition.Center) {
                position.X = position.X + sprite.XOffset * Math.Sign(position.X);
            }
            float aspectRatio = sprite.Owner.AspectRatio;
            if(aspectRatio < 1) {
                position.X = position.X * screenWidth;
            } else {
                position.X = position.X * screenWidth / aspectRatio;
            }
            return position;
        }

        private TimeSpan positionChangeStart = TimeSpan.Zero;
        private SpritePosition oldPosition = SpritePosition.Center;
        private SpritePosition spritePosition = SpritePosition.Center;

        private Action updatePositionCallback;
        private bool positionAnimating = false;

        private void EndSetSpritePosition() {
            positionAnimating = false;
            positionChangeStart = TimeSpan.Zero;
            var callback = updatePositionCallback;
            updatePositionCallback = null;
            callback?.Invoke();
        }

        public void SetSpritePosition(SpritePosition spritePosition,Action callback) {
            if(positionAnimating) {
                return;
            }
            positionAnimating = true;
            oldPosition = this.spritePosition;
            this.spritePosition = spritePosition;
            positionChangeStart = sprite.Game.Time.TotalGameTime;
            updatePositionCallback = callback;
        }

        public void UpdateScreenPosition(GameTime gameTime) {
            Vector2 screenSize = sprite.Owner.Camera.OrthographicArea.Size;
            float scale = screenSize.Y;
            sprite.Scale = new Vector3(scale,scale,0) * (1f - SCREEN_EDGE_MARGIN);
            if(!positionAnimating) {
                sprite.Position = GetPosition(spritePosition,screenSize.X);
                return;
            } else {
                Vector3 startPosition = GetPosition(oldPosition,screenSize.X);
                Vector3 newPosition = GetPosition(spritePosition,screenSize.X);
                float positionT = (float)((gameTime.TotalGameTime - positionChangeStart).TotalSeconds / POSITION_CHANGE_DURATION);
                bool endSpritePosition = false;
                if(positionT < 0f) {
                    positionT = 0f;
                } else if(positionT >= 1f) {
                    positionT = 1f;
                    endSpritePosition = true;
                }
                sprite.Position = Vector3.Lerp(startPosition,newPosition,positionT);
                if(endSpritePosition) {
                    EndSetSpritePosition();
                }
            }
        }
    }
}
