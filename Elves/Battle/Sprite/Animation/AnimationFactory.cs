using Microsoft.Xna.Framework;
using System;

namespace Elves.Battle.Sprite.Animation {
    public static class AnimationFactory {

        private const int BLINK_IDLE_FRAME_COUNT = 10;
        private const float BLINK_DURATION = 1f / 3f;
        private const float DEFAULT_FRAME_LENGTH = 1 / 24f;

        /* Create a static frameset so that the frame controller can set the sprite's size accordingly */
        public static FrameSet CreateStatic(Rectangle frame) {
            return FrameSet.CreateStatic(AnimationType.Static,frame);
        }

        public static FrameSet CreateStatic(int x,int y,int width,int height) {
            return CreateStatic(new Rectangle(x,y,width,height));
        }

        public static FrameSet CreateIdleBlink(Rectangle idleFrame,Rectangle blinkFrame,int idleFrameCount = BLINK_IDLE_FRAME_COUNT,float blinkDuration = BLINK_DURATION) {
            Rectangle[] frames = new Rectangle[idleFrameCount + 1];
            for(int i = 0;i<idleFrameCount;i++) {
                frames[i] = idleFrame;
            }
            frames[frames.Length-1] = blinkFrame;

            return FrameSet.CreateAnimated(AnimationType.Idle,AnimationMode.Loop,TimeSpan.FromSeconds(blinkDuration),frames);
        }

        public static FrameSet CreateIdleBlink(int x1,int y1,int w1,int h1,int x2,int y2,int w2,int h2,int idleFrameCount = BLINK_IDLE_FRAME_COUNT,float blinkDuration = BLINK_DURATION) {
            return CreateIdleBlink(new Rectangle(x1,y1,w1,h1),new Rectangle(x2,y2,w2,h2),idleFrameCount,blinkDuration);
        }

        /* Animations grow horizontally */
        private static FrameSet CreateSlideshow(AnimationType type,AnimationMode mode,Rectangle startFrame,int frameCount,TimeSpan? frameTime = null) {
            Rectangle[] frames = new Rectangle[frameCount];
            for(int i = 0;i<frames.Length;i++) {
                Rectangle newFrame = startFrame;
                newFrame.X = startFrame.X + startFrame.Width * i;
                frames[i] = newFrame;

            }
            return FrameSet.CreateAnimated(type,mode,frameTime ?? TimeSpan.FromSeconds(DEFAULT_FRAME_LENGTH),frames);
        }

        public static FrameSet CreateSlideshowLoop(AnimationType type,Rectangle startFrame,int frameCount,TimeSpan? frameTime = null) {
            return CreateSlideshow(type,AnimationMode.Loop,startFrame,frameCount,frameTime);
        }

        public static FrameSet CreateSlideshowPlayOnce(AnimationType type,Rectangle startFrame,int frameCount,TimeSpan? frameTime = null) {
            return CreateSlideshow(type,AnimationMode.PlayOnce,startFrame,frameCount,frameTime);
        }

        public static FrameSet CreateSlideshowPlayTwice(AnimationType type,Rectangle startFrame,int frameCount,TimeSpan? frameTime = null) {
            return CreateSlideshow(type,AnimationMode.PlayTwice,startFrame,frameCount,frameTime);
        }

        public static FrameSet CreateSlideshowLoop(AnimationType type,int x,int y,int width,int height,int frameCount,TimeSpan? frameTime = null) {
            return CreateSlideshow(type,AnimationMode.Loop,new Rectangle(x,y,width,height),frameCount,frameTime);
        }

        public static FrameSet CreateSlideshowPlayOnce(AnimationType type,int x,int y,int width,int height,int frameCount,TimeSpan? frameTime = null) {
            return CreateSlideshow(type,AnimationMode.PlayOnce,new Rectangle(x,y,width,height),frameCount,frameTime);
        }

        public static FrameSet CreateSlideshowPlayTwice(AnimationType type,int x,int y,int width,int height,int frameCount,TimeSpan? frameTime = null) {
            return CreateSlideshow(type,AnimationMode.PlayTwice,new Rectangle(x,y,width,height),frameCount,frameTime);
        }

    }
}
