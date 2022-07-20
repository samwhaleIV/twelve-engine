using Microsoft.Xna.Framework;
using System;
using tainicom.Aether.Physics2D.Dynamics;

namespace TwelveEngine.Game2D.Entity {
    public abstract class PhysicsEntity2D:Entity2D {

        protected World PhysicsWorld => (Owner as PhysicsGrid2D).PhysicsWorld;

        protected Body Body { get; private set; }
        protected Fixture Fixture { get; private set; }

        protected Action<Fixture> OnFixtureChanged;

        private void UpdateFixture() {
            if(Fixture != null) {
                Body.Remove(Fixture);
            }
            var size = base.GetSize();
            var fixture = Body.CreateRectangle(size.X,size.Y,1f,Vector2.Zero);
            Fixture = fixture;
            OnFixtureChanged?.Invoke(fixture);
        }

        private void SetBodyPosition(Vector2 position) {
            Body.ResetDynamics();
            Body.Position = position;
        }

        protected override void SetPosition(Vector2 position) {
            position += Size * 0.5f;
            base.SetPosition(position);
            if(Body == null) {
                return;
            }
            SetBodyPosition(position);
        }

        protected override Vector2 GetPosition() {
            return Body.Position - Size * 0.5f;
        }

        protected override void SetSize(Vector2 size) {
            if(size == base.GetSize()) {
                return;
            }
            base.SetSize(size);
            if(Body == null) {
                return;
            }
            UpdateFixture();
        }

        public PhysicsEntity2D() {
            OnLoad += PhysicsEntity2D_OnLoad;
            OnUnload += PhysicsEntity2D_OnUnload;
        }

        private void PhysicsEntity2D_OnLoad() {
            var body = PhysicsWorld.CreateBody(base.GetPosition(),0f,BodyType.Dynamic);
            body.FixedRotation = true;
            Body = body;
            UpdateFixture();
        }

        private void PhysicsEntity2D_OnUnload() {
            PhysicsWorld.Remove(Body);
            Body = null;
            Fixture = null;
        }

    }
}
