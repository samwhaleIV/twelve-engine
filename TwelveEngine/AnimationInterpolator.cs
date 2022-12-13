using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine {
    public sealed class AnimationInterpolator {

        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public TimeSpan Start { get; set; } = TimeSpan.Zero;
        public TimeSpan Now { get; set; } = TimeSpan.Zero;

        public void SetToEnd() {
            Start = Now - Duration;
        }

       public float GetValue() {
            float value = (float)((Now - Start) / Duration);
            if(value > 1) {
                value = 1;
            } else if(value < 0) {
                value = 0;
            }
            return value;
       }

        public bool IsFinished => Now - Start >= Duration;

        public void Finish() {
            Start = TimeSpan.MinValue;
        }

        public Point Interpolate(Point start,Point end) {
            return Vector2.Lerp(start.ToVector2(),end.ToVector2(),GetValue()).ToPoint();
        }

        public Vector2 Interpolate(Vector2 start,Vector2 end) {
            return Vector2.Lerp(start,end,GetValue());
        }

        public Color Interpolate(Color start,Color end) {
            return Color.Lerp(start,end,GetValue());
        }

        public Rectangle Interpolate(Rectangle start,Rectangle end) {
            Point point = Interpolate(start.Location,end.Location);
            Point size = Interpolate(start.Size,end.Size);
            return new Rectangle(point,size);
        }

        public VectorRectangle Interpolate(VectorRectangle start,VectorRectangle end) {
            Vector2 position = Interpolate(start.Location,end.Location);
            Vector2 size = Interpolate(start.Size,end.Size);
            return new VectorRectangle(position,size);
        }
    }
}
