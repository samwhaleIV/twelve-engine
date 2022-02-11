using JewelEditor.Entity;
using TwelveEngine.Game2D;

namespace JewelEditor {
    internal abstract class JewelEntity:Entity2D {
        protected internal StateEntity State => Owner.Entities.Get<StateEntity>(Editor.State);
        protected internal UIEntity UI => Owner.Entities.Get<UIEntity>(Editor.UI);
    }
}
