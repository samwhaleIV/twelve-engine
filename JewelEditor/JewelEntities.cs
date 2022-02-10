using TwelveEngine.EntitySystem;
using TwelveEngine.Game2D;
using JewelEditor.InputContext;

namespace JewelEditor {
    internal static class JewelEntities {

        public const int GridLines = 1;
        public const int InputEntity = 2;
        public const int EntityMarker = 3;
        public const int UIEntity = 4;

        public static EntityFactory<Entity2D,Grid2D> GetFactory() => new EntityFactory<Entity2D,Grid2D>(
            (GridLines, () => new GridLines()),
            (InputEntity, () => new InputEntity()),
            (EntityMarker, () => new EntityMarker()),
            (UIEntity, () => new UIEntity())
        );
    }
}
