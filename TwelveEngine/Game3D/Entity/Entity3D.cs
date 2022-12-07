using Microsoft.Xna.Framework;
using TwelveEngine.EntitySystem;

namespace TwelveEngine.Game3D.Entity {
    public abstract class Entity3D:Entity<World> {

        protected bool WorldMatrixValid { get; set; } = false;

        protected bool PositionValid { get; set; } = false;
        protected bool RotationValid { get; set; } = false;
        protected bool ScaleValid { get; set; } = false;

        private Vector3 _position = Vector3.Zero;
        private Vector3 _rotation = Vector3.Zero;
        private Vector3 _scale = Vector3.One;

        protected override float GetDepth() {
            return _position.Z;
        }

        protected override void SetDepth(float value) {
            var position = _position;
            position.Z = value;
            Position = position;
        }

        public Vector3 Position {
            get => _position;
            set {
                if(_position == value) {
                    return;
                }
                _position = value;
                PositionValid = false;
                WorldMatrixValid = false;
            }
        }

        public Vector3 Rotation {
            get => _rotation;
            set {
                if(_rotation == value) {
                    return;
                }
                _rotation = value;
                RotationValid = false;
                WorldMatrixValid = false;
            }
        }

        public Vector3 Scale {
            get => _scale;
            set {
                if(_scale == value) {
                    return;
                }
                _scale = value;
                ScaleValid = false;
                WorldMatrixValid = false;
            }
        }

        private bool _billboard = false;
        public bool Billboard {
            get => _billboard;
            set {
                if(_billboard == value) {
                    return;
                }
                _billboard = value;
                PositionValid = false;
            }
        }
    }
}
