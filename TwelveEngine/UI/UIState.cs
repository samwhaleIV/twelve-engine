using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TwelveEngine.UI.Elements;
using TwelveEngine.Input;

namespace TwelveEngine.UI {
    public class UIState {

        private readonly GameManager game;
        internal GameManager Game => game;

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

        public int Width => rootNode.Width;
        public int Height => rootNode.Height;

        private Viewport viewport => game.GraphicsDevice.Viewport;

        public void AddChild(Element child) => rootNode.AddChild(child);
        public void StartLayout() => rootNode.StartLayout();

        private sealed class RootNode:Element {
            public void Update(Viewport viewport) {
                SetSize(new Point(viewport.Width,viewport.Height));
            }

            public bool ElementOnSurface(RenderElement element) {
                return element.ScreenArea.Intersects(element.Area);
            }
        }

        private void getUsableElements(
            Element source,List<RenderElement> renderable,List<RenderElement> interactable = null
        ) {
            if(source is RenderElement renderElement) {
                renderable.Add(renderElement);
                if(interactable != null && renderElement.IsInteractable) {
                    interactable.Add(renderElement);
                }
            }
            foreach(var child in source.GetChildren()) {
                getUsableElements(child,renderable,interactable);
            }
        }

        private RenderElement[] renderCache = new RenderElement[0];
        private RenderElement[] interactCache = new RenderElement[0];

        public void UpdateCache() {
            rootNode.StartLayout();

            var renderable = new List<RenderElement>();
            var interactable = new List<RenderElement>();
            getUsableElements(rootNode,renderable,interactable);

            renderCache = renderable.ToArray();
            interactCache = interactable.ToArray();
        }

        private void iterateRenderElements(Action<RenderElement> action) {
            var elements = new List<RenderElement>();
            getUsableElements(rootNode,elements);
            foreach(var element in elements) {
                action.Invoke(element);
            }
        }

        private void loadElement(RenderElement element) => element.Load(game);
        private void unloadElement(RenderElement element) => element.Unload();

        private void loadRenderElements() => iterateRenderElements(loadElement);
        private void unloadRenderElements() => iterateRenderElements(unloadElement);

        internal void Draw(GameTime gameTime) {
            game.GraphicsDevice.Clear(Color.Black);
            game.SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);
            foreach(var renderable in renderCache) {
                if(!rootNode.ElementOnSurface(renderable)) {
                    continue;
                }
                renderable.Render(gameTime);
            }
            game.SpriteBatch.End();
        }

        internal void Update() => rootNode.Update(viewport);

        private RenderElement focusedElement = null;
        private RenderElement hoverElement = null;

        private RenderElement FindElement(int x,int y) {
            foreach(var interactable in interactCache) {
                if(!interactable.ScreenArea.Contains(x,y)) {
                    continue;
                }
                return interactable;
            }
            return null;
        }

        private void Mouse_OnMouseScroll(int x,int y,ScrollDirection direction) {
            if(hoverElement == null || hoverElement.Pressed) {
                return;
            }
            hoverElement?.Scroll(direction);
            refreshHoverElement(x,y);
        }

        private void refreshHoverElement(int x,int y) {
            var newElement = FindElement(x,y);
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

        private void Mouse_OnMouseMove(int x,int y) {
            refreshHoverElement(x,y);
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
            element.Click();
            refreshHoverElement(x,y);
        }

        private void Mouse_OnMouseDown(int x,int y) {
            if(hoverElement == null) {
                return;
            }
            focusedElement = hoverElement;
            hoverElement.Pressed = true;
        }
    }
}
