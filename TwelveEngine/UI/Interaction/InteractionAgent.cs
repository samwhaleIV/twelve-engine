using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TwelveEngine.Shell;
using TwelveEngine.Input;

namespace TwelveEngine.UI.Interaction {
    public abstract class InteractionAgent<TElement> where TElement:InteractionElement<TElement> {

        /// <summary>
        /// A proprety that determines if the last input event was from the mouse or a keyboard/gamepad. True if from mouse, false is from keyboard/gamepad.
        /// </summary>
        private bool LastEventWasFromMouse => GetLastEventWasFromMouse();
        private bool IsTransitioning => GetContextTransitioning();

        protected abstract bool GetLastEventWasFromMouse();
        protected abstract bool GetContextTransitioning();
     
        protected abstract TimeSpan GetCurrentTime();
        public TimeSpan Now => GetCurrentTime();

        protected event Action OnBackButtonPressed;

        private bool _keyboardIsPressingElement = false;
        private TElement _selectedElement = null, _pressedElement = null, _hiddenMouseHoverElement = null;

        protected abstract IEnumerable<TElement> GetElements();

        /// <summary>
        /// Used to override <c>Page.DefaultFocusElement</c> (if it exists).
        /// </summary>
        private TElement _lastSelectedElement = null;

        protected TElement DefaultFocusElement { get; set; } 

        private TElement GetLastSelectedOrDefault() {
            return _lastSelectedElement ?? DefaultFocusElement;
        }

        /// <summary>
        /// Current selected element. Selection status is guaranteed to be exclusive to one element from the element pool.
        /// </summary>
        public TElement SelectedElement {
            get => _selectedElement;
            private set {
                if(IsTransitioning || _selectedElement == value) {
                    return;
                }
                _selectedElement?.UpdateInteractionState(InteractionStateChange.ClearSelected);
                _selectedElement = value;
                if(value is not null) {
                    _lastSelectedElement = value;
                }
                value?.UpdateInteractionState(InteractionStateChange.SetSelected);
            }
        }

        protected bool TrySetDefaultElement() {
            if(DefaultFocusElement is null || LastEventWasFromMouse) {
                return false;
            }
            SelectedElement = GetLastSelectedOrDefault();
            return true;
        }

        /// <summary>
        /// Current pressed element. I.e. key press or mouse down with valid selected element. Pressed status is guaranteed to be exclusive to one element from the element pool.
        /// </summary>
        public TElement PressedElement {
            get => _pressedElement;
            private set {
                if(_pressedElement == value || IsTransitioning || value is not null && value.InputPaused) {
                    /* Do not set a new pressed element if it is waiting for an animation during a page */
                    return;
                }
                _pressedElement?.UpdateInteractionState(InteractionStateChange.ClearPressed);
                _pressedElement = value;
                value?.UpdateInteractionState(InteractionStateChange.SetPressed);
            }
        }

        /// <summary>
        /// Compute the true mouse hover element (no visual changes) and set the selected element to it if the current input state allows for it.
        /// </summary>
        /// <param name="location">Mouse location in viewport coordinates.</param>
        private InputEventResponse UpdateHoveredElement(Point location) {
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

            /* Yes, this one is a little dense... but realtime, multi-input UI logic is complex, okay? */
            if(!LastEventWasFromMouse || (SelectedElement is not null || hoverElement is null) && PressedElement is not null) {
                return InputEventResponse.NoChange;
            }

            if(SelectedElement == hoverElement) {
                return InputEventResponse.NoChange;
            }

            SelectedElement = hoverElement;
            return InputEventResponse.Success;
        }

        /// <summary>
        /// Clear the interaction state when you modify a page's UI but do not set a new page.
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

            /* Strictly speaking this doesn't need to be cleared but we may as well drop the reference for internal consistency. */
            DefaultFocusElement = null;
        }

        private InputEventResponse MouseDown() {
            if(PressedElement is not null || SelectedElement is null) {
                return InputEventResponse.NoChange;
            }
            if(_hiddenMouseHoverElement == null) {
                SelectedElement = null;
                return InputEventResponse.NoChange;
            }
            if(SelectedElement != _hiddenMouseHoverElement) {
                SelectedElement = _hiddenMouseHoverElement;
            }
            PressedElement = SelectedElement;
            return InputEventResponse.Success;
        }

        /// <summary>
        /// A button, such as the enter button, has been pressed. Fire once per keystroke.
        /// </summary>
        private InputEventResponse AcceptDown() {
            if(PressedElement is not null) {
                return InputEventResponse.NoChange;
            }
            SelectedElement ??= GetLastSelectedOrDefault();
            if(SelectedElement is null) {
                return InputEventResponse.NoChange;
            }
            PressedElement = SelectedElement;
            _keyboardIsPressingElement = true;
            return InputEventResponse.Success;
        }

