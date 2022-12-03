using Microsoft.Xna.Framework;
using System;

namespace Elves.Battle.Sprite.Animation {
    public readonly struct FrameSet {

        public readonly int ID;
        public readonly Rectangle[] Frames;

        public readonly int FrameCount => Frames.Length;
        public readonly bool IsAnimated => Frames.Length > 0;

        public readonly TimeSpan FrameLength;
        public readonly TimeSpan AnimationLength;

        public readonly AnimationMode AnimationMode;

        public Rectangle AreaOrDefault => FrameCount <= 0 ? Rectangle.Empty : Frames[0];

        private 
            FrameSet(int ID,Rectangle[] frames,TimeSpan frameLength,AnimationMode animationMode) {
            this.ID = ID;
            Frames = frames;
            FrameLength = frameLength;
            AnimationLength = frames.Length * frameLength;
            AnimationMode = animationMode;
        }

        public static FrameSet CreateStatic(int ID,Rectangle frame) {
            return new FrameSet(ID,new Rectangle[1] { frame },TimeSpan.Zero,AnimationMode.Static);
        }
        public static FrameSet CreateStatic(AnimationType animation,Rectangle frame) {
            return new FrameSet((int)animation,new Rectangle[1] { frame },TimeSpan.Zero,AnimationMode.Static);
        }
        public static FrameSet CreateAnimated(int animationTypeID,AnimationMode animationMode,TimeSpan frameLength,params Rectangle[] frames) {
            return new FrameSet(animationTypeID,frames,frameLength,animationMode);
        }
        public static FrameSet CreateAnimated(AnimationType animation,AnimationMode animationMode,TimeSpan frameLength,params Rectangle[] frames) {
            return new FrameSet((int)animation,frames,frameLength,animationMode);
        }
    }
}
