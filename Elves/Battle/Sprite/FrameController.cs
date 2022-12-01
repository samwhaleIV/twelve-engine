using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Elves.Battle.Sprite {
    public sealed class FrameController {

        public bool CompleteLoopBeforeDeqeueue { get; set; } = false;

        private readonly BattleSprite sprite;

        private readonly int frameWidth;
        private readonly int frameHeight;

        public int Width => frameWidth;
        public int Height => frameHeight;

        private readonly SpriteFrame defaultSpriteFrame;
        private readonly Dictionary<int,SpriteFrame> frames = new Dictionary<int,SpriteFrame>();

        public FrameController(BattleSprite battleSprite,SpriteFrame[] frames,int baseHeight) {
            sprite = battleSprite;
            foreach(SpriteFrame frame in frames) {
                this.frames[frame.ID] = frame;
            }
            Rectangle idleArea; /* This is where we get the base sprite size from. Use a larger size and smaller baseHeight to dial your animations in! */
            int idleID = (int)AnimationType.Idle;
            if(this.frames.TryGetValue(idleID,out var idleFrame)) {
                idleArea = idleFrame.Frames[0];
                defaultSpriteFrame = idleFrame;
            } else {
                idleArea = new Rectangle(0,0,baseHeight,baseHeight);
                var spriteFrame = SpriteFrame.CreateStatic(idleID,idleArea);
                defaultSpriteFrame = spriteFrame;
                this.frames[idleID] = spriteFrame;
            }
            frameWidth = idleArea.Width;
            frameHeight = idleArea.Height;
            currentFrame = defaultSpriteFrame;
        }

        private void SetUVArea(Rectangle area) {
            var texture = sprite.Texture;
            sprite.UVTopLeft = new Vector2(
                (float)area.X / texture.Width,
                (float)area.Y / texture.Height
            );
            sprite.UVBottomRight = new Vector2(
                (float)area.Right / texture.Width,
                (float)area.Bottom / texture.Height
            );
        }

        public void UpdateUVArea(GameTime gameTime) {
            SetUVArea(GetSpriteArea(gameTime));
        }

        private void ClearAnimation() {
            currentFrame = defaultSpriteFrame;
            animationCallback = null;
            animationStart = TimeSpan.Zero;
        }

        private SpriteFrame currentFrame;
        private Action animationCallback;
        private TimeSpan animationStart = TimeSpan.Zero;
        private readonly Queue<(SpriteFrame Frame, Action Callback)> animationQueue = new Queue<(SpriteFrame Frame, Action Callback)>();

        private Rectangle FilterSpriteAreaLoopCount(TimeSpan now,int loopCount) {
            double t = (animationStart - now) / currentFrame.AnimationLength;
            double tReal = t;
            bool atEnd = t >= loopCount;
            t = Math.Max(0,t) % 1;
            /* Hold last frame for extremely bad timing, instead of showing the first animation frame again for one game frame */
            int frameIndex = atEnd ? currentFrame.FrameCount - 1 : (int)Math.Floor(currentFrame.FrameCount * t);
            /* Hold the last frame, until the next frame is supposed to start */
            int frameIndexReal = (int)Math.Floor(currentFrame.FrameCount * tReal);
            if(atEnd && frameIndexReal % currentFrame.FrameCount == 0) {
                AdvanceAnimationQueue();
            }
            return currentFrame.Frames[frameIndex];
        }

        private Rectangle FilterSpriteAreaLoop(TimeSpan now) {
            /* A loop is immediately terminated, unlike loop count. It is presumed that n-less loops are idle animations. */
            double t = (animationStart - now) / currentFrame.AnimationLength;
            t = Math.Max(0,t) % 1;
            int frameIndex = (int)Math.Floor(currentFrame.FrameCount * t);
            if(animationQueue.Count > 0) {
                AdvanceAnimationQueue();
            }
            return currentFrame.Frames[frameIndex];
        }

        private Rectangle FilterSpriteAreaStatic() {
            if(animationQueue.Count > 0) {
                AdvanceAnimationQueue();
            }
            return currentFrame.Frames[0];
        }

        private Rectangle GetSpriteArea(GameTime gameTime) {
            TimeSpan now = gameTime.TotalGameTime;
            if(updateAnimationStart) {
                animationStart = now;
                updateAnimationStart = false;
            }
            return currentFrame.AnimationMode switch {
                AnimationMode.PlayOnce => FilterSpriteAreaLoopCount(now,1),
                AnimationMode.PlayTwice => FilterSpriteAreaLoopCount(now,2),
                AnimationMode.Loop => FilterSpriteAreaLoop(now),
                _ => FilterSpriteAreaStatic(),
            };
        }

        private bool updateAnimationStart = false;

        private void AdvanceAnimationQueue() {
            var callback = animationCallback;
            if(animationQueue.TryPeek(out (SpriteFrame Frame, Action Callback) newAnimation)) {
                currentFrame = newAnimation.Frame;
                callback = newAnimation.Callback;
            } else {
                ClearAnimation();
                updateAnimationStart = true;
            }
            animationCallback?.Invoke();
        }

        public void PushAnimation(int animationTypeID,Action callback = null) {
            SpriteFrame newFrame = frames[animationTypeID];
            if(animationQueue.Count < 1) {
                currentFrame = newFrame;
                animationCallback = callback;
                updateAnimationStart = true;
                return;
            }
            animationQueue.Enqueue((newFrame,callback));
        }
        public void PushAnimation(AnimationType animationType,Action callback = null) {
            PushAnimation((int)animationType,callback);
        }
    }
}
