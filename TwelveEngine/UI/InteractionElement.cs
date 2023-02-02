namespace TwelveEngine.UI {
    public abstract class InteractionElement<TElement>:FocusElement<TElement> where TElement:InteractionElement<TElement> {

        /// <summary>
        /// End point for element activation routing.<br/> Supports variable return values.
        /// See <see cref="Endpoint{TReturnValue}"/>.
        /// </summary>
        protected internal Endpoint Endpoint { get; protected set; }

        private bool _inputPaused = false;

        protected virtual bool GetInputPaused() => _inputPaused;
        protected virtual void SetInputPaused(bool value) => _inputPaused = value;

        public bool InputIsPaused {
            get => GetInputPaused();
            protected set => SetInputPaused(value);
        }

        private bool _pressed = false, _selected = false;

        public abstract FloatRectangle GetScreenArea();

        /// <summary>
        /// Filtered by <c>InputPaused</c>.
        /// </summary>
        public bool Pressed => _pressed && !InputIsPaused;

        /// <summary>
        /// Filtered by <c>InputPaused</c>.
        /// </summary>
        public bool Selected => _selected && !InputIsPaused;

        public bool CanInteract { get; set; } = false;

        public void UpdateInteractionState(InteractionStateChange interactionStateChange) {
            switch(interactionStateChange) {
                case InteractionStateChange.SetSelected: _selected = true; break;
                case InteractionStateChange.ClearSelected: _selected = false; break;
                case InteractionStateChange.SetPressed: _pressed = true; break;
                case InteractionStateChange.ClearPressed: _pressed = false; break;
            }
        }
    }
}
