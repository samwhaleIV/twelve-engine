using Microsoft.Xna.Framework;
using TwelveEngine.Game3D.Entity.Types;
using Microsoft.Xna.Framework.Graphics;
using Elves.Scenes.Battle.Sprite.Animation;

namespace Elves.Scenes.Battle.Sprite {
    public class BattleSprite:TextureEntity {

        private readonly FrameController frameController;
        private readonly PositionController positionController;

        private readonly UserData userData = new();
        public UserData UserData => userData;

        public float XOffset { get; set; } = 0f;

        public void SetPosition(SpritePosition position) {
            positionController.SetSpritePosition(Now,position);
        }

        public void SetAnimation(AnimationType animationType) {
            frameController.SetAnimation(Now,animationType);
        }

        public void SetAnimation(int animationType) {
            frameController.SetAnimation(Now,animationType);
        }

        public void SetDefaultAnimation() {
            frameController.SetDefaultAnimation(Now);
        }

        private readonly int baseHeight;

        public BattleSprite(string textureName,int baseHeight,params FrameSet[] frameSets) :base(textureName) {
            this.baseHeight = baseHeight;

            PixelSmoothing = false;

            frameController = new FrameController(this,frameSets,baseHeight);
            positionController = new PositionController(this);

            OnLoad += BattleSprite_OnLoad;
            OnUpdate += BattleSprite_OnUpdate;
        }

        public BattleSprite(Texture2D texture,int baseHeight,params FrameSet[] frameSets) : base(texture) {
            this.baseHeight = baseHeight;

            PixelSmoothing = false;

            frameController = new FrameController(this,frameSets,baseHeight);
            positionController = new PositionController(this);

            OnLoad += BattleSprite_OnLoad;
            OnUpdate += BattleSprite_OnUpdate;
        }

        private void BattleSprite_OnUpdate() {
            positionController.UpdateScreenPosition(Now);
            frameController.Update(Now);
        }

        private void BattleSprite_OnLoad() {
            float baseSize = baseHeight;
            float width = frameController.Width / baseSize, height = frameController.Height / baseSize;
            float halfWidth = width * 0.5f, halfHeight = height * 0.5f;
            Vector3 baseSizeOffset = new(0f,(1f-height)*-0.5f,0f);
            TopLeft = new Vector3(-halfWidth,halfHeight,0f) + baseSizeOffset;
            BottomRight = new Vector3(halfWidth,-halfHeight,0f) + baseSizeOffset;
        }

    }
}
