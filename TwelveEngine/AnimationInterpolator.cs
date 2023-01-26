using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine {
    public sealed class AnimationInterpolator {

        public AnimationInterpolator(TimeSpan duration) {
            Duration = duration;
            Start = -duration;
        }

        public AnimationInterpolator() {
            /* If the start time is set the minimum value there will be an overflow error in the calculation of 'Value' */
            Start = TimeSpan.FromHours(-1); /* Hopefully an (negative) hour is reasonable. I can think of no god forsaken reason you'd have an animation last 1 hour in a video game */
        }

        public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0.1f);

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
            Value = GetValue();
        }

        public float Value { get; private set; }

       private float GetValue() {
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
            return Vector2.Lerp(start.ToVector2(),end.ToVector2(),Value).ToPoint();
        }

        public float Interpolate(float x,float y) {
            return MathHelper.Lerp(x,y,Value);
        }

        public Vector2 Interpolate(Vector2 start,Vector2 end) {
            return Vector2.Lerp(start,end,Value);
        }

        public Vector3 Interpolate(Vector3 start,Vector3 end) {
            return Vector3.Lerp(start,end,Value);
        }

        public Vector3 SmoothStep(Vector3 start,Vector3 end) {
            return Vector3.SmoothStep(start,end,Value);
        }

        public float SmoothStep(float start,float end) {
            return MathHelper.SmoothStep(start,end,Value);
        }

        public Color Interpolate(Color start,Color end) {
            return Color.Lerp(start,end,Value);
        }

        public Rectangle Interpolate(Rectangle start,Rectangle end) {
            Point point = Interpolate(start.Location,end.Location);
            Point size = Interpolate(start.Size,end.Size);
            return new Rectangle(point,size);
        }

        public VectorRectangle Interpolate(VectorRectangle start,VectorRectangle end) {
            Vector2 position = Interpolate(start.Position,end.Position);
            Vector2 size = Interpolate(start.Size,end.Size);
            return new VectorRectangle(position,size);
        }
    }
}
