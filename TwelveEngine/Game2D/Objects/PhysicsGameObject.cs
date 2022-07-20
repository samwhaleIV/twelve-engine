using Microsoft.Xna.Framework;
using System;
using tainicom.Aether.Physics2D.Dynamics;
using TwelveEngine.Serial;

namespace TwelveEngine.Game2D.Objects {
    public class PhysicsGameObject:GameObject {

        private readonly BodyType bodyType;
        internal BodyType BodyType => bodyType;

        public PhysicsGameObject(ObjectManager owner,int ID,BodyType bodyType) : base(owner,ID) {
            this.bodyType = bodyType;

            OnLoad += PhysicsGameObject_OnLoad;
            OnUnload += PhysicsGameObject_OnUnload;

            OnImport += PhysicsGameObject_OnImport;
            OnExport += PhysicsGameObject_OnExport;
        }

        protected Action<Fixture> OnFixtureChanged;

        protected Body Body { get; private set; }
        protected Fixture Fixture { get; private set; }

        private void UpdateFixture() {
            if(Fixture != null) {
                Body.Remove(Fixture);
            }
            var size = base.GetSize();
            var hitboxSize = _hitboxSize;
            var newFixture = Body.CreateRectangle(hitboxSize.X,hitboxSize.Y,1f,HitboxOffset - size * 0.5f + hitboxSize * 0.5f);
            Body.SetTransform(size * 0.5f,0f);
            Fixture = newFixture;
            OnFixtureChanged?.Invoke(Fixture);
        }

        private void SetBodyPosition(Vector2 position) {
            Body.Position = position;
        }

        protected override void SetPosition(Vector2 position) {
            base.SetPosition(position);
            position += Size * 0.5f;
            if(Body == null) {
                return;
            }
            SetBodyPosition(position);
        }

        protected override Vector2 GetPosition() {
            return Body.Position - Size * 0.5f;
        }

        protected override float GetRotation() {
            return Body.Rotation;
        }

