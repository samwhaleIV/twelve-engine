using TwelveEngine.Shell;

namespace Elves.Scenes.Carousel {
    public sealed class CarouselMenu:CarouselScene3D {

        private readonly CarouselUI UI;

        public CarouselMenu() {
            Name = "Carousel Menu";
            UI = new(this);
            UI.BindInputEvents(this);
            OnPreRender += PreRenderCarouselScene;
            OnRender += CarouselMenu_OnRender;
            OnUpdate += CarouselMenu_OnUpdate;
        }

        private void CarouselMenu_OnUpdate() {
            UI.Update(Now,new(Viewport));
            CustomCursor.State = UI.CursorState;
            UpdateCarouselItems();
        }

        private void CarouselMenu_OnRender() {
            RenderCarouselScene();
            UI.Render(SpriteBatch);
        }
    }
}
