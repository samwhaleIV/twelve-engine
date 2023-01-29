using TwelveEngine.Input;

namespace TwelveEngine.UI {
    /// <summary>
    /// A cache used to allow opposite direction focus travel to a repeatable destination.<br/>
    /// E.g., one row has 2 values but the following only has 1 and your focus direction is alternating between row 1 and 2 (See <see cref="DirectionIsOpposite(Direction, Direction)"/>.<br/>
    /// The cache is retained even if the mouse has manipulated the cursor state by manner of checking <see cref="CurrentFocusElement"/> against <see cref="SelectedElement"/>.
    /// </summary>
    internal readonly struct FocusNavState<TElement> where TElement : class {

        public readonly TElement PreviousFocusElement, CurrentFocusElement;
        public readonly Direction Direction;

        public FocusNavState(TElement previousFocusElement, TElement currentFocusElement, Direction direction) {
            PreviousFocusElement = previousFocusElement;
            CurrentFocusElement = currentFocusElement;
            Direction = direction;
        }

        public static readonly FocusNavState<TElement> None = new(null,null,Direction.None);
    }
}
