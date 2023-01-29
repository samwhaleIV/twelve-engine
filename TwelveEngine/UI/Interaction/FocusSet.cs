using TwelveEngine.Input;

namespace TwelveEngine.UI.Interaction {
    public struct FocusSet<TElement> where TElement : class {

        public TElement Up, Down, Left, Right;

        /// <summary>
        /// Qualify that a focus direction is indeterminate.<br/>
        /// I.e., the element has more than one possible destination and the focus target must be supplemented by a contextual history.<br/>
        /// A fallback element <see cref="Up"/>, <see cref="Down"/>, <see cref="Left"/>, or <see cref="Right"/> is still expected to be provided.
        /// </summary>
        public bool IndeterminateUp, IndeterminateDown, IndeterminateLeft, IndeterminateRight;

        public static readonly FocusSet<TElement> Empty = new();

        public bool IsIndeterminate(Direction direction) => direction switch {
            Direction.Up => IndeterminateUp,
            Direction.Down => IndeterminateDown,
            Direction.Left => IndeterminateLeft,
            Direction.Right => IndeterminateRight,
            _ => false
        };
    }
}
