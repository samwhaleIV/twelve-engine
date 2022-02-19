using System;

namespace TwelveEngine.Time {
    public sealed class VelocityController {

        public float Accel { get; set; } = 7f;
        public float ReboundAccel { get; set; } = 14f;

        public float Decay { get; set; } = 7f;
        public float MaxVelocity { get; set; } = 2.5f;

        private float velocity = 0f;

        private enum VelocityState { Rising, Falling }
        private VelocityState state = VelocityState.Falling;

        private int sign = 1;

        public void SoftAccel(int sign) {
            this.sign = sign;
            state = VelocityState.Rising;
        }
        public void SoftDecay() => state = VelocityState.Falling;

        public void HardAccel() {
            velocity = MaxVelocity;
            state = VelocityState.Rising;
        }

        public void HardDecay() {
            velocity = 0f;
            state = VelocityState.Falling;
        }

        public void Update(TimeSpan delta) {
            float timeDelta = (float)delta.TotalSeconds;
            if(state == VelocityState.Rising) {
                float accel = Accel;
                if(sign > 0) {
                    if(velocity < 0f) accel = ReboundAccel;
                    velocity += timeDelta * accel;
                    if(velocity > MaxVelocity) velocity = MaxVelocity;
                } else if(sign < 0) {
                    if(velocity > 0f) accel = ReboundAccel;
                    velocity -= timeDelta * accel;
                    if(velocity < -MaxVelocity) velocity = -MaxVelocity;
                }
            } else {
                if(sign > 0) {
                    velocity -= timeDelta * Decay;
                    if(velocity < 0f) velocity = 0f;
                } else if(sign < 0) {
                    velocity += timeDelta * Decay;
                    if(velocity > 0f) velocity = 0f;
                }
            }
        }

        public bool IsMoving => velocity > 0f;
        public float Velocity => velocity;
    }
}
