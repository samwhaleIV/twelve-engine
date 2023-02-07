using TwelveEngine.Shell;
using Elves.Scenes.Carousel.UI;

namespace Elves.Scenes.Carousel {
    public sealed class CarouselMenu:CarouselScene3D {

        private readonly CarouselUI UI;

        public CarouselMenu() {
            Name = "Carousel Menu";
            UI = new(this);
            UI.BindInputEvents(this);
            OnPreRender.Add(PreRenderCarouselScene);
            OnRender.Add(Render);
            OnUpdate.Add(Update);
        }

        private void Update() {
            UI.Update(Now,new(Viewport));
            CustomCursor.State = UI.CursorState;
            UpdateCarouselItems();
        }

        private void Render() {
            UI.Render(SpriteBatch);
        }
    }
}
