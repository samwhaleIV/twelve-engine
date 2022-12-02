﻿using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D {
    public class Camera {

        private Vector2 _position, _offset;
        private float _scale = Constants.Config.RenderScale;

        private bool _horizontalPadding = false, _verticalPadding = false;

        public Vector2 Position {
            get => _position;
            set {
                if(value == _position) {
                    return;
                }
                _position = value;
                Invalidated?.Invoke();
            }
        }

        public Vector2 Offset {
            get => _offset;
            set {
                if(value == _offset) {
                    return;
                }
                _offset = value;
                Invalidated?.Invoke();
            }
        }

        public float Scale {
            get => _scale;
            set {
                if(value == _scale) {
                    return;
                }
                _scale = value;
                Invalidated?.Invoke();
            }
        }

        public bool HorizontalPadding {
            get => _horizontalPadding;
            set {
                if(value == _horizontalPadding) {
                    return;
                }
                _horizontalPadding = value;
                Invalidated?.Invoke();
            }
        }

        public bool VerticalPadding {
            get => _verticalPadding;
            set {
                if(value == _verticalPadding) {
                    return;
                }
                _verticalPadding = value;
                Invalidated?.Invoke();
            }
        }

        internal event Action Invalidated;

    }
}
