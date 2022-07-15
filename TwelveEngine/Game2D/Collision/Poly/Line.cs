using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine.Game2D.Collision.Poly {
    public struct Line {

        public Vector2 A;
        public Vector2 B;

        public Line(Vector2 a,Vector2 b) {
            A = a;
            B = b;
        }

        public Line(float x1,float y1,float x2,float y2) {
            A = new Vector2(x1,y1);
            B = new Vector2(x2,y2);
        }

        public bool SharesPoint(Line line) {
            return A == line.A || A == line.B || B == line.A || B == line.B;
        }
    }
}
