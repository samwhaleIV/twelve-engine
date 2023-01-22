using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TwelveEngine.Input;
using TwelveEngine.Shell;

namespace TwelveEngine.UI {
    public abstract class Book<TElement> where TElement:Element {

        public static readonly TimeSpan DefaultAnimationDuration = TimeSpan.FromMilliseconds(150);
        public static readonly TimeSpan DefaultTransitionDuration = TimeSpan.FromMilliseconds(400);

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
        private bool unlockedElements = false;

        public void SetPage(Page<TElement> newPage,TimeSpan now) {
            if(newPage is null) {
                throw new ArgumentNullException(nameof(newPage));
            }
            if(Page == newPage) {
                throw new InvalidOperationException("Cannot set page to the page that is already active!");
            }

            pageTransitionAnimator.Duration = TransitionDuration;

            if(Page is not null) {
                LockElementsForTransition(now);
                pageTransitionAnimator.Reset(now);

                Page.SetTime(now);
                Page.SetTransitionDuration(TransitionDuration);
                Page.Close();
            } else {
                /* This means this is the very first page, we do no want to animate a transition to it. */
                pageTransitionAnimator.Finish();
            }

            Page = newPage;
            Page.SetTime(now);
            Page.SetTransitionDuration(TransitionDuration);
            Page.Open();

        }

        /// <summary>
        /// Adds an element to the element pool. Elements should not be added dynamically. Allocate your elements before setting pages. Dynamic elements will result in undefined behavior.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
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
            if(Page.DefaultFocusElement is null) {
                Logger.WriteLine($"UI page \"{Page.Name}\" has opened without a default keyboard focus element!");
            } else if(lastEventWasKeyboard) {
                SelectedElement = GetLastSelectedOrDefault();
            }
            unlockedElements = true;
        }

        public bool TransitionComplete => pageTransitionAnimator.IsFinished;

        public void Update(TimeSpan now,VectorRectangle viewport) {
            pageTransitionAnimator.Update(now);
            if(!unlockedElements && TransitionComplete) {
                UnlockPageControls();
            }
            Page?.SetTime(now);
            Page?.Update(viewport);
            foreach(var element in Elements) {
                element.Update(now,viewport);
            }
        }

        private void LockAndResetElement(TimeSpan now,Element element) {
            element.KeyAnimation(now,TransitionDuration);
            element.LockKeyAnimation();
            element.Scale = 0;
            element.Flags = ElementFlags.None;
            element.ClearKeyFocus();
        }

        /// <summary>
        /// Hides and resets all elements to a default/non-interactable state. Prevents new animation keying or modifying current selected or pressed element. Clears interaction state.
        /// </summary>
        /// <param name="now">Current total time.</param>
        private void LockElementsForTransition(TimeSpan now) {
            foreach(var element in Elements) {
                LockAndResetElement(now,element);
            }
            SelectedElement = null;
            PressedElement = null;
            _hiddenMouseHoverElement = null;
            _lastSelectedElement = null;
            unlockedElements = false;
        }

        private bool keyboardPressingElement = false, lastEventWasKeyboard = false;
        private Element _selectedElement = null, _pressedElement = null, _hiddenMouseHoverElement = null;

        /// <summary>
        /// Used to override <c>Page.DefaultFocusElement</c> (if it exists).
        /// </summary>
        private Element _lastSelectedElement = null;

        private Element GetLastSelectedOrDefault() {
            return _lastSelectedElement is not null ? _lastSelectedElement : Page?.DefaultFocusElement ?? null;
        }

        /// <summary>
        /// Current selected element. Selection status is guaranteed to be exclusive to one element from the element pool.
        /// </summary>
        public Element SelectedElement {
            get => _selectedElement;
            private set {
                if(!TransitionComplete) {
                    return;
                }
                if(_selectedElement == value) {
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
                if(!TransitionComplete) {
                    return;
                }
                if(_pressedElement == value) {
                    return;
                }
                _pressedElement?.ClearPressed();
                _pressedElement = value;
                value?.SetPressed();
            }
        }

        /// <summary>
        /// Compute the hidden mouse hover element and set the selected element if the input mode allows for it.
        /// </summary>
        /// <param name="location">Mouse location in screen coordinates.</param>
        private void UpdateHoveredElement(Point location) {
            Element hoverElement = null;
            /* O(n)! Wildcard, bitches! */
            foreach(var element in Elements) {
                if(!element.Flags.HasFlag(ElementFlags.Interactable)) {
                    continue;
                }
                if(element.ComputedArea.Destination.Contains(location)) {
                    hoverElement = element;
                    break;
                }
            }
            _hiddenMouseHoverElement = hoverElement;

            if(SelectedElement is null && hoverElement is not null) {
                if(!lastEventWasKeyboard) {
                    SelectedElement = hoverElement;
                }
            } else if(!lastEventWasKeyboard && PressedElement is null) {
                SelectedElement = hoverElement;
            }
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
            lastEventWasKeyboard = false;
        }

        public void MouseDown() {
            lastEventWasKeyboard = false;
            if(PressedElement is null && SelectedElement is not null) {
                if(_hiddenMouseHoverElement == null) {
                    SelectedElement = null;
                    return;
                }
                if(SelectedElement != _hiddenMouseHoverElement) {
                    SelectedElement = _hiddenMouseHoverElement;
                }
                PressedElement = SelectedElement;
            }
        }

        public void AcceptDown() {
            lastEventWasKeyboard = true;
            if(PressedElement is not null) {
                return;
            }
            SelectedElement ??= GetLastSelectedOrDefault();
            if(SelectedElement is not null) {
                PressedElement = SelectedElement;
                keyboardPressingElement = true;
            }
        }

        public void DirectionDown(Direction direction) {
            lastEventWasKeyboard = true;
            if(PressedElement is not null) {
                return;
            }
            if(SelectedElement is null) {
                SelectedElement = GetLastSelectedOrDefault();
            } else {
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
        }

        public void AcceptUp(TimeSpan now) {
            lastEventWasKeyboard = true;
            if(PressedElement is not null) {
                PressedElement.Activate(now);
                PressedElement = null;
                keyboardPressingElement = false;
            }
        }

        public void MouseUp(TimeSpan now) {
            lastEventWasKeyboard = false;
            if(keyboardPressingElement) {
                return;
            }
            if(PressedElement is not null) {
                if(_hiddenMouseHoverElement == PressedElement) {
                    PressedElement.Activate(now);
                }
                PressedElement = null;
                _hiddenMouseHoverElement = null;
            }
        }

        /// <summary>
        /// A way to implement a keyboard/gamepad back button. Not all pages need to a back method.
        /// </summary>
        public void CancelDown() {
            lastEventWasKeyboard = true;
            if(PressedElement is not null) {
                return;
            }
            Page?.Back();
        }

        private CursorState GetCursorState() {
            if(lastEventWasKeyboard) {
                return CursorState.Default;
            } else if(PressedElement is not null) {
                return PressedElement == _hiddenMouseHoverElement ? CursorState.Pressed : CursorState.Default;
            } else if(SelectedElement is not null) {
                return SelectedElement == _hiddenMouseHoverElement ? CursorState.Interact : CursorState.Default;
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
            _ => 0 /* What the fuck is zero doing here! */
        };
    }
}
