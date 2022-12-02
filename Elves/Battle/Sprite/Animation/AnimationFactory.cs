using Microsoft.Xna.Framework;
using System;

namespace Elves.Battle.Sprite.Animation {
    public static class AnimationFactory {

        private const int BLINK_IDLE_FRAME_COUNT = 10;
        private const float BLINK_DURATION = 1f / 3f;

        /* Create a static frameset so that the frame controller can set the sprite's size accordingly */
        public static FrameSet CreateStatic(Rectangle frame) {
            return FrameSet.CreateStatic(AnimationType.Static,frame);
        }

        public static FrameSet CreateIdleBlink(Rectangle idleFrame,Rectangle blinkFrame,int idleFrameCount = BLINK_IDLE_FRAME_COUNT,float blinkDuration = BLINK_DURATION) {
            Rectangle[] frames = new Rectangle[idleFrameCount + 1];
            for(int i = 0;i<idleFrameCount;i++) {
                frames[i] = idleFrame;
            }
            frames[frames.Length-1] = blinkFrame;

            return FrameSet.CreateAnimated(AnimationType.Idle,AnimationMode.Loop,TimeSpan.FromSeconds(blinkDuration),frames);
        }
    }
}
