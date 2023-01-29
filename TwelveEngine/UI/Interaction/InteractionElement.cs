using System;

namespace TwelveEngine.UI.Interaction {
    public abstract class InteractionElement<TElement> where TElement:InteractionElement<TElement> {

        private bool _inputPaused = false;

        protected virtual bool GetInputPaused() => _inputPaused;
        protected virtual void SetInputPaused(bool value) => _inputPaused = value;

        public bool InputIsPaused {
            get => GetInputPaused();
            protected set => SetInputPaused(value);
        }

        private bool _pressed = false, _selected = false;

        public abstract VectorRectangle GetScreenArea();

        /// <summary>
        /// Filtered by <c>InputPaused</c>.
        /// </summary>
        public bool Pressed => _pressed && !InputIsPaused;

        /// <summary>
        /// Filtered by <c>InputPaused</c>.
        /// </summary>
        public bool Selected => _selected && !InputIsPaused;

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

        public bool CanInteract { get; set; } = false;

        public void UpdateInteractionState(InteractionStateChange interactionStateChange) {
            switch(interactionStateChange) {
                case InteractionStateChange.SetSelected: _selected = true; break;
                case InteractionStateChange.ClearSelected: _selected = false; break;
                case InteractionStateChange.SetPressed: _pressed = true; break;
                case InteractionStateChange.ClearPressed: _pressed = false; break;
            }
        }
        
        public event Action<TElement> OnActivated;

        public void Activate() {
            if(!CanInteract || InputIsPaused) {
                return;
            }
            OnActivated?.Invoke((TElement)this);
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
