using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Shell.Input;

namespace TwelveEngine.UI.Elements {
    public sealed class ScrollBox:RenderFrame {

        private const int SCROLL_AMOUNT = 25;
        private static TimeSpan SMOOTH_TIME => TimeSpan.FromMilliseconds(100);

        public ScrollBox(UIState state): base(state) {
            IsScrollable = true;

            OnScroll += ScrollBox_OnScroll;
            OnUpdate += ScrollBox_OnUpdate;

            LayoutUpdated += ScrollBox_LayoutUpdated;
        }

        private void ScrollBox_OnScroll(Point mousePosition,ScrollDirection direction) {
            cancelScrollAnimation();
            switch(scrollMode) {
                default:
                case ScrollMode.Y:
                    sendScrollY(direction);
                    break;
                case ScrollMode.X:
                    throw new NotImplementedException();
                    break;
                case ScrollMode.XY:
                    throw new NotImplementedException();
                    break;
            }
        }

        private int limitY(int value) {
            (int minY, int maxY) = getYLimits();
            if(value < minY) {
                value = minY;
            } else if(value > maxY) {
                value = maxY;
            }
            return value;
        }

        public void ScrollTo(Point position) {
            cancelScrollAnimation();
            Target.PushLayoutFreeze();
            //TODO: Implement X dimension
            if(position.Y != Target.Y) {
                Target.Y = limitY(position.Y);
                InteractionState.RefreshElement();
            }
            Target.PopLayoutFreeze();
        }

        private void applyYLimit() {
            var newY = limitY(Target.Y);
            Target.Y = newY;
            InteractionState.RefreshElement();
        }

        private void ScrollBox_LayoutUpdated() {
            cancelScrollAnimation();
            applyYLimit();
            //TODO: Implement X dimension
        }

        private void ScrollBox_OnUpdate(GameTime gameTime) {
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
            animationData.Apply(Target,t);
            InteractionState.RefreshElement();
        }

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

            public void Apply(UIElement element) {
                element.PushLayoutFreeze();
                element.X = StartX + XDifference;
                element.Y = StartY + YDifference;
                element.PopLayoutFreeze();
            }

            public void Apply(UIElement element,float t) {
                element.PushLayoutFreeze();
                element.X = StartX + (int)(t * XDifference);
                element.Y = StartY + (int)(t * YDifference);
                element.PopLayoutFreeze();
            }
        }

        private bool hasPendingAnimationData = false;
        private AnimationData pendingAnimationData;

        private bool hasAnimationData = false;
        private AnimationData animationData;

        private TimeSpan animationStart;

        private void cancelScrollAnimation() {
            if(hasPendingAnimationData) {
                pendingAnimationData.Apply(Target);

            } else if(hasAnimationData) {
                animationData.Apply(Target);
            }

            hasPendingAnimationData = false;
            hasAnimationData = false;
        }

        private (int minY,int maxY) getYLimits() {
            int maxY = 0;

            int minY = 0;
            foreach(var child in Target.Children) {
                var testY = child.Y - child.ComputedHeight + ComputedHeight;
                if(testY < minY) {
                    minY = testY;
                }
            }

            return (minY, maxY);
        }

        private void sendScrollY(ScrollDirection direction) {
            var area = Target.Area;

            int yDifference = -(int)direction * SCROLL_AMOUNT;
            var endY = area.Y + yDifference;
            (int minY, int maxY) = getYLimits();

            if(endY < minY) {
                yDifference = minY - area.Y;
            } else if(endY > maxY) {
                yDifference = maxY - area.Y;
            }

            pendingAnimationData = new AnimationData(area.X,area.Y,0,yDifference);
            hasPendingAnimationData = true;
        }

        public ScrollMode ScrollMode {
            get => scrollMode;
            set => scrollMode = value;
        }
    }
}
