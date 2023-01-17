using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TwelveEngine;

namespace Elves.Scenes.Battle.Sprite.Animation {
    public sealed class FrameController {

        private readonly BattleSprite sprite;

        private readonly int frameWidth;
        private readonly int frameHeight;

        public int Width => frameWidth;
        public int Height => frameHeight;

        private readonly FrameSet defaultFrameSet;
        private readonly Dictionary<int,FrameSet> frameSets = new();

        public FrameController(BattleSprite battleSprite,FrameSet[] frameSets,int baseHeight) {
            frameSets ??= Array.Empty<FrameSet>();
            sprite = battleSprite;
            foreach(FrameSet frameSet in frameSets) {
                this.frameSets[frameSet.ID] = frameSet;
            }

            bool hasStaticFrameSet = this.frameSets.TryGetValue((int)AnimationType.Static,out FrameSet staticFrameSet);
            bool hasIdleFrameSet = this.frameSets.TryGetValue((int)AnimationType.Idle,out FrameSet idleFrameSet);

            Rectangle defaultFrameArea;
            if(!hasStaticFrameSet && !hasIdleFrameSet) {
                defaultFrameArea = new Rectangle(0,0,baseHeight,baseHeight);
                defaultFrameSet = FrameSet.CreateStatic(AnimationType.Static,defaultFrameArea);
                Logger.WriteLine($"Battle sprite for {sprite.Name} is missing their frame sets! Resorting to an (ugly) fallback!");
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

        private FrameSet currentFrameSet;
        private TimeSpan animationStart = TimeSpan.Zero;

        private AnimationMode AnimationMode => currentFrameSet.AnimationMode;

        private int FrameCount => currentFrameSet.FrameCount;

        private float GetAnimationProgress(TimeSpan now) {
            /* Thankfully, it seems to be safe to divide by TimeSpan.Zero */
            return (float)((now - animationStart) / currentFrameSet.AnimationLength);
        }

        private int GetFrameIndexRepeating(float t) {
            return (int)(t % 1 * FrameCount);
        }

        private Rectangle GetAnimatedFrame(float t) {
            int frame = GetFrameIndexRepeating(t);
            return currentFrameSet.Frames[frame];
        }

        private Rectangle GetStaticFrame() {
            return currentFrameSet.Frames[0];
        }

        private bool OnFirstFrame(float t) {
            return GetFrameIndexRepeating(t) == 0;
        }

        private Rectangle GetSpriteArea(float t) {
            if(FrameCount <= 0) {
                return Rectangle.Empty;
            }
            return AnimationMode switch {
                AnimationMode.Once => GetAnimatedFrame(t),
                AnimationMode.Twice => GetAnimatedFrame(t),
                AnimationMode.StaticLoop => GetAnimatedFrame(t),
                AnimationMode.Loop => GetAnimatedFrame(t),
                AnimationMode.Static => GetStaticFrame(),
                _ => GetStaticFrame(),
            };
        }

        private FrameSet? pendingFrameSet = null;

        private bool HasPendingFrameSet => pendingFrameSet.HasValue;

        public void Update(TimeSpan now) {
            if(pendingFrameSet.HasValue && pendingFrameSet.Value.ID == currentFrameSet.ID) {
                pendingFrameSet = null;
            }
            float t = GetAnimationProgress(now);
            Rectangle spriteArea = GetSpriteArea(t);
            if(TryAdvanceFrameSet(t)) {
                /* Reset so we don't show the first animation frame for one game frame */
                animationStart = now;
                t = GetAnimationProgress(now);
                spriteArea = GetSpriteArea(t);
            }
            sprite.SetUVArea(spriteArea);
        }

        private void ApplyPendingFrameSet() {
            FrameSet newFrame = pendingFrameSet ?? defaultFrameSet;
            pendingFrameSet = null;
            currentFrameSet = newFrame;
        }

        private bool TryAdvanceFrameSet(float t) {
            switch(currentFrameSet.AnimationMode) {
                default:
                case AnimationMode.Static:
                case AnimationMode.StaticLoop:
                    if(HasPendingFrameSet) {
                        ApplyPendingFrameSet();
                        return true;
                    }
                    break;
                case AnimationMode.Loop:
                    if(HasPendingFrameSet && OnFirstFrame(t) && t >= 1) {
                        ApplyPendingFrameSet();
                        return true;
                    }
                    break;
                case AnimationMode.Twice:
                    if(t >= 2) {
                        ApplyPendingFrameSet();
                        return true;
                    }
                    break;
                case AnimationMode.Once:
                    if(t >= 1) {
                        ApplyPendingFrameSet();
                        return true;
                    }
                    break;
            }
            return false;
        }

        public void SetAnimation(TimeSpan now,int animationTypeID) {
            if(!frameSets.TryGetValue(animationTypeID,out FrameSet frameSet)){
                frameSet = defaultFrameSet;
                Logger.WriteLine($"Missing animation for animation type ID {animationTypeID}.");
            }
            pendingFrameSet = frameSet;
            Update(now);
        }

        public void SetDefaultAnimation(TimeSpan now) {
            pendingFrameSet = defaultFrameSet;
            Update(now);
        }

        public void SetAnimation(TimeSpan now,AnimationType animationType) {
            SetAnimation(now,(int)animationType);
        }
    }
}
