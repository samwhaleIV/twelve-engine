using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TwelveEngine;
using TwelveEngine.Shell;

namespace Elves.Battle.Sprite.Animation {
    public sealed class FrameController {

        public bool CompleteLoopBeforeDeqeueue { get; set; } = false;

        private readonly BattleSprite sprite;

        private readonly int frameWidth;
        private readonly int frameHeight;

        public int Width => frameWidth;
        public int Height => frameHeight;

        private readonly FrameSet defaultFrameSet;
        private readonly Dictionary<int,FrameSet> frameSets = new Dictionary<int,FrameSet>();

        public FrameController(BattleSprite battleSprite,FrameSet[] frameSets,int baseHeight) {
            if(frameSets == null) {
                frameSets = new FrameSet[0];
            }
            sprite = battleSprite;
            foreach(FrameSet frameSet in frameSets) {
                this.frameSets[frameSet.ID] = frameSet;
            }

            FrameSet staticFrameSet, idleFrameSet;
            bool hasStaticFrameSet = this.frameSets.TryGetValue((int)AnimationType.Static,out staticFrameSet);
            bool hasIdleFrameSet = this.frameSets.TryGetValue((int)AnimationType.Idle,out idleFrameSet);

            Rectangle defaultFrameArea;
            if(!hasStaticFrameSet && !hasIdleFrameSet) {
                defaultFrameArea = new Rectangle(0,0,baseHeight,baseHeight);
                defaultFrameSet = FrameSet.CreateStatic(AnimationType.Static,defaultFrameArea);
                Logger.Write("Battle sprite for ");
                Logger.Write(sprite.Name);
                Logger.WriteLine(" is missing their frame sets! Resorting to an (ugly) fallback!");
            } else if(hasStaticFrameSet && hasIdleFrameSet) {
                defaultFrameArea = staticFrameSet.AreaOrDefault;
                defaultFrameSet = idleFrameSet;
            } else if(hasStaticFrameSet) {
                defaultFrameArea = staticFrameSet.AreaOrDefault;
                defaultFrameSet = staticFrameSet;
            } else {
                defaultFrameArea = idleFrameSet.AreaOrDefault;
                defaultFrameSet = idleFrameSet;
            }

            frameWidth = defaultFrameArea.Width;
            frameHeight = defaultFrameArea.Height;

            currentFrameSet = defaultFrameSet;
        }

        public void UpdateUVArea(TimeSpan now) {
            sprite.SetUVArea(GetSpriteArea(now));
        }

        private void ClearAnimation() {
            currentFrameSet = defaultFrameSet;
            animationCallback = null;
            animationStart = TimeSpan.Zero;
        }

        private FrameSet currentFrameSet;
        private Action animationCallback;
        private TimeSpan animationStart = TimeSpan.Zero;
        private readonly Queue<(FrameSet FrameSet, Action Callback)> animationQueue = new Queue<(FrameSet FrameSet, Action Callback)>();

        private Rectangle FilterSpriteAreaLoopCount(TimeSpan now,int loopCount) {
            double t = (now - animationStart) / currentFrameSet.AnimationLength;
            double tReal = t;
            bool atEnd = t >= loopCount;
            t = Math.Max(0,t) % 1;
            /* Hold last frame for extremely bad timing, instead of showing the first animation frame again for one game frame */
            int frameIndex = atEnd ? currentFrameSet.FrameCount - 1 : (int)Math.Floor(currentFrameSet.FrameCount * t);
            /* Hold the last frame, until the next frame is supposed to start */
            int frameIndexReal = (int)Math.Floor(currentFrameSet.FrameCount * tReal);
            if(atEnd && frameIndexReal % currentFrameSet.FrameCount == 0) {
                AdvanceAnimationQueue();
            }
            return currentFrameSet.Frames[frameIndex];
        }

        private Rectangle FilterSpriteAreaLoop(TimeSpan now) {
            /* A loop is immediately terminated, unlike loop count. It is presumed that n-less loops are idle animations. */
            double t = (now - animationStart) / currentFrameSet.AnimationLength;
            t = Math.Max(0,t) % 1;
            int frameIndex = (int)Math.Floor(currentFrameSet.FrameCount * t);
            if(animationQueue.Count > 0) {
                AdvanceAnimationQueue();
            }
            return currentFrameSet.Frames[frameIndex];
        }

        private Rectangle FilterSpriteAreaStatic() {
            if(animationQueue.Count > 0) {
                AdvanceAnimationQueue();
            }
            return currentFrameSet.Frames[0];
        }

        private Rectangle GetSpriteArea(TimeSpan now) {
            if(updateAnimationStart) {
                animationStart = now;
                updateAnimationStart = false;
            }
            return currentFrameSet.AnimationMode switch {
                AnimationMode.PlayOnce => FilterSpriteAreaLoopCount(now,1),
                AnimationMode.PlayTwice => FilterSpriteAreaLoopCount(now,2),
                AnimationMode.Loop => FilterSpriteAreaLoop(now),
                _ => FilterSpriteAreaStatic(),
            };
        }

        private bool updateAnimationStart = false;

        private void AdvanceAnimationQueue() {
            var callback = animationCallback;
            if(animationQueue.TryPeek(out (FrameSet FrameSet, Action Callback) newAnimation)) {
                currentFrameSet = newAnimation.FrameSet;
                callback = newAnimation.Callback;
            } else {
                ClearAnimation();
                updateAnimationStart = true;
            }
            animationCallback?.Invoke();
        }

        public void PushAnimation(int animationTypeID,Action callback = null) {
            FrameSet newFrameSet = frameSets[animationTypeID];
            if(animationQueue.Count < 1) {
                currentFrameSet = newFrameSet;
                animationCallback = callback;
                updateAnimationStart = true;
                return;
            }
            animationQueue.Enqueue((newFrameSet,callback));
        }
        public void PushAnimation(AnimationType animationType,Action callback = null) {
            PushAnimation((int)animationType,callback);
        }
    }
}
