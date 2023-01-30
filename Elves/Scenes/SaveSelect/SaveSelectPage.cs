using TwelveEngine.UI.Book;

namespace Elves.Scenes.SaveSelect {
    public abstract class SaveSelectPage:BookPage<SpriteElement> {
        public SaveSelectUI UI { get; set; }
        public SaveSelectScene Scene { get; set; }
    }
}
