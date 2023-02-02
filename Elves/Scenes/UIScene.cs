using TwelveEngine.Shell;
using TwelveEngine.UI.Book;

namespace Elves.Scenes.SaveSelect {
    public abstract class UIScene:Scene {

        private SpriteBook _book;
        public SpriteBook UI {
            get => _book;
            set {
                if(value == _book) {
                    return;
                }
                _book?.UnbindInputEvents(this);
                _book = value;
                value?.BindInputEvents(this);
            }
        }

        public UIScene() {
            Name = "UI Scene";
            OnRender += UIScene_OnRender;
            OnUpdate += UIScene_OnUpdate;
        }

        private void UIScene_OnUpdate() {
            if(UI is null) {
                return;
            }
            UI.Update(Now,new(Viewport));
            CustomCursor.State = UI.CursorState;
        }

        private void UIScene_OnRender() => UI?.Render(SpriteBatch);
    }
}
