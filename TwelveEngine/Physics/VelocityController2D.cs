using Microsoft.Xna.Framework;

namespace TwelveEngine.Physics {
    public sealed class VelocityController2D:IVelocityData {

        private readonly VelocityController _x, _y;

        public VelocityController2D() {
            _x = new VelocityController(this);
            _y = new VelocityController(this);
        }

        public VelocityController X => _x;
        public VelocityController Y => _y;

        public float Acceleration { get; set; }
        public float Decay { get; set; }
        public float MaxVelocity { get; set; }
        public float ReboundStrength { get; set; }

        public Vector2 Velocity {
            get => new Vector2(X.Velocity,Y.Velocity);
            set {
                X.Velocity = value.X;
                Y.Velocity = value.Y;
            }
        }

        public Vector2 Force {
            get => new Vector2(X.Force,Y.Force);
            set {
                X.Force = value.X;
                Y.Force = value.Y;
            }
        }

        public void Update(float delta) {
            X.Update(delta);
            Y.Update(delta);
        }
    }
}
