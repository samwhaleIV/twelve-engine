using TwelveEngine.Input;
using TwelveEngine.Shell;

namespace TwelveEngine.UI {
    public abstract class InteractionAgent<TElement> where TElement:InteractionElement<TElement> {

        /// <summary>
        /// See <see cref="GetLastEventWasFromMouse"/>
        /// </summary>
        private bool LastEventWasFromMouse => GetLastEventWasFromMouse();

        /// <summary>
        /// See <see cref="GetContextTransitioning"/>
        /// </summary>
        protected bool IsTransitioning => GetContextTransitioning();

        /// <summary>
        /// Indicate if the most recent event was from a mouse or keyboard/gamepad.
        /// </summary>
        /// <returns>True if the last input event was cursor related, false if it was another type of impulse based device.</returns>
        protected virtual bool GetLastEventWasFromMouse() => InputStateCache.LastInputEventWasFromMouse;

        /// <summary>
        /// Context dependent meaning. Typically, if a layout is animating to an alternate layout or the UI is part of a higher order transitioning state.<br/>
        /// Used to prevent inputs and manage proper elemental selection state.
        /// </summary>
        /// <returns>True if transitioning, false if not transitioning.</returns>
        protected abstract bool GetContextTransitioning();

        /// <summary>
        /// If left click or right click is currently pressed.
        /// </summary>
        /// <returns>True if a mouse button is down, false if all are released.</returns>
        protected abstract bool GetMouseIsCapturing();

        private bool AnyMouseButtonIsPressed => GetMouseIsCapturing();

        /// <summary>
        /// Allow elements and super classes to reference current, total elapsed time.
        /// </summary>
        /// <returns>Current, total time.</returns>
        protected abstract TimeSpan GetCurrentTime(); /* Careful not to return a circular reference to this getter method. */
        public TimeSpan Now => GetCurrentTime();

        /// <summary>
        /// Activated when the back button is pressed. E.g., the escape button. Useful for pagination.
        /// </summary>
        /// <returns>A value indicating if the back button request was accepted.</returns>
        protected abstract bool BackButtonPressed();

        private bool _keyboardIsPressingElement = false;
        private TElement _selectedElement = null, _pressedElement = null;

        /// <summary>
        /// The element that is under the mouse cursor or null. This value is always updated, even if a keyboard/gamepad focus state is overriding it.
        /// </summary>
        private TElement _hiddenMouseHoverElement = null;

        protected abstract IEnumerable<TElement> GetElements();

        /// <summary>
        /// Used to override <see cref="DefaultFocusElement"/> (if is not null).
        /// </summary>
        private TElement _lastSelectedElement = null;

        /// <summary>
        /// The selection state the last time that button focus was shifted with impulse events.
        /// </summary>
        private ButtonFocusNavState<TElement> _historicalButtonFocusState = ButtonFocusNavState<TElement>.None;

        /// <summary>
        /// The default focus element for focus, impulse-based interaction. <br/>
        /// Activated internally, but also call <see cref="TrySetDefaultElement"/> when appropriate.
        /// </summary>
        public TElement DefaultFocusElement { get; set; } 

        private TElement GetLastSelectedOrDefault() {
            return _lastSelectedElement ?? DefaultFocusElement;
        }

        /// <summary>
        /// Current selected element. Selection status is guaranteed to be exclusive to one element from the element pool.
        /// </summary>
        public TElement SelectedElement {
            get => _selectedElement;
            private set {
                if(IsTransitioning || _selectedElement == value || AnyMouseButtonIsPressed) {
                    return;
                }
                _selectedElement?.UpdateInteractionState(InteractionStateChange.ClearSelected);

                if(value is not null) {
                    _lastSelectedElement = value;
                }

                _selectedElement = value;
                value?.UpdateInteractionState(InteractionStateChange.SetSelected);
            }
        }

        public void FocusDefault() {
            if(DefaultFocusElement is null || LastEventWasFromMouse || AnyMouseButtonIsPressed) {
                return;
            }
            SelectedElement = GetLastSelectedOrDefault();
            return;
        }

