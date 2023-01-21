using Elves.UI.SpriteUI;
using Elves.UI;

namespace Elves.Scenes.SaveSelect {
    public abstract class SaveSelectUIPage:Page<SpriteElement> {
        private readonly SaveSelectUI ui;
        public SaveSelectUI UI => ui;
        public SaveSelectUIPage(SaveSelectUI ui) => this.ui = ui;
    }
}
