using TwelveEngine.UI.Book;

namespace Elves.Scenes.Carousel {
    public sealed class CarouselUI:SpriteBook {

        private readonly CarouselScene3D scene;

        public CarouselUI(CarouselScene3D scene):base(scene) {
            this.scene = scene;
            Initialize();
            scene.Impulse.Router.OnAcceptDown += () => scene.StartBattle();
        }

        private void Initialize() {
            //todo make elements...
        }
    }
}