        /// <summary>
        /// Current pressed element. I.e. key press or mouse down with valid selected element.
        /// Pressed status is guaranteed to be exclusive to one element from the element pool.
        /// </summary>
        public TElement PressedElement {
            get => _pressedElement;
            private set {
                if(_pressedElement == value || IsTransitioning || value is not null && value.InputIsPaused) {
                    /* Do not set a new pressed element if it is waiting for an animation during a page */
                    return;
                }
                _pressedElement?.UpdateInteractionState(InteractionStateChange.ClearPressed);
                _pressedElement = value;
                value?.UpdateInteractionState(InteractionStateChange.SetPressed);
            }
        }

        /// <summary>
        /// Compute the true mouse hover element (no visual changes) and set the selected element to it if the current input state/mode allows for it.
        /// </summary>
        /// <param name="location">Mouse location in viewport coordinates.</param>
        private void UpdateHoveredElement(Point location) {
            TElement hoverElement = null;
            /* O(n)! Wildcard, bitches! */
            foreach(var element in GetElements()) {
                if(!element.CanInteract || !element.GetScreenArea().Contains(location)) {
                    continue;
                }
                hoverElement = element;
                break;
            }
            _hiddenMouseHoverElement = hoverElement;

            /* Yes, this one is a little dense... but realtime, multi-input UI logic is complex, okay?
             * But seriously, after a few refactors, I have no idea what's happening here. This boolean logic is black magic. */
            if(!LastEventWasFromMouse || (SelectedElement is not null || hoverElement is null) && PressedElement is not null || SelectedElement == hoverElement) {
                return;
            }

            SelectedElement = hoverElement;
        }

        /// <summary>
        /// Reset the interaction state. Call for significant UI layout changes.
        /// </summary>
        public void ResetInteractionState(TElement newSelectedElement = null) {
            if(!LastEventWasFromMouse) {
                SelectedElement = newSelectedElement;
            } else {
                SelectedElement = null;
            }
            PressedElement = null;
            _hiddenMouseHoverElement = null;
            _lastSelectedElement = newSelectedElement;

            _historicalButtonFocusState = ButtonFocusNavState<TElement>.None;
            DefaultFocusElement = null;
        }

        private void MouseDown() {
            if(PressedElement is not null || SelectedElement is null) {
                return;
            }
            if(_hiddenMouseHoverElement == null) {
                SelectedElement = null;
                return;
            }
            if(SelectedElement != _hiddenMouseHoverElement) {
                SelectedElement = _hiddenMouseHoverElement;
            }
            PressedElement = SelectedElement;
        }

        /// <summary>
        /// A button, such as the enter button, has been pressed. Fire once per keystroke.
        /// </summary>
        private void AcceptDown() {
            if(PressedElement is not null || AnyMouseButtonIsPressed) {
                return;
            }
            SelectedElement ??= GetLastSelectedOrDefault();
            if(SelectedElement is null) {
                return;
            }
            PressedElement = SelectedElement;
            _keyboardIsPressingElement = true;
        }

        /* If performance mattered, you could do this with arithmetic and setting enum values to specific integers */
        private static bool DirectionIsOpposite(Direction a,Direction b) => (a, b) switch {
            (Direction.Up, Direction.Down) => true,
            (Direction.Left, Direction.Right) => true,
            (Direction.Right, Direction.Left) => true,
            (Direction.Down, Direction.Up) => true,
            _ => false
        };

        private bool UseHistoricalFocusShift(FocusSet<TElement> focusSet,Direction direction) {
            return focusSet.IsIndeterminate(direction) &&
                _historicalButtonFocusState.CurrentFocusElement == SelectedElement &&
                DirectionIsOpposite(direction,_historicalButtonFocusState.Direction);
        }

        private void SetButtonFocus(TElement newElement,Direction direction) {
            var oldSelectedElement = SelectedElement;
            SelectedElement = newElement;
            _historicalButtonFocusState = new(oldSelectedElement,newElement,direction);
        }

        /// <summary>
        /// A button, bound to a direction, has been pressed. Fire once per keystroke.
        /// </summary>
        /// <param name="direction">The direction representing the button that was pressed.</param>
        private void DirectionDown(Direction direction,bool rollover = false) {
            if(direction == Direction.None || PressedElement is not null || AnyMouseButtonIsPressed) {
                return;
            }

            if(SelectedElement is null) {
                SelectedElement = GetLastSelectedOrDefault();
                return;
            }

            FocusSet<TElement> focusSet = SelectedElement.FocusSet;

            /* A solution for handling mismatched column/row sizes and using directional navigation */
            if(UseHistoricalFocusShift(focusSet,direction)) {
                SetButtonFocus(_historicalButtonFocusState.PreviousFocusElement,direction);
                return;
            }

            TElement newElement = direction switch {
                Direction.Left => focusSet.Left,
                Direction.Right => focusSet.Right ?? (rollover ? DefaultFocusElement : null),
                Direction.Up => focusSet.Up,
                Direction.Down => focusSet.Down,
                Direction.None => GetLastSelectedOrDefault(),
                _ => null
            };

            if(newElement is null || newElement == SelectedElement) {
                return;
            }
            SetButtonFocus(newElement,direction);
        }