        /// <summary>
        /// A button, bound to a direction, has been pressed. Fire once per keystroke.
        /// </summary>
        /// <param name="direction">The direction representing the button that was pressed.</param>
        private InputEventResponse DirectionDown(Direction direction) {
            if(PressedElement is not null || direction == Direction.None) {
                return InputEventResponse.NoChange;
            }
            if(SelectedElement is null) {
                SelectedElement = GetLastSelectedOrDefault();
                return InputEventResponse.Success;
            }
            int uiDirection = GetUIDirection(direction);
            TElement newElement = null;
            if(uiDirection < 0) {
                newElement = SelectedElement.PreviousElement;
            } else if(uiDirection > 0) {
                newElement = SelectedElement.NextElement;
            } else if(SelectedElement is null) {
                /* This should be impossible, but might as well cover our ass if a "None" direction is ever added */
                newElement = GetLastSelectedOrDefault();
            }
            if(newElement is null) {
                return InputEventResponse.NoChange;
            }
            SelectedElement = newElement;
            return InputEventResponse.Success;
        }

        /// <summary>
        /// A button, such as enter, has been released. Fire after <c>AcceptDown</c>.
        /// </summary>
        private InputEventResponse AcceptUp() {
            if(PressedElement is null) {
                return InputEventResponse.NoChange;
            }
            PressedElement.Activate();
            PressedElement = null;
            _keyboardIsPressingElement = false;
            return InputEventResponse.Success;
        }

        /// <summary>
        /// Fire when the mouse button has been released.
        /// </summary>
        private InputEventResponse MouseUp() {
            if(_keyboardIsPressingElement || PressedElement is null) {
                return InputEventResponse.NoChange;
            }
            if(_hiddenMouseHoverElement == PressedElement) {
                PressedElement.Activate();
            }
            PressedElement = null;
            _hiddenMouseHoverElement = null;
            return InputEventResponse.Success;
        }

        /// <summary>
        /// A way to implement a keyboard/gamepad back button. Not all pages need to have a back method.
        /// </summary>
        private InputEventResponse CancelDown() {
            if(PressedElement is not null) {
                return InputEventResponse.NoChange;
            }
            if(OnBackButtonPressed is null) {
                return InputEventResponse.NoChange;
            }
            OnBackButtonPressed.Invoke();
            return InputEventResponse.Success;
        }

        /// <summary>
        /// Central routing method for all interaction. See <see cref="InputEventType">InputEventType</see> for events.
        /// </summary>
        /// <param name="inputEvent">A valuable diagnostic value, but not particularly useful for UI applications themselves.</param>
        /// <returns></returns>
        public InputEventResponse SendEvent(InputEvent inputEvent) => inputEvent.Type switch {
            InputEventType.MouseUpdate => UpdateHoveredElement(inputEvent.MousePosition),
            InputEventType.MousePressed => MouseDown(),
            InputEventType.MouseReleased => MouseUp(),
            InputEventType.DirectionImpulse => DirectionDown(inputEvent.Direction),
            InputEventType.AcceptPressed => AcceptDown(),
            InputEventType.AcceptReleased => AcceptUp(),
            InputEventType.BackButtonActivated => CancelDown(),
            _ => InputEventResponse.UnsupportedEventType
        };

        /// <summary>
        /// A visual indication of the UI interaction state.
        /// </summary>
        /// <returns>The cursor state. <c>CursorState.Default</c> if the last event was from a keyboard/gamepad.</returns>
        private CursorState GetCursorState() {
            if(!LastEventWasFromMouse || _keyboardIsPressingElement) {
                return CursorState.Default;
            } else if(PressedElement is not null) {
                return PressedElement == _hiddenMouseHoverElement ? CursorState.Pressed : CursorState.Default;
            } else if(SelectedElement is not null) {
                /* Don't show interaction cursor state if the element is waiting for its animation to finish. */
                return SelectedElement == _hiddenMouseHoverElement && !SelectedElement.InputPaused ? CursorState.Interact : CursorState.Default;
            } else {
                return _hiddenMouseHoverElement is not null ? CursorState.Interact : CursorState.Default;
            }
        }

        public CursorState CursorState => GetCursorState();

        public static int GetUIDirection(Direction direction) => direction switch {
            Direction.Left => -1,
            Direction.Right => 1,
            Direction.Up => -1,
            Direction.Down => 1,
            _ => 0 /* Don't worry about zero, he's just chillin'. */
        };
    }
}
