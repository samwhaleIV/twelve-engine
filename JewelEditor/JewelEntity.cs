using JewelEditor.Entity;
using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Entity;

namespace JewelEditor {
    internal abstract class JewelEntity:Entity2D {
        protected internal TileGrid TileGrid => Owner as TileGrid;
        protected internal StateEntity GetState() => Owner.Entities.Get<StateEntity>(Editor.State);
        protected internal QuickActionBar GetQuickActionBar() => Owner.Entities.Get<QuickActionBar>(Editor.QuickActionBar);
    }
}