        private static bool TryActivateElement(TElement element) {
            if(!element.CanInteract || element.InputIsPaused  || element.EndPoint is null) {
                return false;
            }
            element.EndPoint.Activate();
            return true;
        }

        /// <summary>
        /// A button, such as enter, has been released. Fire after <c>AcceptDown</c>.
        /// </summary>
        private void AcceptUp() {
            if(PressedElement is null || AnyMouseButtonIsPressed) {
                return;
            }
            TryActivateElement(PressedElement);
            PressedElement = null;
            _keyboardIsPressingElement = false;
        }

        /// <summary>
        /// Fire when the mouse button has been released.
        /// </summary>
        private void MouseUp() {
            if(_keyboardIsPressingElement || PressedElement is null) {
                return;
            }
            if(_hiddenMouseHoverElement == PressedElement) {
                TryActivateElement(PressedElement);
            }
            PressedElement = null;
        }

        /// <summary>
        /// A way to implement a keyboard/gamepad back button. Not all pages need to have a back method.
        /// </summary>
        private void CancelDown() {
            if(PressedElement is not null || AnyMouseButtonIsPressed) {
                return;
            }
            BackButtonPressed();
        }

        private void DirectionDown(Direction direction) => DirectionDown(direction,rollover: false);
        private void FocusDown() => DirectionDown(Direction.Right,rollover: true);

        /// <summary>
        /// Bind generic routers to this interaction agent.
        /// </summary>
        /// <param name="inputGameState">The state to obtain the input routers from.</param>
        public void BindInputEvents(InputGameState inputGameState) {
            var mouse = inputGameState.Mouse.Router;
            var impulse = inputGameState.Impulse.Router;

            mouse.OnPress += MouseDown;
            mouse.OnRelease += MouseUp;
            mouse.OnUpdate += UpdateHoveredElement;

            impulse.OnAcceptDown += AcceptDown;
            impulse.OnAcceptUp += AcceptUp;

            impulse.OnCancelDown += CancelDown;
            impulse.OnDirectionDown += DirectionDown;
            impulse.OnFocusDown += FocusDown;
        }

        public void UnbindInputEvents(InputGameState inputGameState) {
            var mouse = inputGameState.Mouse.Router;
            var impulse = inputGameState.Impulse.Router;

            mouse.OnPress -= MouseDown;
            mouse.OnRelease -= MouseUp;
            mouse.OnUpdate -= UpdateHoveredElement;

            impulse.OnAcceptDown -= AcceptDown;
            impulse.OnAcceptUp -= AcceptUp;

            impulse.OnCancelDown -= CancelDown;
            impulse.OnDirectionDown -= DirectionDown;
            impulse.OnFocusDown -= FocusDown;
        }

        /// <summary>
        /// A visual indication of the UI interaction state.
        /// </summary>
        /// <returns>The cursor state. <c>CursorState.Default</c> if the last event was from a keyboard/gamepad.</returns>
        private CursorState GetCursorState() {
            if(IsTransitioning || !LastEventWasFromMouse || _keyboardIsPressingElement) {
                return CursorState.Default;
            } else if(PressedElement is not null) {
                return PressedElement == _hiddenMouseHoverElement ? CursorState.Pressed : CursorState.Default;
            } else if(SelectedElement is not null) {
                /* Don't show interaction cursor state if the element is waiting for its animation to finish. */
                return SelectedElement == _hiddenMouseHoverElement && !SelectedElement.InputIsPaused ? CursorState.Interact : CursorState.Default;
            } else {
                return (_hiddenMouseHoverElement is not null && !AnyMouseButtonIsPressed) ? CursorState.Interact : CursorState.Default;
            }
        }

        public CursorState CursorState => GetCursorState();
    }
}
