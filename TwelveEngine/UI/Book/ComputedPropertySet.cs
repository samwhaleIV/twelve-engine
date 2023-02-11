namespace TwelveEngine.UI.Book {
    public readonly struct ComputedPropertySet {
        /// <summary>
        /// Floating point rectangle in screen space.
        /// </summary>
        public readonly FloatRectangle Destination;
        
        /// <summary>
        /// Rotation (in degress).
        /// </summary>
        public readonly float Rotation;

        public readonly Color Color;

        public ComputedPropertySet() {
            Destination = FloatRectangle.Empty;
            Rotation = 0;
            Color = Color.White;
        }

        public ComputedPropertySet(FloatRectangle area,float rotation,Color color) {
            Destination = area;
            Rotation = rotation;
            Color = color;
        }

        public static readonly ComputedPropertySet Empty = new();
    }
}
