using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TwelveEngine.Input;
using TwelveEngine.Shell;

namespace TwelveEngine.UI {
    public abstract class Book<TElement> where TElement:Element {

        public static readonly TimeSpan DefaultAnimationDuration = TimeSpan.FromMilliseconds(150);
        public static readonly TimeSpan DefaultTransitionDuration = TimeSpan.FromMilliseconds(300);

        /// <summary>
        /// Must be iterated by a parent class in order to, for example, create a rendering method. See <c>SpriteBook</c> for an example.
        /// </summary>
        protected readonly List<TElement> Elements = new();

        /// <summary>
        /// The current, active page.
        /// </summary>
        public Page<TElement> Page { get; private set; } = null;

        /// <summary>
        /// The transition duration when changing the active page. By default, <c>DefaultTransitionDuration</c>.
        /// </summary>
        public TimeSpan TransitionDuration { get; set; } = DefaultTransitionDuration;
        private readonly AnimationInterpolator pageTransitionAnimator = new(DefaultAnimationDuration);

        /// <summary>
        /// Flag controlled by <c>UnlockPageControls</c> and <c>LockElementsForTransition</c>. Checked in <c>Update</c>.
        /// </summary>
        private bool _elementsAreLocked = true;

        private bool _keyboardIsPressingElement = false, _lastEventWasFromKeyboard = false;
        private Element _selectedElement = null, _pressedElement = null, _hiddenMouseHoverElement = null;

        /// <summary>
        /// Used to override <c>Page.DefaultFocusElement</c> (if it exists).
        /// </summary>
        private Element _lastSelectedElement = null, _defaultFocusElement = null;

        /// <summary>
        /// The time that <c>Update()</c> was last invoked with.
        /// </summary>
        private TimeSpan _currentTime = TimeSpan.FromHours(-1); /* Hopefully your users aren't time travelers. */

        public void SetPage(Page<TElement> newPage) {
            if(newPage is null) {
                throw new ArgumentNullException(nameof(newPage));
            }

            pageTransitionAnimator.Duration = TransitionDuration;

            Page<TElement> oldPage = Page;

            if(oldPage is not null) {
                LockElementsForTransition(_currentTime);
                pageTransitionAnimator.Reset(_currentTime);

                oldPage.SetTime(_currentTime);
                oldPage.SetTransitionDuration(TransitionDuration);
                oldPage.Close();
            } else {
                /* This means this is the very first page, we do no want to animate a transition to it. */
                pageTransitionAnimator.Finish();
            }

            newPage.SetTime(_currentTime);
            newPage.SetTransitionDuration(TransitionDuration);

            _defaultFocusElement = newPage.Open();

            if(oldPage is null) {
                /* If this is the first page we are loading just assume the last event was from the keyboard so we can get a default focus element right away. */
                _lastEventWasFromKeyboard = true;
            }

            Page = newPage;
        }

        /// <summary>
        /// Adds an element to the element pool. Elements should not be added dynamically. Allocate your elements before setting pages. Dynamic elements will result in undefined behavior.
        /// </summary>
        /// <param name="element">The element to be added to the pool. Don't add the same element more than once.</param>
        /// <returns>The element that has been added. Provided as syntactic sugar for instantiating an element in the argument.</returns>
        public virtual TElement AddElement(TElement element) {
            Elements.Add(element);
            return element;
        }

        /// <summary>
        /// Ends the page transition period and allows interactable elements to be pressed and selected.
        /// </summary>
        private void UnlockPageControls() {
            foreach(var element in Elements) {
                element.UnlockKeyAnimation();
            }
            if(_defaultFocusElement is null) {
                Logger.WriteLine($"UI page has been opened without a default keyboard focus element!",LoggerLabel.UI);
            } else if(_lastEventWasFromKeyboard) {
                SelectedElement = GetLastSelectedOrDefault();
            }
            _elementsAreLocked = false;
        }

