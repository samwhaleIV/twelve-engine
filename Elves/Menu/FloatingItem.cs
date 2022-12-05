using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using TwelveEngine.Game3D;
using TwelveEngine.Game3D.Entity.Types;

namespace Elves.Menu {
    public sealed class FloatingItem:Screenspace3DSprite {

        public const int TOTAL_ITEM_COUNT = 12;

        private const float MIN_DURATION = 20;
        private const float MAX_DURATION = 40;

        private const float WIGGLE_RATE_MIN = 3;
        private const float WIGGLE_RATE_MAX = 7;

        private const float WIGGLE_STRENGTH_MIN = 4;
        private const float WIGGLE_STRENGTH_MAX = 8;

        private const float MIN_X = 1 / 8f;
        private const float MAX_X = 7 / 8f;

        private const float WIGGLE_RATE_RANGE = WIGGLE_RATE_MAX - WIGGLE_RATE_MIN;
        private const float DURATION_RANGE = MAX_DURATION - MIN_DURATION;
        private const float WIGGLE_STRENGTH_RANGE = WIGGLE_STRENGTH_MAX - WIGGLE_STRENGTH_MIN;
        private const float X_RANGE = MAX_X - MIN_X;

        private static readonly (Rectangle TextureSource, int Weight)[] FloatingItemSources = new[]{
                (new Rectangle(0,29,4,4),200),
                (new Rectangle(4,29,23,7),1),
                (new Rectangle(27,29,20,9),1),
                (new Rectangle(0,36,9,23),1),
                (new Rectangle(9,36,9,15),1),
                (new Rectangle(27,38,9,23),1),
                (new Rectangle(18,36,9,23),1),
                (new Rectangle(36,38,23,9),1),
                (new Rectangle(36,47,23,9),1),
                (new Rectangle(27,38,9,23),1),
                (new Rectangle(0,59,23,7),1)
            };

        private static readonly Rectangle[] GrabBag;

        static FloatingItem() {
            int total = 0;
            foreach(var item in FloatingItemSources) {
                total += item.Weight;
            }
            GrabBag = new Rectangle[total];
            int i = 0;
            foreach(var item in FloatingItemSources) {
                int count = item.Weight;
                while(count > 0) {
                    count--;
                    GrabBag[i++] = item.TextureSource;
                }
            }
        }

        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }

        public float X { get; set; }

        public TimeSpan WiggleRate { get; set; }
        public float WiggleStrength { get; set; }

        public bool SinWiggle { get; set; }

        private Rectangle GetRandomFloatingItemSource() {
            int index = random.Next(0,GrabBag.Length);
            return GrabBag[index];
        }

        private readonly Random random;

        public FloatingItem(Random random,Texture2D texture) : base(texture) {
            this.random = random;
            OnUpdate += FloatingItem_OnUpdate;
            PixelSmoothing = false;
        }

        private bool firstTime = true;

        private void Reset() {
            StartTime = Game.Time.TotalGameTime;

            Duration = TimeSpan.FromSeconds(random.NextSingle() * DURATION_RANGE + MIN_DURATION);

            if(firstTime) {
                StartTime = StartTime.Subtract(Duration * random.NextSingle());
                firstTime = false;
                TextureSource = GrabBag[0];
            } else {
                TextureSource = GetRandomFloatingItemSource();
            }
            Depth = random.NextSingle() > 0.5f ? DepthConstants.MiddleFront : DepthConstants.MiddleBack;
            X = random.NextSingle() * X_RANGE + MIN_X;
            WiggleRate = TimeSpan.FromSeconds(random.NextSingle() * WIGGLE_RATE_RANGE + WIGGLE_RATE_MIN);
            WiggleStrength = WIGGLE_RATE_MIN + random.NextSingle() * WIGGLE_STRENGTH_RANGE + WIGGLE_STRENGTH_MIN;
            SinWiggle = random.NextSingle() > 0.5;
        }

        private bool needsReset = true;

        private void FloatingItem_OnUpdate(GameTime gameTime) {
            if(needsReset) {
                Reset();
                needsReset = false;
            }
            float scale = (Owner as MainMenu).GetPixelScale();
            float t = (float)((gameTime.TotalGameTime - StartTime) / Duration);
            if(t < 0) {
                t = 0;
            } else if(t > 1) {
                t = 1;
                needsReset = true;
            }

            Vector2 size = TextureSource.Size.ToVector2() * scale;
            float xValue = X * Game.Viewport.Width;

            Vector2 position = Vector2.Lerp(
                new Vector2(xValue,Game.Viewport.Height),
                new Vector2(xValue,-size.Y),t
            );

            var wiggleT = (float)(gameTime.TotalGameTime / WiggleRate % 1);
            var offset = SinWiggle ? MathF.Sin(MathF.PI * 2 * wiggleT) : MathF.Cos(MathF.PI * 2 * wiggleT);
            position.X += offset * WiggleStrength;

            Area = new VectorRectangle(position-new Vector2(size.X*0.5f,0f),size);
        }
    }
}
