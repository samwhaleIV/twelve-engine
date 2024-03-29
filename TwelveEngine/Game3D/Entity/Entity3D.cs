﻿using TwelveEngine.EntitySystem;

namespace TwelveEngine.Game3D.Entity {
    public abstract class Entity3D:Entity<GameState3D> {

        protected bool WorldMatrixValid { get; set; } = false;

        protected bool PositionValid { get; set; } = false;
        protected bool RotationValid { get; set; } = false;
        protected bool ScaleValid { get; set; } = false;

        private Vector3 _position = Vector3.Zero;
        private Vector3 _rotation = Vector3.Zero;

        private Vector3 _scale = Vector3.One;

        public float Depth {
            get => _position.Z;
            set {
                var position = _position;
                if(position.Z == value) {
                    return;
                }
                position.Z = value;
                Position = position;
            }
        }

        public Vector3 Position {
            get => _position;
            set {
                if(_position == value) {
                    return;
                }
                float oldZ = _position.Z;
                _position = value;
                PositionValid = false;
                WorldMatrixValid = false;
                if(oldZ != value.Z) {
                    NotifySortedOrderChange();
                }
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
