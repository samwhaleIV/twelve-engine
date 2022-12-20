using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine {
    public struct VectorRectangle {
        private float _x, _y, _width, _height;

        public VectorRectangle(int x,int y,int width,int height) {
            _x = x;
            _y = y;

            _width = width;
            _height = height;
        }

        public VectorRectangle(int x,int y,float width,float height) {
            _x = x;
            _y = y;

            _width = width;
            _height = height;
        }

        public VectorRectangle(float x,float y,int width,int height) {
            _x = x;
            _y = y;

            _width = width;
            _height = height;
        }

        public VectorRectangle(float x,float y,float width,float height) {
            _x = x;
            _y = y;

            _width = width;
            _height = height;
        }

        public VectorRectangle(Vector2 position,Vector2 size) {
            _x = position.X;
            _y = position.Y;

            _width = size.X;
            _height = size.Y;
        }

        public VectorRectangle(Vector2 position,float width,float height) {
            _x = position.X;
            _y = position.Y;

            _width = width;
            _height = height;
        }

        public VectorRectangle(float x,float y,Vector2 size) {
            _x = x;
            _y = y;

            _width = size.X;
            _height = size.Y;
        }

        public VectorRectangle(Rectangle rectangle) {
            _x = rectangle.X;
            _y = rectangle.Y;

            _width = rectangle.Width;
            _height = rectangle.Height;
        }

        public float X { get => _x; set => _x = value; }
        public float Y { get => _y; set => _y = value; }

        public float Width { get => _width; set => _width = value; }
        public float Height { get => _height; set => _height = value; }

        public Vector2 Size {
            get => new(_width,_height);
            set {
                _width = value.X;
                _height = value.Y;
            }
        }

        public Vector2 Location {
            get => new(_x,_y);
            set {
                _x = value.X;
                _y = value.Y;
            }
        }

        public Vector2 Center => new(_x + _width * 0.5f,_y + _height * 0.5f);

        public float Top => _y;
        public float Bottom => _y + _height;

        public float Left => _x;
        public float Right => _x + _width;

        public Vector2 TopLeft => new(_x,_y);
        public Vector2 BottomRight => new(_x+_width,_y+_height);

        public static readonly VectorRectangle Zero = new(0,0,0,0);
        public static readonly VectorRectangle One = new(0,0,1,1);
        public static readonly VectorRectangle Empty = new(Vector2.Zero,Vector2.Zero);

        public bool Contains(Point point) {
            return _x <= point.X && point.X < _x + _width && _y <= point.Y && point.Y < _y + _height;
        }

        public bool Contains(Vector2 vector) {
            return _x <= vector.X && vector.X < _x + _width && _y <= vector.Y && vector.Y < _y + _height;
        }

        public bool Contains(int x,int y) {
            return _x <= x && x < _x + _width && _y <= y && y < _y + _height;
        }

        public bool Contains(float x,float y) {
            return _x <= x && x < _x + _width && _y <= y && y < _y + _height;
        }

        public static explicit operator Rectangle(VectorRectangle vectorRectangle) {
            return new Rectangle((int)vectorRectangle._x,(int)vectorRectangle._y,(int)vectorRectangle._width,(int)vectorRectangle._height);
        }

        public Rectangle ToRectangle() {
            return new Rectangle((int)_x,(int)_y,(int)_width,(int)_height);
        }

        public override int GetHashCode() {
            return HashCode.Combine(_x,_y,_width,_height);
        }

        public override bool Equals(object obj) {
            return obj is VectorRectangle other && Equals(other);
        }

        public bool Equals(VectorRectangle vectorRectangle) {
            return _x == vectorRectangle.X &&
                   _y == vectorRectangle.Y &&
                   _width == vectorRectangle.Width &&
                   _height == vectorRectangle.Height;
        }

        public static bool operator ==(VectorRectangle a,VectorRectangle b) {
            return a.Equals(b);
        }
        public static bool operator !=(VectorRectangle a,VectorRectangle b) {
            return !a.Equals(b);
        }
    }
}
