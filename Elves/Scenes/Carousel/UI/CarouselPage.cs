using TwelveEngine.UI.Book;

namespace Elves.Scenes.Carousel.UI {
    public abstract class CarouselPage:BookPage<SpriteElement> {
        public CarouselUI UI { get; set; }
        public CarouselScene3D Scene { get; set; }
    }
}
