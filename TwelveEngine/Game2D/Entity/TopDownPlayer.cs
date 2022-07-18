using Microsoft.Xna.Framework;
using System;
using Porthole.Physics;
using tainicom.Aether.Physics2D;
using tainicom.Aether.Physics2D.Dynamics;

namespace TwelveEngine.Game2D.Entity {
    public abstract class TopDownPlayer:PhysicsEntity2D {

        public bool CameraTracking { get; set; } = false;

        public float ReboundMultiplier { get; set; } = 1.25f;

        public float Force { get; set; } = 16f;

        private float _damping = 6f, _friction = 1f, _restitution = 0.1f;

        public float Damping {
            get => _damping;
            set {
                _damping = value;
                if(Body == null) {
                    return;
                }
                Body.LinearDamping = value;
            }
        }

        public float Friction {
            get => _friction;
            set {
                _friction = value;
                if(Fixture == null) {
                    return;
                }
                Fixture.Friction = value;
            }
        }
        public float Restitution {
            get => _restitution;
            set {
                _restitution = value;
                if(Fixture == null) {
                    return;
                }
                Fixture.Restitution = value;
            }
        }

        public TopDownPlayer() {
            OnUpdate += TopDownPlayer_OnUpdate;
            OnLoad += TopDownPlayer_OnLoad;
            OnUnload += TopDownPlayer_OnUnload;
            OnFixtureChanged += TopDownPlayer_OnFixtureChanged;
        }

        private void TopDownPlayer_OnFixtureChanged(Fixture fixture) {
            fixture.Restitution = Restitution;
            fixture.Friction = Friction;
        }

        private void TopDownPlayer_OnUnload() {
            Owner.OnUpdate -= Owner_OnUpdate;
        }

        private void Owner_OnUpdate(GameTime gameTime) {
            if(!CameraTracking) {
                return;
            }
            Owner.Camera.Position = Position;
        }

        private void TopDownPlayer_OnLoad() {
            Body.IgnoreGravity = true;
            Body.LinearDamping = Damping;
            Owner.OnUpdate += Owner_OnUpdate;
        }

        private void TopDownPlayer_OnUpdate(GameTime gameTime) {
            var force = Owner.Input.GetDelta2D() * Force;
            var velocity = Body.LinearVelocity;
            if(!(force.X == 0 || velocity.X == 0) && Math.Sign(force.X) != Math.Sign(velocity.X)) {
                force.X *= ReboundMultiplier;
            }
            if(!(force.Y == 0 || velocity.Y == 0) && Math.Sign(force.Y) != Math.Sign(velocity.Y)) {
                force.Y *= ReboundMultiplier;
            }
            Body.ApplyForce(force);
        }
    }
}
