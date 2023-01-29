using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine.UI.Interaction {
    public abstract class InteractionElement<TElement> where TElement:InteractionElement<TElement> {
        public bool InputPaused { get; protected set; }

        private bool _pressed = false, _selected = false;

        public abstract VectorRectangle GetScreenArea();

        /// <summary>
        /// Filtered by <c>InputPaused</c>.
        /// </summary>
        public bool Pressed => _pressed && !InputPaused;

        /// <summary>
        /// Filtered by <c>InputPaused</c>.
        /// </summary>
        public bool Selected => _selected && !InputPaused;

        public TElement PreviousElement { get; set; } = null;
        public TElement NextElement { get; set; } = null;

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
            if(!CanInteract || InputPaused) {
                return;
            }
            OnActivated?.Invoke((TElement)this);
        }

        public void SetKeyFocus(TElement previous,TElement next) {
            NextElement = next;
            PreviousElement = previous;
        }

        public void ClearKeyFocus() {
            PreviousElement = null;
            NextElement = null;
        }
    }
}
