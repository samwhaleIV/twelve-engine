using Elves.UI.Battle;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TwelveEngine;

namespace Elves.Battle.Sprite {
    public sealed class PositionController:IBattleUIAnimated {

        private const float SCREEN_EDGE_MARGIN = 0.015f;

        private readonly AnimationInterpolator interpolator = new AnimationInterpolator(TimeSpan.FromSeconds(0.1f));

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

        bool IBattleUIAnimated.GetAnimationCompleted() {
            return interpolator.IsFinished;
        }
    }
}
