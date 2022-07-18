using Microsoft.Xna.Framework;
using System;
using tainicom.Aether.Physics2D.Dynamics;

namespace TwelveEngine.Game2D.Entity {
    public abstract class PhysicsEntity2D:Entity2D {

        protected World PhysicsWorld => (Owner as PhysicsGrid2D).PhysicsWorld;

        protected Body Body { get; private set; }
        protected Fixture Fixture { get; private set; }

        protected Action<Fixture> OnFixtureChanged;

        private void SetRectangleFixture(Vector2 size) {
            var fixture = Body.CreateRectangle(size.X,size.Y,1f,Vector2.Zero);
            Fixture = fixture;
            OnFixtureChanged?.Invoke(fixture);
        }

        private void SetFixtureSize(Vector2 size) {
            Body.Remove(Fixture);
            SetRectangleFixture(size);
        }

        private void SetBodyPosition(Vector2 position) {
            Body.ResetDynamics();
            Body.Position = position;
        }

        protected override void SetPosition(Vector2 position) {
            position += Size * 0.5f;
            base.SetPosition(position);
            if(!IsLoaded) {
                return;
            }
            SetBodyPosition(position);
        }

        protected override Vector2 GetPosition() {
            return Body.Position - Size * 0.5f;
        }

        protected override void SetSize(Vector2 size) {
            base.SetSize(size);
            if(!IsLoaded) {
                return;
            }
            SetFixtureSize(size);
        }

        public PhysicsEntity2D() {
            OnLoad += PhysicsEntity2D_OnLoad;
            OnUnload += PhysicsEntity2D_OnUnload;
        }

        private void PhysicsEntity2D_OnLoad() {
            var body = PhysicsWorld.CreateBody(base.GetPosition(),0f,BodyType.Dynamic);
            body.Rotation = 0f;
            body.FixedRotation = true;
            Body = body;
            SetRectangleFixture(Size);
        }

        private void PhysicsEntity2D_OnUnload() {
            Body.ResetDynamics();
            PhysicsWorld.Remove(Body);
            Body = null;
            Fixture = null;
        }

    }
}
