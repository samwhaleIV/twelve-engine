using TwelveEngine;
using TwelveEngine.Shell;
using TwelveEngine.UI;

namespace Elves.Scenes.Carousel {
    public sealed class CarouselMenu:CarouselScene3D {

        private CarouselUI UI;

        public CarouselMenu() {
            UI = new(this);
            UI.BindInputEvents(this);
            OnPreRender += PreRenderCarouselScene;
            OnRender += CarouselMenu_OnRender;
        }

        protected override void UpdateGame() {
            FloatRectangle viewport = new(Viewport);
            UI.Update(Now,viewport);
            UpdateInputDevices();
            UI.SendEvent(InputEvent.CreateMouseUpdate(Mouse.Position));
            UI.Update(Now,viewport);
            CustomCursor.State = UI.CursorState;
            UpdateCarouselItems();
            UpdateEntities();
            UpdateCamera();
        }

        private void CarouselMenu_OnRender() {
            RenderCarouselScene();
            UI.Render(SpriteBatch);
        }
    }
}
