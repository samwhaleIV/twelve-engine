using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D {
    public struct VectorRectangle {

        public Vector2 Location;
        public Vector2 Size;

        public VectorRectangle(Vector2 location,Vector2 size) {
            Location = location;
            Size = size;
        }

        public override bool Equals(object obj) => obj is VectorRectangle other && this.Equals(other);
        public bool Equals(VectorRectangle vectorRectangle) => Location == vectorRectangle.Location && Size == vectorRectangle.Size;
        public override int GetHashCode() => (Location, Size).GetHashCode();
        public static bool operator == (VectorRectangle a,VectorRectangle b) => a.Equals(b);
        public static bool operator != (VectorRectangle a,VectorRectangle b) => !(a == b);

        public static VectorRectangle Empty => new VectorRectangle(Vector2.Zero,Vector2.Zero);
    }
}
