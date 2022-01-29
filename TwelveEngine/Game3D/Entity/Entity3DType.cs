using TwelveEngine.EntitySystem;
using TwelveEngine.Game3D.Entity.Types;

namespace TwelveEngine.Game3D.Entity {
    public static class Entity3DType {

        public const int Undefined = 0;
        public const int Model = 1;
        public const int Texture = 2;
        public const int RenderTarget = 3;
        public const int GridLines = 4;
        public const int AnimatedModel = 5;

        public static EntityFactory<Entity3D,World> GetFactory() => new EntityFactory<Entity3D,World>(

            (Undefined, () => null),
            (Model, () => new ModelEntity()),
            (Texture, () => new TextureEntity()),
            (RenderTarget,() => new RenderTargetEntity()),
            (GridLines,() => new GridLinesEntity()),
            (AnimatedModel,() => new AnimatedModelEntity())

        );
    }
}
