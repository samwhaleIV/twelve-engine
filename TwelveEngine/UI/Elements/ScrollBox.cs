using Microsoft.Xna.Framework;
using System;
using TwelveEngine.Input;

namespace TwelveEngine.UI.Elements {
    public sealed class ScrollBox:RenderElement {

        private const int SCROLL_AMOUNT = 60;
        private static readonly TimeSpan SMOOTH_TIME = TimeSpan.FromMilliseconds(60);

        private readonly Element scrollFrame;

        public ScrollBox() {
            IsScrollable = true;
            OnScroll += ScrollBox_OnScroll;

            scrollFrame = new Element() {
                Sizing = Sizing.Fill
            };

            AddChild(scrollFrame);
        }
        public Element Target => scrollFrame;

        private ScrollMode scrollMode = ScrollMode.Y;

        private readonly struct AnimationData {
            public AnimationData(int startX,int startY,int xDifference,int yDifference) {
                StartX = startX;
                StartY = startY;

                XDifference = xDifference;
                YDifference = yDifference;
            }

            public readonly int StartX;
            public readonly int StartY;

            public readonly int XDifference;
            public readonly int YDifference;

            public void Apply(Element element) {
                element.PauseLayout();
                element.X = StartX + XDifference;
                element.Y = StartY + YDifference;
                element.StartLayout();
            }

            public void Apply(Element element,float t) {
                element.PauseLayout();
                element.X = StartX + (int)(t * XDifference);
                element.Y = StartY + (int)(t * YDifference);
                element.StartLayout();
            }
        }

        private bool hasPendingAnimationData = false;
        private AnimationData pendingAnimationData;

        private bool hasAnimationData = false;
        private AnimationData animationData;

        private TimeSpan animationStart;

        private void cancelScrollAnimation() {

            if(hasPendingAnimationData) {
                pendingAnimationData.Apply(scrollFrame);

            } else if(hasAnimationData) {
                animationData.Apply(scrollFrame);
            }

            hasPendingAnimationData = false;
            hasAnimationData = false;
        }

        private void sendScrollY(int difference) {
            var area = scrollFrame.Area;
            pendingAnimationData = new AnimationData(area.X,area.Y,0,difference);
            hasPendingAnimationData = true;
        }

        private void ScrollBox_OnScroll(ScrollDirection direction) {
            cancelScrollAnimation();
            switch(scrollMode) {
                //TODO: Implement other cases
                default:
                case ScrollMode.Y: {
                    sendScrollY((int)direction * SCROLL_AMOUNT);
                    break;
                }
            }
        }

        public override void Render(GameTime gameTime) {
            if(hasPendingAnimationData) {
                animationData = pendingAnimationData;

                animationStart = gameTime.TotalGameTime;
                hasPendingAnimationData = false;
                hasAnimationData = true;
            }
            if(!hasAnimationData) {
                return;
            }
            var t = (float)((gameTime.TotalGameTime - animationStart) / SMOOTH_TIME);

            if(t >= 1f) {
                cancelScrollAnimation();
                return;
            }
            animationData.Apply(scrollFrame,t);
        }

        public ScrollMode ScrollMode {
            get => scrollMode;
            set => scrollMode = value;
        }
    }
}
