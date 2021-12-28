using TwelveEngine.EntitySystem;
using TwelveEngine.Game2D.Entity.Types;

namespace TwelveEngine.Game2D.Entity {
    public static class Entity2DType {

        public const int Undefined = 0;
        public const int RedBox = 1;
        public const int Player = 2;

        public static EntityFactory<Entity2D,Grid2D> GetFactory() => new EntityFactory<Entity2D,Grid2D>(

            (Undefined, () => null),
            (RedBox, () => new RedBox()),
            (Player, () => new Player())

        );
    }
}
