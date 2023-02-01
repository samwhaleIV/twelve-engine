using Microsoft.Xna.Framework;
using System;

namespace Elves.Animation {
    public readonly struct FrameSet {

        public readonly AnimationType AnimationType;
        public readonly Rectangle[] Frames;

        public readonly int FrameCount => Frames.Length;
        public readonly bool IsAnimated => Frames.Length > 0;

        public readonly TimeSpan FrameLength;
        public readonly TimeSpan AnimationLength;

        public readonly AnimationMode AnimationMode;

        public Rectangle AreaOrDefault => FrameCount <= 0 ? Rectangle.Empty : Frames[0];

        private FrameSet(AnimationType animationType,Rectangle[] frames,TimeSpan frameLength,AnimationMode animationMode) {
            AnimationType = animationType;
            Frames = frames;
            FrameLength = frameLength;
            AnimationLength = frames.Length * frameLength;
            AnimationMode = animationMode;
        }

        private static readonly TimeSpan StaticDuration = TimeSpan.FromSeconds(1);

        public static FrameSet CreateStatic(AnimationType animationType,Rectangle frame) {
            return new FrameSet(animationType,new Rectangle[1] { frame },StaticDuration,AnimationMode.Static);
        }
        public static FrameSet CreateAnimated(AnimationType animationType,AnimationMode animationMode,TimeSpan frameLength,params Rectangle[] frames) {
            return new FrameSet(animationType,frames,frameLength,animationMode);
        }

    }
}
