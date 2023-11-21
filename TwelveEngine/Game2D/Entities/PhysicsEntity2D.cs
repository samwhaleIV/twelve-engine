using nkast.Aether.Physics2D.Dynamics;

namespace TwelveEngine.Game2D.Entities {
    public abstract class PhysicsEntity2D:Entity2D {

        private readonly Body _physicsBody;
        public Body PhysicsBody => _physicsBody;

        private Fixture _bodyFixture;
        public Vector2 Size { get; private init; }

        public PhysicsEntity2D(Vector2 size) {

            Size = size;

            Vector2 localCenter = Vector2.Zero;
            Vector2 fixtureOffset = Vector2.Zero;

            float fixtureDensity = 1f;
            float bodyMass = 10f;

            _physicsBody = new Body() {
                Position = Vector2.Zero,
                FixedRotation = true,
                BodyType = BodyType.Dynamic,
                SleepingAllowed = true,
                Enabled = true,
                Mass = bodyMass,
                LocalCenter = localCenter
            };

            _bodyFixture = _physicsBody.CreateRectangle(size.X,size.Y,fixtureDensity,fixtureOffset);

            OnLoad += PhysicsEntity2D_OnLoad;
            OnUnload += PhysicsEntity2D_OnUnload;
        }

        protected override Vector2 GetPosition() {
            return _physicsBody.Position;
        }

        protected override void SetPosition(Vector2 position) {
            _physicsBody.Position = position;
        }

        private void PhysicsEntity2D_OnUnload() {
            Owner.PhysicsWorld.Remove(_physicsBody);
        }

        private void PhysicsEntity2D_OnLoad() {
            Owner.PhysicsWorld.Add(_physicsBody);
        }
    }
}
