using TwelveEngine;
using TwelveEngine.Input;
using TwelveEngine.UI;
using TwelveEngine.UI.Book;

namespace Elves.Scenes.SaveSelect {
    public abstract class UIScene:Scene {

        public SpriteBook UI { get; set; }

        public UIScene() {
            Name = "UI Scene";

            OnRender += UIScene_OnRender;
            OnUpdate += UIScene_OnUpdate;

            Mouse.OnPress += Mouse_OnPress;
            Mouse.OnRelease += Mouse_OnRelease;

            Input.OnAcceptDown += Input_OnAcceptDown;
            Input.OnAcceptUp += Input_OnAcceptUp;

            Input.OnCancelDown += Input_OnCancelDown;
            Input.OnDirectionDown += Input_OnDirectionDown;
            Input.OnFocusDown += Input_OnFocusDown;
        }

        private void Input_OnFocusDown() {
            UI?.SendEvent(InputEvent.FocusButtonActivated);
        }

        private void Mouse_OnRelease() => UI?.SendEvent(InputEvent.MouseReleased);
        private void Input_OnAcceptUp() => UI?.SendEvent(InputEvent.AcceptReleased);
        private void Input_OnDirectionDown(Direction direction) => UI?.SendEvent(InputEvent.CreateDirectionImpulse(direction));
        private void Mouse_OnPress() => UI?.SendEvent(InputEvent.MousePressed);
        private void Input_OnCancelDown() => UI?.SendEvent(InputEvent.BackButtonActivated);
        private void Input_OnAcceptDown() => UI?.SendEvent(InputEvent.AcceptPressed);

        private void UIScene_OnUpdate() {
            if(UI is null) {
                return;
            }
            VectorRectangle viewport = new(Game.Viewport.Bounds);
            UI.Update(Now,viewport);
            UpdateInputDevices();
            UI.SendEvent(InputEvent.CreateMouseUpdate(Mouse.Position));
            UI.Update(Now,viewport); /* Interaction can be delayed by 1 frame if we don't update the UI again */
            Game.CursorState = UI.CursorState;
        }

        private void UIScene_OnRender() => UI?.Render(Game.SpriteBatch);
    }
}
