using TwelveEngine.EntitySystem;
using TwelveEngine.Game3D.Entity.Types;

namespace TwelveEngine.Game3D.Entity {
    public static class Entity3DType {

        public const int Undefined = 0;
        public const int Model = 1;

        public static EntityFactory<Entity3D,World> GetFactory() => new EntityFactory<Entity3D,World>(

            (Undefined, () => null),
            (Model, () => new ModelEntity())

        );
    }
}
