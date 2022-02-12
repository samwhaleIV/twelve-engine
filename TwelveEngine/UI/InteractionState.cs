using Microsoft.Xna.Framework;
using System;
using TwelveEngine.Shell.Input;
using TwelveEngine.UI.Elements;

namespace TwelveEngine.UI {
    public sealed class InteractionState:IMouseTarget {

        private Func<RenderCache> getCache;

        internal InteractionState(Func<RenderCache> getCache) {
            this.getCache = getCache;
        }

        private RenderElement focusedElement = null, hoverElement = null;

        private RenderElement findElement(Point mousePosition,RenderElement[] cache) {
            foreach(var element in cache) {
                if(!element.ScreenArea.Contains(mousePosition)) {
                    continue;
                }
                return element;
            }
            return null;
        }

        private Point lastMousePosition;

        private void refreshHoverElement(Point mousePosition) {
            lastMousePosition = mousePosition;
            var newElement = findElement(mousePosition,getCache().InteractCache);
            var oldElement = hoverElement;
            if(newElement == oldElement) {
                return;
            }
            hoverElement = newElement;
            if(oldElement != null) {
                oldElement.Hovered = false;
                oldElement.MouseLeave();
            }
            if(newElement == null) {
                return;
            }
            newElement.Hovered = true;
        }

        internal void RefreshElement() {
            refreshHoverElement(lastMousePosition);
            if(hoverElement != null && hoverElement is RenderFrame frame) {
                frame.InteractionState.MouseMove(lastMousePosition - frame.ComputedArea.Location);
            }
        }

        internal void DropFocus() {
            if(hoverElement == null) {
                return;
            }
            hoverElement.Hovered = false;
            hoverElement.MouseLeave();
            hoverElement = null;
        }

        public void Scroll(Point mousePosition,ScrollDirection direction) {
            var element = findElement(mousePosition,getCache().ScrollCache);
            if(element == null) {
                return;
            }
            element.Scroll(mousePosition,direction);
            refreshHoverElement(mousePosition);
        }

        public void MouseMove(Point mousePosition) {
            refreshHoverElement(mousePosition);
            if(hoverElement == null) {
                return;
            }
            hoverElement.MouseMove(mousePosition);
        }

        public void MouseUp(Point mousePosition) {
            var element = focusedElement;
            if(element == null) {
                return;
            }
            element.Pressed = false;
            focusedElement = null;

            element.MouseUp(mousePosition);
            refreshHoverElement(mousePosition);
        }

        public void MouseDown(Point mousePosition) {
            if(hoverElement == null) {
                return;
            }
            focusedElement = hoverElement;
            hoverElement.Pressed = true;

            hoverElement.MouseDown(mousePosition);
            refreshHoverElement(mousePosition);
        }
    }
}
