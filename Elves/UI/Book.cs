using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TwelveEngine;
using TwelveEngine.Input;
using TwelveEngine.Shell;

namespace Elves.UI {
    public abstract class Book<TElement> where TElement:Element {

        protected readonly List<TElement> Elements = new();

        private Page<TElement> page = null;

        public void SetPage(Page<TElement> newPage,TimeSpan now,TimeSpan? animationDuration = null) {
            if(newPage is null) {
                throw new ArgumentNullException(nameof(newPage));
            }
            if(page == newPage) {
                throw new InvalidOperationException("Cannot set page to the page that is already active!");
            }
            page?.Close();
            page = newPage;
            HideAllElements(now,animationDuration);
            page.Open(now);
            if(page.DefaultFocusElement is null) {
                Logger.WriteLine("UI page has no default keyboard focus element!");
            } else if(lastEventWasKeyboard) {
                SelectedElement = page.DefaultFocusElement;
            }
        }

        public virtual TElement AddElement(TElement element) {
            Elements.Add(element);
            return element;
        }

        public void Update(TimeSpan now,VectorRectangle viewport) {
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

        public void SkipAnimations() {
            foreach(var element in Elements) {
                element.SkipAnimation();
            }
        }

        private void HideAllElements(TimeSpan now,TimeSpan? duration = null) {
            foreach(var element in Elements) {
                element.KeyAnimation(now,duration);
                element.Scale = 0;
                element.Flags = ElementFlags.None;
                element.ClearKeyFocus();
            }
            SelectedElement = null;
            PressedElement = null;
            _hiddenMouseHoverElement = null;
        }

        protected abstract void RenderElement(TElement element);

        private bool keyboardPressingElement = false, lastEventWasKeyboard = false;
        private Element _selectedElement = null, _pressedElement = null, _hiddenMouseHoverElement = null;

        public Element SelectedElement {
            get => _selectedElement;
            private set {
                if(_selectedElement == value) {
                    return;
                }
                _selectedElement?.SelectEnd();
                _selectedElement = value;
                value?.Select();
            }
        }
        public Element PressedElement {
            get => _pressedElement;
            private set {
                if(_pressedElement == value) {
                    return;
                }
                _pressedElement?.Depress();
                _pressedElement = value;
                value?.Press();
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
                PressedElement = SelectedElement;
            }
        }

        public void AcceptDown() {
            lastEventWasKeyboard = true;
            if(PressedElement is not null) {
                return;
            }
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
                SelectedElement = page?.DefaultFocusElement ?? null;
            } else {
                int uiDirection = GetUIDirection(direction);
                Element newElement = null;
                if(uiDirection < 0) {
                    newElement = SelectedElement.PreviousElement;
                } else if(uiDirection > 0) {
                    newElement = SelectedElement.NextElement;
                } else if(SelectedElement is null) {
                    /* This should be impossible, but might as well cover our ass if a "None" direction is ever added */
                    newElement = page?.DefaultFocusElement ?? null;
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
