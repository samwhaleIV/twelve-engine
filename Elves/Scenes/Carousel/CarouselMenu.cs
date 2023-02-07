using TwelveEngine.Shell;
using Elves.Scenes.Carousel.UI;

namespace Elves.Scenes.Carousel {
    public sealed class CarouselMenu:CarouselScene3D {

        private readonly CarouselUI UI;

        public CarouselMenu(bool animateLastBattleProgress) : base(animateLastBattleProgress) {
            Name = "Carousel Menu";
            UI = new(this);
            UI.BindInputEvents(this);
            OnPreRender.Add(PreRenderCarouselScene);
            OnRender.Add(Render);
            OnUpdate.Add(Update);
            OnLoad.Add(LoadUI);
            OnInputActivated += UI.FocusDefault;
        }

        private void LoadUI() {
            UI.SetFirstPage(UI.DefaultPage,new(Viewport));
        }

        private void Update() {
            UI.Update(new(Viewport));
            CustomCursor.State = UI.CursorState;
            UpdateCarouselItems();
        }

        private void Render() {
            UI.Render(SpriteBatch);
        }
    }
}
