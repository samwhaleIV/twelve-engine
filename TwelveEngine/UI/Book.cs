using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TwelveEngine.Input;
using TwelveEngine.Shell;

namespace TwelveEngine.UI {
    public abstract class Book<TElement> where TElement:Element {

        public static readonly TimeSpan DefaultAnimationDuration = TimeSpan.FromMilliseconds(150);
        public static readonly TimeSpan DefaultTransitionDuration = TimeSpan.FromMilliseconds(400);

        protected readonly List<TElement> Elements = new();

        private Page<TElement> page = null;
        public TimeSpan TransitionDuration { get; set; } = DefaultTransitionDuration;
        private readonly AnimationInterpolator pageTransitionAnimator = new(DefaultAnimationDuration);

        private bool unlockedElements = false;

        public void SetPage(Page<TElement> newPage,TimeSpan now) {
            if(newPage is null) {
                throw new ArgumentNullException(nameof(newPage));
            }
            if(page == newPage) {
                throw new InvalidOperationException("Cannot set page to the page that is already active!");
            }

            pageTransitionAnimator.Duration = TransitionDuration;

            if(page is not null) {
                LockElementsForTransition(now);
                pageTransitionAnimator.Reset(now);

                page.SetTime(now);
                page.SetTransitionDuration(TransitionDuration);
                page.Close();
            } else {
                pageTransitionAnimator.Finish();
            }

            page = newPage;
            page.SetTime(now);
            page.SetTransitionDuration(TransitionDuration);
            page.Open();

        }

        public virtual TElement AddElement(TElement element) {
            Elements.Add(element);
            return element;
        }

        private void UnlockPageControls() {
            foreach(var element in Elements) {
                element.UnlockKeyAnimation();
            }
            if(page.DefaultFocusElement is null) {
                Logger.WriteLine($"UI page \"{page.Name}\" has opened without a default keyboard focus element!");
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
            page?.SetTime(now);
            page?.Update(viewport);
            foreach(var element in Elements) {
                element.Update(now,viewport);
            }
        }

        protected void RenderElements() {
            foreach(var element in Elements) {
                RenderElement(element);
            }
        }

        private void LockElementsForTransition(TimeSpan now) {
            foreach(var element in Elements) {
                element.KeyAnimation(now,TransitionDuration);
                element.LockKeyAnimation();
                element.Scale = 0;
                element.Flags = ElementFlags.None;
                element.ClearKeyFocus();
            }
            SelectedElement = null;
            PressedElement = null;
            _hiddenMouseHoverElement = null;
            _lastSelectedElement = null;
            unlockedElements = false;
        }

        protected abstract void RenderElement(TElement element);

        private bool keyboardPressingElement = false, lastEventWasKeyboard = false;
        private Element _selectedElement = null, _pressedElement = null, _hiddenMouseHoverElement = null, _lastSelectedElement = null;

        private Element GetLastSelectedOrDefault() {
            return _lastSelectedElement is not null ? _lastSelectedElement : page?.DefaultFocusElement ?? null;
        }

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

        public void UpdateMouseLocation(Point location) {
            UpdateHoveredElement(location);
        }

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

        public void CancelDown() {
            lastEventWasKeyboard = true;
            if(PressedElement is not null) {
                return;
            }
            page?.Back();
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
