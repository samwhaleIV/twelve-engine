using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Elves.UI.Battle {
    public enum HealthBarAlignment { Left, Right };

    public sealed class HealthBar:UIElement {

        public HealthBar() => Texture = UITextures.Panel;

        public HealthBarAlignment Alignment { get; set; } = HealthBarAlignment.Left;
        public float Value { get; set; } = 1f;

        private TimeSpan Now;
        private int Scale;

        public void Update(int scale,TimeSpan now) {
            Scale = scale;
            Now = now;
        }

        private TimeSpan dropHealthAnimateStart = TimeSpan.Zero - DropHealthDuration;

        public void DropHealthAnimate(TimeSpan now) {
            dropHealthAnimateStart = now;
        }

        private float GetDropHealthNormal() {
            var difference = Now - dropHealthAnimateStart;
            var t = (float)(difference / DropHealthDuration);
            if(t > 1) {
                t = IsDead ? t : 1;
            } else if(t < 0) {
                t = 0;
            }
            return t;
        }

        public float WaveSpeed { get; set; } = 1f;
        public float WaveStrength { get; set; } = 8;

        private bool IsDead => Value <= 0f;

        private static TimeSpan DropHealthDuration => TimeSpan.FromMilliseconds(100);

        private int GetStripYOffset(float xNormal,float t) {
            float time = MathF.PI * 2 * t;
            float distance = xNormal * MathF.PI * 2;
            float offset = MathF.Sin((time + distance) * WaveSpeed) * WaveStrength / 19 * Scale;
            return (int)MathF.Round(offset);
        }

        private (Color Color,int YOffset,Point textureOffset) GetStripeData(float xNormal,Color healthColor) {
            Color color;
            int stripYOffset;
            float healthDropNormal = GetDropHealthNormal();
            Point textureOffset = Point.Zero;
            if(Alignment == HealthBarAlignment.Left) {
                if(xNormal < Value) {
                    color = healthColor;
                } else {
                    color = Color.White;
                    textureOffset = new Point(32,0);
                }
                stripYOffset = GetStripYOffset(xNormal,healthDropNormal);
            } else {
                if(xNormal < 1 - Value) {
                    color = Color.White;
                    textureOffset = new Point(32,0);
                } else {
                    color = healthColor;
                }
                stripYOffset = GetStripYOffset(1-xNormal,healthDropNormal);
            }
            return (color, stripYOffset, textureOffset);
        }

        public override void Draw(SpriteBatch spriteBatch,Color? color = null) {
            Rectangle area = Area;
            Color healthColor = color ?? Color.White;

            int pixelSize = Area.Height / 16;
            pixelSize += 1;

            (Color Color, int YOffset, Point textureOffset) stripeData;

            int pixelCount = (int)MathF.Ceiling((Area.Width - pixelSize * 2) / (float)pixelSize);
            float halfPixelSize = pixelSize / 2;

            stripeData = GetStripeData(halfPixelSize / area.Width,healthColor);
            spriteBatch.Draw(
                Texture,
                new Rectangle(area.X,area.Y + stripeData.YOffset,pixelSize,area.Height),
                new Rectangle(16+stripeData.textureOffset.X,stripeData.textureOffset.Y,1,16),
            stripeData.Color);
            for(int i = 1;i<pixelCount;i++) {
                stripeData = GetStripeData((pixelSize + i * pixelSize + halfPixelSize) / area.Width,healthColor);
                spriteBatch.Draw(
                    Texture,
                    new Rectangle(area.X + i * pixelSize,area.Y + stripeData.YOffset,pixelSize,area.Height),
                    new Rectangle(17+stripeData.textureOffset.X,stripeData.textureOffset.Y,1,16),
                stripeData.Color);
            }
            int overshoot = (pixelCount * pixelSize) - (Area.Width - pixelSize * 2);

            int offsetStripeX = area.X + pixelCount * pixelSize - overshoot;
            stripeData = GetStripeData((area.Width - halfPixelSize - pixelSize) / area.Width,healthColor);
            spriteBatch.Draw(
                Texture,
                new Rectangle(offsetStripeX,area.Y+stripeData.YOffset,pixelSize,area.Height),
                new Rectangle(17+stripeData.textureOffset.X,stripeData.textureOffset.Y,1,16),
            stripeData.Color);

            stripeData = GetStripeData((area.Width - halfPixelSize) / area.Width,healthColor);
            spriteBatch.Draw(
                Texture,
                new Rectangle(area.Right - pixelSize,area.Y + stripeData.YOffset,pixelSize,area.Height),
                new Rectangle(16+stripeData.textureOffset.X,stripeData.textureOffset.Y,1,16),
            stripeData.Color);
        }
    }

}
