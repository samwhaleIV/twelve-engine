using TwelveEngine.UI;

namespace Elves.Scenes.SaveSelect {
    public abstract class SaveSelectPage:Page<SpriteElement> {
        public SaveSelectUI UI { get; set; }
        public SaveSelectScene Scene { get; set; }
    }
}
