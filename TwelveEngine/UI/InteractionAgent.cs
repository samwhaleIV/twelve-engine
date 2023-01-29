using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TwelveEngine.Shell;
using TwelveEngine.Input;

namespace TwelveEngine.UI {
    public abstract class InteractionAgent<TElement> where TElement:InteractionElement<TElement> {

        /// <summary>
        /// See <see cref="GetLastEventWasFromMouse"/>
        /// </summary>
        private bool LastEventWasFromMouse => GetLastEventWasFromMouse();

        /// <summary>
        /// See <see cref="GetContextTransitioning"/>
        /// </summary>
        private bool IsTransitioning => GetContextTransitioning();

        /// <summary>
        /// Indicate if the most recent event was from a mouse or keyboard/gamepad.
        /// </summary>
        /// <returns>True if the last input event was cursor related, false if it was another type of impulse based device.</returns>
        protected abstract bool GetLastEventWasFromMouse();

        /// <summary>
        /// Context dependent meaning. Typically, if a layout is animating to an alternate layout or the UI is part of a higher order transitioning state.<br/>
        /// Used to prevent inputs and manage proper elemental selection state.
        /// </summary>
        /// <returns>True if transitioning, false if not transitioning.</returns>
        protected abstract bool GetContextTransitioning();
     
        /// <summary>
        /// Allow elements and super classes to reference current, total elapsed time.
        /// </summary>
        /// <returns>Current, total time.</returns>
        protected abstract TimeSpan GetCurrentTime();
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
        private FocusNavState<TElement> _historicalButtonFocusState = FocusNavState<TElement>.None;

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
                if(IsTransitioning || _selectedElement == value) {
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

        public bool TrySetDefaultElement() {
            if(DefaultFocusElement is null || LastEventWasFromMouse) {
                return false;
            }
            SelectedElement = GetLastSelectedOrDefault();
            return true;
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

            /* Yes, this one is a little dense... but realtime, multi-input UI logic is complex, okay?
             * But seriously, after a few refactors, I have no idea what's happening here. This boolean logic is black magic. */
            if(!LastEventWasFromMouse || (SelectedElement is not null || hoverElement is null) && PressedElement is not null || SelectedElement == hoverElement) {
                return InputEventResponse.NoChange;
            }

            SelectedElement = hoverElement;
            return InputEventResponse.Success;
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

            _historicalButtonFocusState = FocusNavState<TElement>.None;
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
        private InputEventResponse DirectionDown(Direction direction,bool rollover = false) {
            if(PressedElement is not null || direction == Direction.None) {
                return InputEventResponse.NoChange;
            }

            if(SelectedElement is null) {
                SelectedElement = GetLastSelectedOrDefault();
                return InputEventResponse.Success;
            }

            FocusSet<TElement> focusSet = SelectedElement.FocusSet;

            /* A solution for handling mismatched column/row sizes and using directional navigation */
            if(UseHistoricalFocusShift(focusSet,direction)) {
                SetButtonFocus(_historicalButtonFocusState.PreviousFocusElement,direction);
                return InputEventResponse.Success;
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
                return InputEventResponse.NoChange;
            }
            SetButtonFocus(newElement,direction);
            return InputEventResponse.Success;
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
        private InputEventResponse AcceptUp() {
            if(PressedElement is null) {
                return InputEventResponse.NoChange;
            }
            TryActivateElement(PressedElement);
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
                TryActivateElement(PressedElement);
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
            return BackButtonPressed() ? InputEventResponse.Success : InputEventResponse.NoChange;
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
            InputEventType.DirectionImpulse => DirectionDown(inputEvent.Direction,rollover: false),
            InputEventType.AcceptPressed => AcceptDown(),
            InputEventType.AcceptReleased => AcceptUp(),
            InputEventType.BackButtonActivated => CancelDown(),
            InputEventType.FocusButtonActivated => DirectionDown(Direction.Right,rollover: true),
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
                return SelectedElement == _hiddenMouseHoverElement && !SelectedElement.InputIsPaused ? CursorState.Interact : CursorState.Default;
            } else {
                return _hiddenMouseHoverElement is not null ? CursorState.Interact : CursorState.Default;
            }
        }

        public CursorState CursorState => GetCursorState();
    }
}
