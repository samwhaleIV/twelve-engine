using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;

namespace TwelveEngine.Game2D.Entity {
    public abstract class PhysicsEntity2D:Entity2D {

        protected World PhysicsWorld => (Owner as PhysicsGrid2D).PhysicsWorld;

        public Body Body { get; private set; }
        public Fixture Fixture { get; private set; }

        private void SetRectangleFixture(Vector2 size) {
            Fixture = Body.CreateRectangle(size.X,size.Y,1f,Vector2.Zero);
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
            base.SetPosition(position);
            if(!IsLoaded) {
                return;
            }
            SetBodyPosition(position);
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
            Body = PhysicsWorld.CreateBody(Position,0f,BodyType.Dynamic);
            Body.IgnoreGravity = true;
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
