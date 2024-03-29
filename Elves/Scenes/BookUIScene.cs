﻿using TwelveEngine.Shell;
using TwelveEngine.UI.Book;

namespace Elves.Scenes.SaveSelect {
    public abstract class BookUIScene:InputGameState {

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

        public BookUIScene() {
            Name = "UI Scene";
            OnRender.Add(RenderUI);
            OnUpdate.Add(UpdateUI);
        }

        private void UpdateUI() {
            if(UI is null) {
                return;
            }
            UI.Update();
            CustomCursor.State = UI.CursorState;
        }

        private void RenderUI() => UI?.Render(SpriteBatch);
    }
}
