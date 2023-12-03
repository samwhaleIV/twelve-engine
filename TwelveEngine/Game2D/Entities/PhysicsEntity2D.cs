using nkast.Aether.Physics2D.Dynamics;

namespace TwelveEngine.Game2D.Entities {
    public abstract class PhysicsEntity2D:Entity2D {

        public Body Body { get; private set; }
        public Fixture Fixture { get; private set; }

        public Vector2 Size { get; private init; }
        public Vector2 Origin { get; private init; }

        public PhysicsEntity2D(Vector2 size,Vector2 origin) {
            Size = size;
            Origin = origin;
            OnLoad += PhysicsEntity2D_OnLoad;
            OnUnload += PhysicsEntity2D_OnUnload;
        }

        private Vector2 _position;

        protected override Vector2 GetPosition() {
            if(!IsLoaded) {
                return _position;
            }
            return Body.Position / Owner.PhysicsScale;
        }

        protected override void SetPosition(Vector2 position) {
            if(!IsLoaded) {
                _position = position;
                return;
            }
            Body.Position = position * Owner.PhysicsScale;
        }

        private void PhysicsEntity2D_OnLoad() {
            Vector2 size = Size * Owner.PhysicsScale;

            Body = new Body {
                Position = Vector2.Zero,
                BodyType = BodyType.Dynamic,
                Rotation = 0f,
                FixedRotation = true,
                LocalCenter = Vector2.Zero,
                SleepingAllowed = true,
                Enabled = true,
            };

            float density = 1f;

            Vector2 offset = -Origin * Owner.PhysicsScale;
            Fixture = Body.CreateRectangle(size.X,size.Y,density,offset);

            Fixture.Friction = _friction;
            Body.LinearDamping = _linearDamping;
            Body.Mass = _mass;
            Fixture.Restitution = _restitution;
            Body.Position = _position * Owner.PhysicsScale;

            Owner.PhysicsWorld.Add(Body);
        }

        private void PhysicsEntity2D_OnUnload() {
            Owner.PhysicsWorld.Remove(Body);
        }

        public float _friction, _linearDamping, _mass, _restitution;

        public float Friction {
            get {
                if(!IsLoaded) {
                    return _friction;
                }
                return Fixture.Friction;
            }
            set {
                if(!IsLoaded) {
                    _friction = value;
                    return;
                }
                Fixture.Friction = value;
            }
        }

        public float LinearDamping {
            get {
                if(!IsLoaded) {
                    return _linearDamping;
                }
                return Body.LinearDamping;
            }
            set {
                if(!IsLoaded) {
                    _linearDamping = value;
                    return;
                }
                Body.LinearDamping = value;
            }
        }

        public float Mass {
            get {
                if(!IsLoaded) {
                    return _mass;
                }
                return Body.Mass;
            }
            set {
                if(!IsLoaded) {
                    _mass = value;
                    return;
                }
                Body.Mass = value;
            }
        }

        public float Restitution {
            get {
                if(!IsLoaded) {
                    return _restitution;
                }
                return Fixture.Restitution;
            }
            set {
                if(!IsLoaded) {
                    _restitution = value;
                    return;
                }
                Fixture.Restitution = value;
            }
        }
    }
}
