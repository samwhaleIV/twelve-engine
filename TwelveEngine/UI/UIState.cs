using Microsoft.Xna.Framework;
using TwelveEngine.Shell;
using TwelveEngine.Shell.Input;
using TwelveEngine.UI.Elements;

namespace TwelveEngine.UI {
    public class UIState {

        private readonly GameManager game;
        internal GameManager Game => game;

        private readonly RootNode rootNode;

        private Point getViewportSize() {
            return game.Viewport.Bounds.Size;
        }

        private RenderCache renderCache;

        public void UpdateCache() {
            renderCache = RenderElementSorter.GenerateCache(rootNode);
        }

        private readonly InteractionState interactionState;

        public void DropFocus() => interactionState.DropFocus();

        public UIState(GameManager game) {
            rootNode = new RootNode(getViewportSize);
            this.game = game;
            interactionState = new InteractionState(() => renderCache);
        }

        public IMouseTarget MouseTarget => interactionState;

        public void Load() {
            UpdateCache();
            renderCache.Load(game);
        }
        public void Unload() => renderCache.Unload();

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

        public void Render(GameTime gameTime) {
            renderCache.Render(game.SpriteBatch,gameTime);
        }

        public void PreRender(GameTime gameTime) {
            renderCache.PreRender(gameTime);
        }

        public void Update(GameTime gameTime) {
            rootNode.Update();
            renderCache.Update(gameTime);
        }
    }
}
