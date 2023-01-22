using TwelveEngine.UI;
using TwelveEngine;

namespace Elves.Scenes.SaveSelect {
    public abstract class UIScene:Scene {

        private SpriteBook ui = null;
        protected SpriteBook UI {
            get => ui;
            set {
                if(value == ui) {
                    return;
                }
                if(ui is not null) {
                    UnbindUIEvents(ui);
                }
                if(value is not null) {
                    BindUIEvents(value);
                }
                ui = value;
            }
        }

        public UIScene() {
            Name = "UI Scene";
            OnRender += UIScene_OnRender;
            Input.OnAcceptUp += Input_OnAcceptUp;
            Mouse.OnRelease += Mouse_OnRelease;
        }

        protected override void UpdateGame() {
            VectorRectangle viewport = new(Game.Viewport.Bounds);
            UI.Update(Now,viewport);
            UpdateInputs();
            UI.UpdateMouseLocation(Mouse.Position);
            UI.Update(Now,viewport); /* Interaction can be delayed by 1 frame if we don't update the UI again */
            Game.CursorState = UI.CursorState;
            UpdateCameraScreenSize();
            Entities.Update();
            UpdateCamera();
        }

        private void BindUIEvents(SpriteBook ui) {
            Input.OnAcceptDown += ui.AcceptDown;
            Input.OnCancelDown += ui.CancelDown;
            Mouse.OnPress += ui.MouseDown;
            Input.OnDirectionDown += ui.DirectionDown;
            Mouse.OnMove += ui.MouseMove;
        }

        private void UnbindUIEvents(SpriteBook ui) {
            Input.OnAcceptDown -= ui.AcceptDown;
            Input.OnCancelDown -= ui.CancelDown;
            Mouse.OnPress -= ui.MouseDown;
            Input.OnDirectionDown -= ui.DirectionDown;
            Mouse.OnMove -= ui.MouseMove;
            Input.OnAcceptUp -= Input_OnAcceptUp;
            Mouse.OnRelease -= Mouse_OnRelease;
        }

        private void Mouse_OnRelease() => UI?.MouseUp(Now);
        private void Input_OnAcceptUp() => UI?.AcceptUp(Now);
        private void UIScene_OnRender() => UI?.Render(Game.SpriteBatch);
    }
}
