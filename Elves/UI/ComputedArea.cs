using TwelveEngine;

namespace Elves.UI {
    public readonly struct ComputedArea {
        /// <summary>
        /// Floating point rectangle in screen space.
        /// </summary>
        public readonly VectorRectangle Destination;
        
        /// <summary>
        /// Rotation (in degress).
        /// </summary>
        public readonly float Rotation;

        public ComputedArea() {
            Destination = VectorRectangle.Empty;
            Rotation = 0;
        }

        public ComputedArea(VectorRectangle area,float rotation) {
            Destination = area;
            Rotation = rotation;
        }

        public static readonly ComputedArea Empty = new();
    }
}
