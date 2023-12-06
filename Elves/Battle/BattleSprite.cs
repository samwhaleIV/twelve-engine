using Microsoft.Xna.Framework;
using TwelveEngine.Game3D.Entity.Types;
using Elves.Animation;
using Microsoft.Xna.Framework.Graphics;
using Elves.ElfData;
using System.Collections.Generic;

namespace Elves.Battle {
    public class BattleSprite:TextureEntity {

        private readonly FrameController frameController;
        private readonly PositionController positionController;

        public UserData UserData { get; private init; }

        public float XOffset { get; set; } = 0f;

        public void SetPosition(SpritePosition position) {
            positionController.SetSpritePosition(Now,position);
        }

        public void SetAnimation(AnimationType animationType) {
            frameController.SetAnimation(Now,animationType);
        }

        public void SetDefaultAnimation() {
            frameController.SetDefaultAnimation(Now);
        }

        public int BaseHeight { get; private init; }

        public BattleSprite(Elf elf):base(elf.Texture) {
            UserData = new(elf);
            BaseHeight = elf.BaseHeight;

            PixelSmoothing = false;

            frameController = new FrameController(this,elf.FrameSets);
            positionController = new PositionController(this);

            OnUpdate += Update;
            OnLoad += Load;
        }

        public BattleSprite(string name,Color color,Texture2D texture,int baseHeight,Dictionary<AnimationType,FrameSet> frameSets):base(texture) {
            UserData = new(name,color);
            BaseHeight = baseHeight;

            PixelSmoothing = false;

            frameController = new FrameController(this,frameSets);
            positionController = new PositionController(this);

            OnUpdate += Update;
            OnLoad += Load;
        }

        public BattleSprite(UserData userData,Texture2D texture,int baseHeight,Dictionary<AnimationType,FrameSet> frameSets) :base(texture) {
            UserData = userData;
            BaseHeight = baseHeight;

            PixelSmoothing = false;

            frameController = new FrameController(this,frameSets);
            positionController = new PositionController(this);

            OnUpdate += Update;
            OnLoad += Load;
        }

        private void Update() {
            positionController.UpdateScreenPosition(Now);
            frameController.Update(Now);
        }

        private void Load() {
            float baseSize = BaseHeight;
            float width = frameController.Width / baseSize, height = frameController.Height / baseSize;
            float halfWidth = width * 0.5f, halfHeight = height * 0.5f;
            Vector3 baseSizeOffset = new(0f,(1f-height)*-0.5f,0f);
            TopLeft = new Vector3(-halfWidth,halfHeight,0f) + baseSizeOffset;
            BottomRight = new Vector3(halfWidth,-halfHeight,0f) + baseSizeOffset;
        }

    }
}
