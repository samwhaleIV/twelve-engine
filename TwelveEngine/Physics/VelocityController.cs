using System;

namespace TwelveEngine.Physics {
    public sealed class VelocityController {

        private readonly IVelocityData _data;

        public VelocityController() => _data = new VelocityData();
        public VelocityController(IVelocityData data) => _data = data;

        public float Decay => _data.Decay;
        public float Acceleration => _data.Acceleration;
        public float MaxVelocity => _data.MaxVelocity;
        public float ReboundStrength => _data.ReboundStrength;

        public float Velocity { get; set; } = 0f;
        public float Force { get; set; } = 0f;

        private void Deaccelerate(float delta) {
            float difference = delta * Decay;
            int sign = Math.Sign(Velocity);

            Velocity += difference * -sign;
            if(Math.Sign(Velocity) != sign) {
                Velocity = 0f;
            }
        }

        private void Accelerate(float delta,float accelerationScale) {
            int sign = Math.Sign(Force);

            float acceleration = Acceleration * accelerationScale;
            if(Velocity != 0f && Math.Sign(Velocity) != sign) {
                acceleration *= ReboundStrength;
            }

            float difference = delta * acceleration * Force;
            Velocity += difference;

            if(Math.Abs(Velocity) > MaxVelocity) {
                Velocity = MaxVelocity * sign;
            }
        }

        public void Update(float delta,float accelerationScale = 1f) {
            float timeDelta = delta;
            if(Force != 0f) {
                Accelerate(timeDelta,accelerationScale);
            } else if(Velocity != 0f) {
                Deaccelerate(timeDelta);
            }
        }
    }
}
