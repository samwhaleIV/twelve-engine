using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Game3D.Entity.Types;

namespace Elves.Battle.Sprite {
    public class BattleSprite:TextureEntity {

        private readonly FrameController frameController;
        private readonly PositionController positionController;

        public void SetSpritePosition(SpritePosition spritePosition,Action callback) {
            positionController.SetSpritePosition(spritePosition,callback);
        }

        public BattleSprite(string textureName,SpriteFrame[] frames,int baseHeight) :base(textureName) {

            frameController = new FrameController(this,frames,baseHeight);
            positionController = new PositionController(this);

            OnLoad += () => {
                float baseSize = baseHeight;
                float width = frameController.Width / baseSize, height = frameController.Height / baseSize;
                float halfWidth = width * 0.5f, halfHeight = height * 0.5f;
                Vector3 baseSizeOffset = new Vector3(0f,(1f-height)*-0.5f,0f);
                TopLeft = new Vector3(-halfWidth,halfHeight,0f) + baseSizeOffset;
                BottomRight = new Vector3(halfWidth,-halfHeight,0f) + baseSizeOffset;
            };

            OnUpdate += gameTime => {
                positionController.UpdateScreenPosition(gameTime);
                frameController.UpdateUVArea(gameTime);
            };
        }
    }
}
