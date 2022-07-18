using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using tainicom.Aether.Physics2D.Dynamics;
using TwelveEngine.Serial;

namespace TwelveEngine.Game2D.Objects {
    public class PhysicsGameObject:GameObject {

        private readonly BodyType bodyType;
        internal BodyType BodyType => bodyType; //serial helper stem object

        public PhysicsGameObject(ObjectManager owner,int ID,BodyType bodyType) : base(owner,ID) {
            this.bodyType = bodyType;
            OnLoad += PhysicsGameObject_OnLoad;
            OnUnload += PhysicsGameObject_OnUnload;
            OnImport += PhysicsGameObject_OnImport;
            OnExport += PhysicsGameObject_OnExport;
        }

        private Body body;
        private Fixture fixture;

        protected Action<Fixture> OnFixtureChanged;

        private void SetRectangleFixture(Vector2 size) {
            var fixture = body.CreateRectangle(size.X,size.Y,1f,Vector2.Zero);
            body.SetTransform(size * 0.5f,0f);
            this.fixture = fixture;
            OnFixtureChanged?.Invoke(fixture);
        }

        private void SetFixtureSize(Vector2 size) {
            body.Remove(fixture);
            SetRectangleFixture(size);
        }

        private void SetBodyPosition(Vector2 position) {
            body.Position = position;
        }

        protected override void SetPosition(Vector2 position) {
            position += Size * 0.5f;
            base.SetPosition(position);
            if(body == null) {
                return;
            }
            SetBodyPosition(position);
        }

        protected override Vector2 GetPosition() {
            return body.Position - Size * 0.5f;
        }

        protected override float GetRotation() {
            return body.Rotation;
        }

        protected override void SetRotation(float rotation) {
            base.SetRotation(rotation);
            if(body == null) {
                return;
            }
            body.Rotation = rotation;
        }

        protected override void SetSize(Vector2 size) {
            base.SetSize(size);
            if(!IsLoaded) {
                return;
            }
            SetFixtureSize(size);
        }

        private void PhysicsGameObject_OnLoad() {
            Body body = Grid.PhysicsWorld.CreateBody(base.GetPosition(),base.GetRotation(),bodyType);

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

            this.body = body;
            SetRectangleFixture(Size);
        }

        private void PhysicsGameObject_OnUnload() {
            body.ResetDynamics();
            Grid.PhysicsWorld.Remove(body);
            body = null;
            fixture = null;
        }

        private Vector2 _linearVelocity = Vector2.Zero;
        private float _angularVelocity = 0f, _linearDamping = 0f, _angularDamping = 0f, _mass = 1f, _inertia = 0f;
        private bool _isBullet = false, _sleepingAllowed = true, _awake = true, _enabled = true, _fixedRotation = false, _ignoreGravity = true, _ignoreCCD = false;

        public Vector2 LinearVelocity {
            get {
                return body == null ? _linearVelocity : body.LinearVelocity;
            }
            set {
                _linearVelocity = value;
                if(body == null) {
                    return;
                }
                body.LinearVelocity = value;
            }
        }
        public float AngularVelocity {
            get {
                return body == null ? _angularVelocity : body.AngularVelocity;
            }
            set {
                _angularVelocity = value;
                if(body == null) {
                    return;
                }
                body.AngularVelocity = value;
            }
        }
        public float LinearDamping {
            get {
                return body == null ? _linearDamping : body.LinearDamping;
            }
            set {
                _linearDamping = value;
                if(body == null) {
                    return;
                }
                body.LinearDamping = value;
            }
        }
        public float AngularDamping {
            get {
                return body == null ? _angularDamping : body.AngularDamping;
            }
            set {
                _angularDamping = value;
                if(body == null) {
                    return;
                }
                body.AngularDamping = value;
            }
        }
        public bool IsBullet {
            get {
                return body == null ? _isBullet : body.IsBullet;
            }
            set {
                _isBullet = value;
                if(body == null) {
                    return;
                }
                body.IsBullet = value;
            }
        }
        public bool SleepingAllowed {
            get {
                return body == null ? _sleepingAllowed : body.SleepingAllowed;
            }
            set {
                _sleepingAllowed = value;
                if(body == null) {
                    return;
                }
                body.SleepingAllowed = value;
            }
        }
        public bool Awake {
            get {
                return body == null ? _awake : body.Awake;
            }
            set {
                _awake = value;
                if(body == null) {
                    return;
                }
                body.Awake = value;
            }
        }
        public bool Enabled {
            get {
                return body == null ? _enabled : body.Enabled;
            }
            set {
                _enabled = value;
                if(body == null) {
                    return;
                }
                body.Enabled = value;
            }
        }
        public bool FixedRotation {
            get {
                return body == null ? _fixedRotation : body.FixedRotation;
            }
            set {
                _fixedRotation = value;
                if(body == null) {
                    return;
                }
                body.FixedRotation = value;
            }
        }
        public bool IgnoreGravity {
            get {
                return body == null ? _ignoreGravity : body.IgnoreGravity;
            }
            set {
                _ignoreGravity = value;
                if(body == null) {
                    return;
                }
                body.IgnoreGravity = value;
            }
        }
        public float Mass {
            get {
                return body == null ? _mass : body.Mass;
            }
            set {
                _mass = value;
                if(body == null) {
                    return;
                }
                body.Mass = value;
            }
        }
        public float Inertia {
            get {
                return body == null ? _inertia : body.Inertia;
            }
            set {
                _inertia = value;
                if(body == null) {
                    return;
                }
                body.Inertia = value;
            }
        }
        public bool IgnoreCCD {
            get {
                return body == null ? _ignoreCCD : body.IgnoreCCD;
            }
            set {
                _ignoreCCD = value;
                if(body == null) {
                    return;
                }
                body.IgnoreCCD = value;
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
        }
    }
}
