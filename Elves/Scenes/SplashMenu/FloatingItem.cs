using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using TwelveEngine.Game3D.Entity.Types;
using TwelveEngine;

namespace Elves.Scenes.SplashMenu {
    public sealed class FloatingItem:Screenspace3DSprite {

        public const int TOTAL_ITEM_COUNT = 64;

        private const float MIN_DURATION = 20;
        private const float MAX_DURATION = 40;

        private const float WIGGLE_RATE_MIN = 3;
        private const float WIGGLE_RATE_MAX = 7;

        private const float WIGGLE_STRENGTH_MIN = 4;
        private const float WIGGLE_STRENGTH_MAX = 8;
        public const float WIGGLE_BASE_SCALE = 20;

        private const float MIN_X = 0;
        private const float MAX_X = 1;


        private const float WIGGLE_RATE_RANGE = WIGGLE_RATE_MAX - WIGGLE_RATE_MIN;
        private const float DURATION_RANGE = MAX_DURATION - MIN_DURATION;
        private const float WIGGLE_STRENGTH_RANGE = WIGGLE_STRENGTH_MAX - WIGGLE_STRENGTH_MIN;
        private const float X_RANGE = MAX_X - MIN_X;

        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }

        public float X { get; set; }

        public TimeSpan WiggleRate { get; set; }
        public float WiggleStrength { get; set; }

        public bool RotationPolarity { get; set; }

        private readonly Random random;

        private readonly SplashMenuState menu;

        public FloatingItem(SplashMenuState menu,Random random,Texture2D texture) : base(texture) {
            this.menu = menu;
            this.random = random;
            OnUpdate += FloatingItem_OnUpdate;
            PixelSmoothing = false;
            Depth = (Constants.Depth.MiddleClose - Constants.Depth.MiddleFar) * random.NextSingle() + Constants.Depth.MiddleFar;
        }

        private bool firstTime = true;

        private readonly static GrabBag<Rectangle> grabBag = FloatingItemData.GetGrabBag();

        private bool RandomBoolean() => random.NextSingle() > 0.5f;

        private void Reset() {
            StartTime = Now;

            Duration = TimeSpan.FromSeconds(random.NextSingle() * DURATION_RANGE + MIN_DURATION);

            if(firstTime) {
                StartTime = StartTime.Subtract(Duration * random.NextSingle());
                firstTime = false;
            }
            TextureSource = grabBag.GetRandom(random);
            X = random.NextSingle() * X_RANGE + MIN_X;
            WiggleRate = TimeSpan.FromSeconds(random.NextSingle() * WIGGLE_RATE_RANGE + WIGGLE_RATE_MIN);
            WiggleStrength = WIGGLE_RATE_MIN + random.NextSingle() * WIGGLE_STRENGTH_RANGE + WIGGLE_STRENGTH_MIN;
            RotationPolarity = RandomBoolean();
            MirrorY = RandomBoolean();
        }

        private bool needsReset = true;

        private void FloatingItem_OnUpdate() {
            if(needsReset) {
                Reset();
                needsReset = false;
            }
            float scale = menu.GetUIScale();
            float t = (float)((Now - StartTime) / Duration);
            if(t < 0) {
                t = 0;
            } else if(t > 1) {
                t = 1;
                needsReset = true;
            }

            Vector2 size = TextureSource.Size.ToVector2() * scale;
            float xValue = X * Game.Viewport.Width;

            Rotation = new Vector3(0f,0f,t * 360f * (RotationPolarity ? 1 : -1));

            Vector2 position = Vector2.Lerp(
                new Vector2(xValue,Game.Viewport.Height),
                new Vector2(xValue,-MathF.Max(size.X,size.Y)),t
            );

            var wiggleT = (float)(Now / WiggleRate % 1);
            var offset = RotationPolarity ? MathF.Sin(MathF.PI * 2 * wiggleT) : MathF.Cos(MathF.PI * 2 * wiggleT);
            position.X += (offset * WiggleStrength) / WIGGLE_BASE_SCALE * scale;

            Area = new FloatRectangle(position - new Vector2(size.X*0.5f,0f),size);
        }
    }
}
