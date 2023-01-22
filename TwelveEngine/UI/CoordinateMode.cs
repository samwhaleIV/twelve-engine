namespace TwelveEngine.UI {
    public enum CoordinateMode {
        /// <summary>
        /// No translation to the coordinate. The viewport origin is always added to coordinates.
        /// </summary>
        Absolute,

        /// <summary>
        /// Coordinate relative to the viewport provided to <c>Element.Update</c>. The viewport origin is always added to coordinates.
        /// </summary>
        Relative
    }
}
