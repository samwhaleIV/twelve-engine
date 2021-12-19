using System;
using TwelveEngine.Input;

namespace TwelveEngine.UI {
    internal sealed class InteractionState {

        private Func<RenderCache> getCache;
        public InteractionState(Func<RenderCache> getCache) {
            this.getCache = getCache;
        }

        private RenderElement focusedElement = null, hoverElement = null;

        private RenderElement findElement(int x,int y,RenderElement[] cache) {
            foreach(var element in cache) {
                if(!element.ScreenArea.Contains(x,y)) {
                    continue;
                }
                return element;
            }
            return null;
        }

        private int lastX = 0, lastY = 0;

        private void refreshHoverElement(int x,int y) {
            lastX = x; lastY = y;
            var newElement = findElement(x,y,getCache().Interact);
            var oldElement = hoverElement;
            if(newElement == oldElement) {
                return;
            }
            hoverElement = newElement;
            if(oldElement != null) {
                oldElement.Hovered = false;
                oldElement.MouseLeave();
            }
            if(newElement != null) {
                newElement.Hovered = true;
            }
        }

        public void RefreshElementSelection() {
            refreshHoverElement(lastX,lastY);
        }

        public void Scroll(int x,int y,ScrollDirection direction) {
            var element = findElement(x,y,getCache().Scroll);
            if(element == null) {
                return;
            }
            element.Scroll(x,y,direction);
            refreshHoverElement(x,y);
        }

        public void MouseMove(int x,int y) {
            refreshHoverElement(x,y);
            if(hoverElement == null) {
                return;
            }
            hoverElement.MouseMove(x,y);
        }

        public void MouseUp(int x,int y) {
            var element = focusedElement;
            if(element == null) {
                return;
            }
            element.Pressed = false;
            focusedElement = null;

            element.MouseUp(x,y);
            refreshHoverElement(x,y);
        }

        public void MouseDown(int x,int y) {
            if(hoverElement == null) {
                return;
            }
            focusedElement = hoverElement;
            hoverElement.Pressed = true;

            hoverElement.MouseDown(x,y);
            refreshHoverElement(x,y);
        }

        public void DropFocus() {
            if(hoverElement == null) {
                return;
            }
            hoverElement.Hovered = false;
            hoverElement.MouseLeave();
            hoverElement = null;
        }
    }
}
