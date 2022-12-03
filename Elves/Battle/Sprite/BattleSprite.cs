using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Game3D.Entity.Types;
using Elves.Battle.Sprite.Animation;

namespace Elves.Battle.Sprite {
    public class BattleSprite:TextureEntity {

        private readonly FrameController frameController;
        private readonly PositionController positionController;

        public Color AccentColor { get; set; } = Color.White;
        public float XOffset { get; set; } = 0f;

        public void SetSpritePosition(SpritePosition spritePosition,Action callback) {
            positionController.SetSpritePosition(spritePosition,callback);
        }

        public BattleSprite(string textureName,FrameSet[] frameSets,int baseHeight) :base(textureName) {

            frameController = new FrameController(this,frameSets,baseHeight);
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
