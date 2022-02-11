using System;
using TwelveEngine.Serial;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D {
    public class Camera:ISerializable {

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

        public void SetPadding(bool horizontal,bool vertical) {
            _horizontalPadding = horizontal;
            _verticalPadding = vertical;
            Invalidated?.Invoke();
        }

        public void SetPadding(bool all) {
            _horizontalPadding = all;
            _verticalPadding = all;
            Invalidated?.Invoke();
        }

        public CameraPadding Padding {
            get {
                if(HorizontalPadding && VerticalPadding) {
                    return CameraPadding.All;
                } else if(HorizontalPadding) {
                    return CameraPadding.Horizontal;
                } else if(VerticalPadding) {
                    return CameraPadding.Vertical;
                } else {
                    return CameraPadding.None;
                }
            }
            set {
                switch(value) {
                    case CameraPadding.All:
                        _horizontalPadding = true;
                        _verticalPadding = true;
                        Invalidated?.Invoke();
                        break;
                    case CameraPadding.Horizontal:
                        _horizontalPadding = true;
                        _verticalPadding = false;
                        Invalidated?.Invoke();
                        break;
                    case CameraPadding.Vertical:
                        _horizontalPadding = false;
                        _verticalPadding = true;
                        Invalidated?.Invoke();
                        break;
                    default:
                    case CameraPadding.None:
                        _horizontalPadding = false;
                        _verticalPadding = false;
                        Invalidated?.Invoke();
                        break;
                }
            }
        }

        public void Export(SerialFrame frame) {
            frame.Set(Position);
            frame.Set(Offset);
            frame.Set(Scale);
            frame.Set(HorizontalPadding);
            frame.Set(VerticalPadding);
        }

        public void Import(SerialFrame frame) {
            _position = frame.GetVector2();
            _offset = frame.GetVector2();
            _horizontalPadding = frame.GetBool();
            _verticalPadding = frame.GetBool();
        }
    }
}
