using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine;
using TwelveEngine.Game3D.Entity.Types;

namespace Elves.Scenes.SaveSelect {
    public sealed class SelectionFinger:Screenspace3DSprite {

        public static readonly Vector2 BaseScale = new Vector2(174,40);
        public static readonly float AspectRatio = BaseScale.X / BaseScale.Y;

        private AnimationInterpolator positionAnimator = new(Constants.AnimationTiming.SaveFingerMovementDuration);

        public SelectionFinger(Texture2D texture) : base(texture) {
            TextureSource = new(0,0,(int)BaseScale.X,(int)BaseScale.Y);
            PixelSmoothing = false;
            OnUpdate += SelectionFinger_OnUpdate;
        }

        private void SelectionFinger_OnUpdate() {
            positionAnimator.Update(Now);
            var bounds = Game.Viewport.Bounds.Size.ToVector2();
            var height = 0.375f * bounds.Y;
            var size = new Vector2(AspectRatio * height,height);
            var y = GetRelativeY() * bounds.Y - size.Y * 0.2f;
            var x = bounds.X * (2f / 3f) - size.X * 1.24f;
            Area = new VectorRectangle(x,y,size);
        }

        private float relativeYStart = 0f, relativeYEnd = 0f;

        public float GetRelativeY() => positionAnimator.SmoothStep(relativeYStart,relativeYEnd);

        public void AnimateTo(float relativeY) {
            relativeYStart = GetRelativeY();
            positionAnimator.Reset(Now);
            relativeYEnd = relativeY;
        }

        public void SetTo(float relativeY) {
            relativeYStart = relativeY;
            relativeYEnd = relativeY;
        }
    }
}