        public bool TransitionComplete => pageTransitionAnimator.IsFinished;

        public void Update(TimeSpan now,VectorRectangle viewport) {
            _currentTime = now;
            pageTransitionAnimator.Update(now);
            if(_elementsAreLocked && TransitionComplete) {
                UnlockPageControls();
            }
            Page?.SetTime(now);
            Page?.Update(viewport);
            foreach(var element in Elements) {
                element.Update(now,viewport);
            }
        }

        /// <summary>
        /// Called on every element when a page is closed. <c>TElement</c> is provided in a key locked state (<c>Element.LockKeyAnimation</c>) and with <c>null</c> focus directives (<c>Element.ClearKeyFocus</c>).
        /// </summary>
        /// <param name="element">The <c>TElement</c> that needs to be reset.</param>
        protected virtual void ResetElement(TElement element) {
            element.Scale = 0;
            element.Flags = ElementFlags.None;
        }

        private void LockAndResetElement(TimeSpan now,TElement element) {
            element.KeyAnimation(now,TransitionDuration);
            element.LockKeyAnimation();
            element.ClearKeyFocus();
            ResetElement(element);
        }

        /// <summary>
        /// Hides and resets all elements to a default/non-interactable state. Prevents new animation keying or modifying current selected or pressed element. Clears interaction state.
        /// </summary>
        /// <param name="now">Current total time.</param>
        private void LockElementsForTransition(TimeSpan now) {
            foreach(var element in Elements) {
                LockAndResetElement(now,element);
            }
            ResetInteractionState();
            _elementsAreLocked = true;
        }

        private Element GetLastSelectedOrDefault() {
            return _lastSelectedElement ?? _defaultFocusElement ?? null;
        }

        /// <summary>
        /// Current selected element. Selection status is guaranteed to be exclusive to one element from the element pool.
        /// </summary>
        public Element SelectedElement {
            get => _selectedElement;
            private set {
                if(!TransitionComplete || _selectedElement == value) {
                    return;
                }
                _selectedElement?.ClearSelected();
                _selectedElement = value;
                if(value is not null) {
                    _lastSelectedElement = value;
                }
                value?.SetSelected();
            }
        }

        /// <summary>
        /// Current pressed element. I.e. key press or mouse down with valid selected element. Pressed status is guaranteed to be exclusive to one element from the element pool.
        /// </summary>
        public Element PressedElement {
            get => _pressedElement;
            private set {
                if(_pressedElement == value || !TransitionComplete || value is not null && value.InputIsPausedByAnimation) {
                    /* Do not set a new pressed element if it is waiting for an animation during a page */
                    return;
                }
                _pressedElement?.ClearPressed();
                _pressedElement = value;
                value?.SetPressed();
            }
        }

        /// <summary>
        /// Clear the interaction state when you modify a page's UI but do not set a new page.
        /// </summary>
        public void ResetInteractionState(Element newSelectedElement = null) {
            if(_lastEventWasFromKeyboard) {
                SelectedElement = newSelectedElement;
            } else {
                SelectedElement = null;
            }
            PressedElement = null;
            _hiddenMouseHoverElement = null;
            _lastSelectedElement = newSelectedElement;

            /* Strictly speaking this doesn't need to be cleared but we may as well drop the reference for internal consistency. */
            _defaultFocusElement = null;
        }

        /// <summary>
        /// Compute the true mouse hover element (no visual changes) and set the selected element to it if the current input state allows for it.
        /// </summary>
        /// <param name="location">Mouse location in viewport coordinates.</param>
        private void UpdateHoveredElement(Point location) {
            Element hoverElement = null;
            /* O(n)! Wildcard, bitches! */
            foreach(var element in Elements) {
                if(!element.IsInteractable || !element.ComputedArea.Destination.Contains(location)) {
                    continue;
                }
                hoverElement = element;
                break;
            }
            _hiddenMouseHoverElement = hoverElement;

            /* Yes, this one is a little dense... but realtime, multi-input UI logic is complex, okay? */
            if(_lastEventWasFromKeyboard || (SelectedElement is not null || hoverElement is null) && PressedElement is not null) {
                return;
            }

            SelectedElement = hoverElement;
        }

