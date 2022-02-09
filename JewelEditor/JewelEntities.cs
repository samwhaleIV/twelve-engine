using TwelveEngine.EntitySystem;
using TwelveEngine.Game2D;

namespace JewelEditor {
    internal static class JewelEntities {

        public const int GridLines = 1;
        public const int MouseReceiver = 2;
        public const int EntityMarker = 3;

        public static EntityFactory<Entity2D,Grid2D> GetFactory() => new EntityFactory<Entity2D,Grid2D>(
            (GridLines, () => new GridLines()),
            (MouseReceiver, () => new MouseReceiver()),
            (EntityMarker, () => new EntityMarker())
        );
    }
}
