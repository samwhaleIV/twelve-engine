namespace TwelveEngine.UI {
    public abstract class FocusElement<TElement> where TElement:FocusElement<TElement> {
        public FocusSet<TElement> FocusSet { get; set; } = FocusSet<TElement>.Empty;

        public TElement PreviousFocusElement {
            set {
                var set = FocusSet;
                set.Left = value;
                set.Up = value;
                FocusSet = set;
            }
        }

        public TElement NextFocusElement {
            set {
                var set = FocusSet;
                set.Right = value;
                set.Down = value;
                FocusSet = set;
            }
        }


        public void SetKeyFocus(TElement previous,TElement next) {
            FocusSet = new FocusSet<TElement>() {
                Up = previous,
                Down = next,
                Left = previous,
                Right = next
            };
        }

        public void SetKeyFocus(TElement up,TElement down,TElement left,TElement right) {
            FocusSet = new FocusSet<TElement>() {
                Up = up,
                Down = down,
                Left = left,
                Right = right
            };
        }

        public void ClearKeyFocus() {
            FocusSet = FocusSet<TElement>.Empty;
        }

        public void LinkFocus(TElement nextFocusElement) {
            NextFocusElement = nextFocusElement;
            nextFocusElement.PreviousFocusElement = (TElement)this;
        }

        public static void LinkFocus(TElement a,TElement b) {
            a.NextFocusElement = b;
            b.PreviousFocusElement = a;
        }
    }
}
