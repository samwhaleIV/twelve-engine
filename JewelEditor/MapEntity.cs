using TwelveEngine.Game2D;

namespace JewelEditor {
    public sealed class MapEntity:Entity2D {
        protected override int GetEntityType() => (int)EntityTypes.MapEntity;
    }
}
