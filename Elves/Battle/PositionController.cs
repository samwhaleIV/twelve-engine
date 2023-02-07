using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TwelveEngine;

namespace Elves.Battle {
    public sealed class PositionController {

        private const float SCREEN_EDGE_MARGIN = 0.015f;

        private readonly Interpolator interpolator = new(Constants.BattleUI.TargetMovementDuration);

        private static readonly Dictionary<SpritePosition,Vector3> positionTable = new() {
            {SpritePosition.Left,new Vector3(-0.4f,0f,Constants.Depth.MiddleFar)},
            {SpritePosition.Right,new Vector3(0.4f,0f,Constants.Depth.MiddleClose)},
            {SpritePosition.Center,new Vector3(-0f,0f,Constants.Depth.Middle)},
            {SpritePosition.CenterLeft,new Vector3(-0.2f,0f,Constants.Depth.MiddleFar)},
            {SpritePosition.CenterRight,new Vector3(0.2f,0f,Constants.Depth.MiddleClose)},
        };

        private readonly BattleSprite sprite;
        public PositionController(BattleSprite battleSprite) {
            sprite = battleSprite;
        }

        private Vector3 GetPosition(SpritePosition spritePosition,float screenWidth) {
            var position = positionTable[spritePosition];
            if(spritePosition != SpritePosition.Center) {
                position.X += sprite.XOffset * Math.Sign(position.X);
            }
            float aspectRatio = sprite.Owner.AspectRatio;
            if(aspectRatio < 1) {
                position.X *= screenWidth;
            } else {
                position.X = position.X * screenWidth / aspectRatio;
            }
            return position;
        }

        private SpritePosition oldPosition, currentPosition;

        public void SetSpritePosition(TimeSpan now,SpritePosition position) {
            interpolator.Reset(now);
            oldPosition = currentPosition;
            currentPosition = position;
        }

        public void UpdateScreenPosition(TimeSpan now) {
            interpolator.Update(now);

            Vector2 screenSize = sprite.Owner.Camera.OrthographicArea.Size;
            float scale = screenSize.Y;
            sprite.Scale = new Vector3(scale,scale,0) * (1f - SCREEN_EDGE_MARGIN);

            sprite.Position = interpolator.Interpolate(
                GetPosition(oldPosition,screenSize.X),
                GetPosition(currentPosition,screenSize.X)
            );
        }

        public bool AnimationIsFinished => interpolator.IsFinished;
    }
}