        protected override void SetRotation(float rotation) {
            base.SetRotation(rotation);
            if(Body == null) {
                return;
            }
            Body.Rotation = rotation;
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

        private void PhysicsGameObject_OnLoad() {
            Body body = Grid.PhysicsWorld.CreateBody(base.GetPosition(),base.GetRotation(),bodyType);
            LoadBodyProperties(body);
            Body = body;
            UpdateFixture();
        }

        private void PhysicsGameObject_OnUnload() {
            Body.ResetDynamics();
            Grid.PhysicsWorld.Remove(Body);
        }

        private Vector2 _linearVelocity = Vector2.Zero;
        private float _angularVelocity = 0f, _linearDamping = 0f, _angularDamping = 0f, _mass = 1f, _inertia = 0f;
        private bool _isBullet = false, _sleepingAllowed = true, _awake = true, _enabled = true, _fixedRotation = false, _ignoreGravity = true, _ignoreCCD = false;

        private Vector2 _hitboxSize = Vector2.One, _hitboxOffset = Vector2.Zero;

        private void LoadBodyProperties(Body body) {
            body.LinearVelocity = _linearVelocity;
            body.AngularVelocity = _angularVelocity;
            body.LinearDamping = _linearDamping;
            body.AngularDamping = _angularDamping;
            body.IsBullet = _isBullet;
            body.SleepingAllowed = _sleepingAllowed;
            body.Awake = _awake;
            body.Enabled = _enabled;
            body.FixedRotation = _fixedRotation;
            body.IgnoreGravity = _ignoreGravity;
            body.Mass = _mass;
            body.Inertia = _inertia;
            body.IgnoreCCD = _ignoreCCD;
        }

        public Vector2 HitboxSize {
            get => _hitboxSize;
            set {
                if(_hitboxSize == value) {
                    return;
                }
                _hitboxSize = value;
                if(Body == null) {
                    return;
                }
                UpdateFixture();
            }
        }

        public Vector2 HitboxOffset {
            get => _hitboxOffset;
            set {
                if(_hitboxOffset == value) {
                    return;
                }
                _hitboxOffset = value;
                if(Body == null) {
                    return;
                }
                UpdateFixture();
            }
        }

        public Vector2 LinearVelocity {
            get => Body == null ? _linearVelocity : Body.LinearVelocity;
            set {
                _linearVelocity = value;
                if(Body == null) return;
                Body.LinearVelocity = value;
            }
        }
        public float AngularVelocity {
            get => Body == null ? _angularVelocity : Body.AngularVelocity;
            set {
                _angularVelocity = value;
                if(Body == null) return;
                Body.AngularVelocity = value;
            }
        }
        public float LinearDamping {
            get => Body == null ? _linearDamping : Body.LinearDamping;
            set {
                _linearDamping = value;
                if(Body == null) return;
                Body.LinearDamping = value;
            }
        }
        public float AngularDamping {
            get => Body == null ? _angularDamping : Body.AngularDamping;
            set {
                _angularDamping = value;
                if(Body == null) return;
                Body.AngularDamping = value;
            }
        }
        public bool IsBullet {
            get => Body == null ? _isBullet : Body.IsBullet;
            set {
                _isBullet = value;
                if(Body == null) return;
                Body.IsBullet = value;
            }
        }
        public bool SleepingAllowed {
            get => Body == null ? _sleepingAllowed : Body.SleepingAllowed;
            set {
                _sleepingAllowed = value;
                if(Body == null) return;
                Body.SleepingAllowed = value;
            }
        }
        public bool Awake {
            get => Body == null ? _awake : Body.Awake;
            set {
                _awake = value;
                if(Body == null) return;
                Body.Awake = value;
            }
        }
        public bool Enabled {
            get => Body == null ? _enabled : Body.Enabled;
            set {
                _enabled = value;
                if(Body == null) return;
                Body.Enabled = value;
            }
        }
        public bool FixedRotation {
            get => Body == null ? _fixedRotation : Body.FixedRotation;
            set {
                _fixedRotation = value;
                if(Body == null) return;
                Body.FixedRotation = value;
            }
        }
        public bool IgnoreGravity {
            get => Body == null ? _ignoreGravity : Body.IgnoreGravity;
            set {
                _ignoreGravity = value;
                if(Body == null) return;
                Body.IgnoreGravity = value;
            }
        }
        public float Mass {
            get => Body == null ? _mass : Body.Mass;
            set {
                _mass = value;
                if(Body == null) return;
                Body.Mass = value;
            }
        }
        public float Inertia {
            get => Body == null ? _inertia : Body.Inertia;
            set {
                _inertia = value;
                if(Body == null) return;
                Body.Inertia = value;
            }
        }
        public bool IgnoreCCD {
            get => Body == null ? _ignoreCCD : Body.IgnoreCCD;
            set {
                _ignoreCCD = value;
                if(Body == null) return;
                Body.IgnoreCCD = value;
            }
        }

        private void PhysicsGameObject_OnExport(SerialFrame frame) {
            frame.Set(LinearVelocity);
            frame.Set(AngularVelocity);
            frame.Set(LinearDamping);
            frame.Set(AngularDamping);
            frame.Set(IsBullet);
            frame.Set(SleepingAllowed);
            frame.Set(Awake);
            frame.Set(Enabled);
            frame.Set(FixedRotation);
            frame.Set(IgnoreGravity);
            frame.Set(Mass);
            frame.Set(Inertia);
            frame.Set(IgnoreCCD);
            frame.Set(HitboxOffset);
            frame.Set(HitboxSize);
        }

        private void PhysicsGameObject_OnImport(SerialFrame frame) {
            LinearVelocity = frame.GetVector2();
            AngularVelocity = frame.GetFloat();
            LinearDamping = frame.GetFloat();
            AngularDamping = frame.GetFloat();
            IsBullet = frame.GetBool();
            SleepingAllowed = frame.GetBool();
            Awake = frame.GetBool();
            Enabled = frame.GetBool();
            FixedRotation = frame.GetBool();
            IgnoreGravity = frame.GetBool();
            Mass = frame.GetFloat();
            Inertia = frame.GetFloat();
            IgnoreCCD = frame.GetBool();
            HitboxOffset = frame.GetVector2();
            HitboxSize = frame.GetVector2();
        }
    }
}
