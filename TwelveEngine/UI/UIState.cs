using System;
using Microsoft.Xna.Framework;
using TwelveEngine.UI.Elements;

namespace TwelveEngine.UI {
    public class UIState {

        private readonly GameManager game;
        internal GameManager Game => game;

        private readonly RootNode rootNode;

        private Point getViewportSize() {
            return game.GraphicsDevice.Viewport.Bounds.Size;
        }

        private RenderCache renderCache;

        public void UpdateCache() {
            renderCache = RenderElementSorter.GenerateCache(rootNode);
        }

        private InteractionState interactionState;

        public UIState(GameManager game) {
            rootNode = new RootNode(getViewportSize);
            this.game = game;
        }

        private void addMouseHandlers() {
            interactionState = new InteractionState(() => renderCache);
            var mouse = game.MouseHandler;
            mouse.OnMouseDown += interactionState.MouseDown;
            mouse.OnMouseUp += interactionState.MouseUp;
            mouse.OnMouseMove += interactionState.MouseMove;
            mouse.OnMouseScroll += interactionState.Scroll;
        }

        private void removeMouseHandlers() {
            var mouse = game.MouseHandler;
            mouse.OnMouseDown -= interactionState.MouseDown;
            mouse.OnMouseUp -= interactionState.MouseUp;
            mouse.OnMouseMove -= interactionState.MouseMove;
            mouse.OnMouseScroll -= interactionState.Scroll;
        }

        public void Load() {
            UpdateCache();
            renderCache.Load(game);
            addMouseHandlers();
        }
        public void Unload() {
            removeMouseHandlers();
            renderCache.Unload();
        }

        public int Width => rootNode.ComputedWidth;
        public int Height => rootNode.ComputedHeight;

        public void AddChild(params Element[] children) {
            rootNode.AddChild(children);
        }

        private void startLayoutRecurse(Element element) {
            element.ForceLayout();
            foreach(var child in element.Children) {
                startLayoutRecurse(child);
            }
        }
        public void StartLayout() => startLayoutRecurse(rootNode);

        internal void Render(GameTime gameTime) {
            renderCache.RenderElements(game.SpriteBatch,gameTime);
        }

        internal void PreRender(GameTime gameTime) {
            renderCache.PreRenderElements(gameTime);
        }

        internal void Update(GameTime gameTime) {
            rootNode.Update();
            renderCache.UpdateElements(gameTime);
        }
    }
}