        /// <summary>
        /// Call this every frame, regardless of whether or not the mouse position has changed. This allows for elements to change position and keep a valid mouse hover status.
        /// </summary>
        /// <param name="location"></param>
        public void UpdateMouseLocation(Point location) {
            UpdateHoveredElement(location);
        }

        /// <summary>
        /// Call this when the mouse position differs from the previous position.
        /// </summary>
        public void MouseMove() {
            _lastEventWasFromKeyboard = false;
        }

        public void MouseDown() {
            _lastEventWasFromKeyboard = false;
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
        public void AcceptDown() {
            _lastEventWasFromKeyboard = true;
            if(PressedElement is not null) {
                return;
            }
            SelectedElement ??= GetLastSelectedOrDefault();
            if(SelectedElement is null) {
                return;
            }
            PressedElement = SelectedElement;
            _keyboardIsPressingElement = true;
        }

        /// <summary>
        /// A button, bound to a direction, has been pressed. Fire once per keystroke.
        /// </summary>
        /// <param name="direction">The direction representing the button that was pressed.</param>
        public void DirectionDown(Direction direction) {
            _lastEventWasFromKeyboard = true;
            if(PressedElement is not null) {
                return;
            }
            if(SelectedElement is null) {
                SelectedElement = GetLastSelectedOrDefault();
                return;
            }
            int uiDirection = GetUIDirection(direction);
            Element newElement = null;
            if(uiDirection < 0) {
                newElement = SelectedElement.PreviousElement;
            } else if(uiDirection > 0) {
                newElement = SelectedElement.NextElement;
            } else if(SelectedElement is null) {
                /* This should be impossible, but might as well cover our ass if a "None" direction is ever added */
                newElement = GetLastSelectedOrDefault();
            }
            if(newElement is null) {
                return;
            }
            SelectedElement = newElement;
        }

        /// <summary>
        /// A button, such as enter, has been released. Fire after <c>AcceptDown</c>.
        /// </summary>
        public void AcceptUp() {
            _lastEventWasFromKeyboard = true;
            if(PressedElement is null) {
                return;
            }
            PressedElement.Activate(_currentTime);
            PressedElement = null;
            _keyboardIsPressingElement = false;
        }

        /// <summary>
        /// Fire when the mouse button has been released.
        /// </summary>
        public void MouseUp() {
            _lastEventWasFromKeyboard = false;
            if(_keyboardIsPressingElement || PressedElement is null) {
                return;
            }
            if(_hiddenMouseHoverElement == PressedElement) {
                PressedElement.Activate(_currentTime);
            }
            PressedElement = null;
            _hiddenMouseHoverElement = null;
        }

        /// <summary>
        /// A way to implement a keyboard/gamepad back button. Not all pages need to have a back method.
        /// </summary>
        public void CancelDown() {
            _lastEventWasFromKeyboard = true;
            if(PressedElement is not null) {
                return;
            }
            Page?.Back();
        }

        /// <summary>
        /// A visual indication of the UI interaction state.
        /// </summary>
        /// <returns>The cursor state. <c>CursorState.Default</c> if the last event was from a keyboard/gamepad.</returns>
        private CursorState GetCursorState() {
            if(_lastEventWasFromKeyboard) {
                return CursorState.Default;
            } else if(PressedElement is not null) {
                return PressedElement == _hiddenMouseHoverElement ? CursorState.Pressed : CursorState.Default;
            } else if(SelectedElement is not null) {
                /* Don't show interaction cursor state if the element is waiting for its animation to finish. */
                return SelectedElement == _hiddenMouseHoverElement && !SelectedElement.InputIsPausedByAnimation ? CursorState.Interact : CursorState.Default;
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
