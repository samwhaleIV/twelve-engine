using Microsoft.Xna.Framework;
using System;

namespace Elves.Battle.Sprite {
    public readonly struct SpriteFrame {

        public readonly int ID;
        public readonly Rectangle[] Frames;

        public readonly int FrameCount => Frames.Length;
        public readonly bool IsAnimated => Frames.Length > 0;

        public readonly TimeSpan FrameLength;
        public readonly TimeSpan AnimationLength;

        public readonly AnimationMode AnimationMode;

        private SpriteFrame(int ID,Rectangle[] frames,TimeSpan frameLength,AnimationMode animationMode) {
            this.ID = ID;
            Frames = frames;
            FrameLength = frameLength;
            AnimationLength = frames.Length * frameLength;
            AnimationMode = animationMode;
        }

        public static SpriteFrame CreateStatic(int ID,Rectangle frame) {
            return new SpriteFrame(ID,new Rectangle[1] { frame },TimeSpan.Zero,AnimationMode.Static);
        }
        public static SpriteFrame CreateStatic(Animation animation,Rectangle frame) {
            return new SpriteFrame((int)animation,new Rectangle[1] { frame },TimeSpan.Zero,AnimationMode.Static);
        }
        public static SpriteFrame CreateAnimated(int ID,AnimationMode animationMode,TimeSpan frameLength,params Rectangle[] frames) {
            return new SpriteFrame(ID,frames,frameLength,animationMode);
        }
        public static SpriteFrame CreateAnimated(Animation animation,AnimationMode animationMode,TimeSpan frameLength,params Rectangle[] frames) {
            return new SpriteFrame((int)animation,frames,frameLength,animationMode);
        }
    }
}
