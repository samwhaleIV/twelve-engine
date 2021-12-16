using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TwelveEngine.Input;

namespace TwelveEngine.UI {
    public class UIState {

        /* This is where the magic happens.       
         * ... Except For System.Linq; Not you. */

        private readonly GameManager game;

        public UIState(GameManager game) {
            this.game = game;
            OnLoad += UIState_OnLoad;
            OnUnload += UIState_OnUnload;
        }

        private void addMouseHandlers() {
            var mouse = game.MouseHandler;
            mouse.OnMouseDown += Mouse_OnMouseDown;
            mouse.OnMouseUp += Mouse_OnMouseUp;
            mouse.OnMouseMove += Mouse_OnMouseMove;
            mouse.OnMouseScroll += Mouse_OnMouseScroll;
        }

        private void removeMouseHandlers() {
            var mouse = game.MouseHandler;
            mouse.OnMouseDown -= Mouse_OnMouseDown;
            mouse.OnMouseUp -= Mouse_OnMouseUp;
            mouse.OnMouseMove -= Mouse_OnMouseMove;
            mouse.OnMouseScroll -= Mouse_OnMouseScroll;
        }

        private void UIState_OnUnload() {
            removeMouseHandlers();
            unloadRenderElements();
        }

        private void UIState_OnLoad() {
            loadRenderElements();
            addMouseHandlers();
        }

        protected event Action OnLoad;
        protected event Action OnUnload;

        public void Load() => OnLoad?.Invoke();
        public void Unload() => OnUnload?.Invoke();

        private readonly RootNode rootNode = new RootNode();

        public int Width => rootNode.ComputedWidth;
        public int Height => rootNode.ComputedHeight;

        private Viewport viewport => game.GraphicsDevice.Viewport;

        public void AddChild(Element child) => rootNode.AddChild(child);

        private void startLayoutRecurse(Element element) {
            element.StartLayout();
            foreach(var child in element.GetChildren()) {
                startLayoutRecurse(child);
            }
        }
        public void StartLayout() => startLayoutRecurse(rootNode);

        private sealed class RootNode:Element {
            public void Update(Viewport viewport) {
                SetSize(new Point(viewport.Width,viewport.Height));
            }

            public bool ElementOnSurface(RenderElement element) {
                return element.ScreenArea.Intersects(ComputedArea);
            }
        }

        private RenderElement[] renderCache, interactionCache, scrollCache;

        /* ------------------------------------------------------------------------------------------------------
           Tightly coupled sorting logic, this matches the rendering state to the input state. Don't fuck with it */
        private readonly SpriteSortMode sortMode = SpriteSortMode.BackToFront;
        private RenderElement[] getRenderCache(List<RenderElement> elements) {
            return elements.OrderByDescending(element => element.Depth).ToArray();
        }
        private RenderElement[] getScrollCache(List<RenderElement> elements) {
            return elements.Where(element => element.IsScrollable).Reverse().OrderBy(element => element.Depth).ToArray();
        }
        private RenderElement[] getInteractionCache(List<RenderElement> elements) {
            /* OrderByDescending is not used in order to preserve the implicit z-indexing of same layer and addChild call orders */
            return elements.Where(element => element.IsInteractable).Reverse().OrderBy(element => element.Depth).ToArray();
        }
        private void calculateDepths(List<RenderElement> elements,float maxDepth) {
            foreach(RenderElement element in elements) {
                element.Depth = 1f - element.Depth / maxDepth / 1f;
            }
        }
        /* ------------------------------------------------------------------------------------------------------ */

        private void getAllChildren(
            Element source,List<RenderElement> elements,int currentDepth,ref int maxDepth
        ) {
            currentDepth += 1;
            if(currentDepth > maxDepth) {
                maxDepth = currentDepth;
            }

            if(source is RenderElement renderElement) {
                elements.Add(renderElement);
                renderElement.Depth = currentDepth;
            }

            foreach(var child in source.GetChildren()) {
                getAllChildren(child,elements,currentDepth,ref maxDepth);
            }
        }
        private void getAllChildren(List<RenderElement> elements,out int depth) {
            int maxDepth = 1;
            foreach(var child in rootNode.GetChildren()) {
                getAllChildren(child,elements,-1,ref maxDepth);
            }
            depth = Math.Max(maxDepth,1);
        }

        private (List<RenderElement> elements,int maxDepth) getChildrenAndDepth() {
            var elements = new List<RenderElement>();
            getAllChildren(elements,out int maxDepth);
            return (elements,maxDepth);
        }

        private List<RenderElement> getChildren() => getChildrenAndDepth().elements;

        public void UpdateCaches() {
            (List<RenderElement> elements, int maxDepth) = getChildrenAndDepth();
            calculateDepths(elements,maxDepth);
            renderCache = getRenderCache(elements);
            interactionCache = getInteractionCache(elements);
            scrollCache = getScrollCache(elements);
        }

        private void loadRenderElements() {
            foreach(var element in getChildren()) element.Load(game);
        }

        private void unloadRenderElements() {
            foreach(var element in getChildren()) element.Unload();
        }

        internal void Draw(GameTime gameTime) {
            game.GraphicsDevice.Clear(Color.Black);
            game.SpriteBatch.Begin(sortMode,null,SamplerState.PointClamp);
            foreach(var renderable in renderCache) {
                if(!rootNode.ElementOnSurface(renderable)) {
                    continue;
                }
                renderable.Render(gameTime);
            }
            game.SpriteBatch.End();
        }

        internal void Update() => rootNode.Update(viewport);

        private RenderElement focusedElement = null, hoverElement = null;

        private RenderElement FindElement(int x,int y,RenderElement[] cache) {
            foreach(var element in cache) {
                if(!element.ScreenArea.Contains(x,y)) {
                    continue;
                }
                return element;
            }
            return null;
        }

        private void refreshHoverElement(int x,int y) {
            var newElement = FindElement(x,y,interactionCache);
            var oldElement = hoverElement;
            if(newElement == oldElement) {
                return;
            }
            hoverElement = newElement;
            if(oldElement != null) {
                oldElement.Hovered = false;
            }
            if(newElement != null) {
                newElement.Hovered = true;
            }
        }

        private void Mouse_OnMouseScroll(int x,int y,ScrollDirection direction) {
            var element = FindElement(x,y,scrollCache);
            if(element == null) {
                return;
            }
            element.Scroll(direction);
            refreshHoverElement(x,y);
        }

        private void Mouse_OnMouseMove(int x,int y) {
            refreshHoverElement(x,y);
            if(hoverElement == null) {
                return;
            }
            var isFocused = hoverElement == focusedElement;
            var data = new MouseMoveData(x,y,isFocused);
            hoverElement.MouseMove(data);
        }

        private void Mouse_OnMouseUp(int x,int y) {
            var element = focusedElement;
            if(element == null) {
                return;
            }
            element.Pressed = false;
            focusedElement = null;
            if(hoverElement != element) {
                return;
            }

            element.MouseUp();
            refreshHoverElement(x,y);
        }

        private void Mouse_OnMouseDown(int x,int y) {
            if(hoverElement == null) {
                return;
            }
            focusedElement = hoverElement;
            hoverElement.Pressed = true;

            hoverElement.MouseDown();
            refreshHoverElement(x,y);
        }
    }
}
