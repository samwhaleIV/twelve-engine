using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine {
    public sealed class AnimationInterpolator {

        private readonly TimeSpan _duration;

        public AnimationInterpolator(TimeSpan duration) {
            _duration =  duration;
            Start = -duration;
        }

        public TimeSpan Duration => _duration;

        public TimeSpan Start { get; private set; } = TimeSpan.Zero;
        public TimeSpan Now { get; private set; } = TimeSpan.Zero;

        public void Reset(TimeSpan now) {
            Now = now;
            Start = now;
        }

        public void ResetCarryOver(TimeSpan now) {
            if(!IsFinished) {
                var progress = GetValue();
                var inverseTime = (1f-progress) * Duration;
                Start = now - inverseTime;
            } else {
                Start = now;
            }
            Now = now;
        }

        public void Update(TimeSpan now) {
            Now = now;
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
            Start = Now - Duration;
        }

        public Point Interpolate(Point start,Point end) {
            return Vector2.Lerp(start.ToVector2(),end.ToVector2(),GetValue()).ToPoint();
        }

        public float Interpolate(float x,float y) {
            return MathHelper.Lerp(x,y,GetValue());
        }

        public Vector2 Interpolate(Vector2 start,Vector2 end) {
            return Vector2.Lerp(start,end,GetValue());
        }

        public Vector3 Interpolate(Vector3 start,Vector3 end) {
            return Vector3.Lerp(start,end,GetValue());
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
