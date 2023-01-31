namespace TwelveEngine.UI.Book {
    public readonly struct ComputedArea {
        /// <summary>
        /// Floating point rectangle in screen space.
        /// </summary>
        public readonly FloatRectangle Destination;
        
        /// <summary>
        /// Rotation (in degress).
        /// </summary>
        public readonly float Rotation;

        public ComputedArea() {
            Destination = FloatRectangle.Empty;
            Rotation = 0;
        }

        public ComputedArea(FloatRectangle area,float rotation) {
            Destination = area;
            Rotation = rotation;
        }

        public static readonly ComputedArea Empty = new();
    }
}
