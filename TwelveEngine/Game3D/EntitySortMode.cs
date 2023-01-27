namespace TwelveEngine.Game3D {
    public enum EntitySortMode {
        /// <summary>
        /// Entities are sorted by their IDs, oldest entities are rendered and updated first, newest entities are last.
        /// </summary>
        CreationOrder,
        /// <summary>
        /// A very haphazard, inefficient sorting method.. but it gets the job done if you have a dynamic camera.
        /// </summary>
        CameraRelative,
        /// <summary>
        /// Use this for fixed camera, 2D, or orthographic scenes.<br/>
        /// Sorted based on a camera Z value that is greater than <c>0</c> with a view matrix facing towards a Z value that is less than <c>0</c>.<br/><br/>
        /// E.g., a background image <c>(Z = 0)</c>, a camera of <c>(Z = 1)</c> (rotated towards the background), and a middleground sprite of <c>(Z = 0.5)</c>.
        /// </summary>
        CameraFixed
    };
}
