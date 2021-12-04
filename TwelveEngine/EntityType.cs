using TwelveEngine.Game2D.Entities;

namespace TwelveEngine.Game2D {

    public enum EntityType {
        Undefined = 0,
        RedBox = 1,
        Player = 2
    }

    public static partial class EntityFactory {
        internal static void InstallDefault() => Install(
            (EntityType.Undefined, () => null),
            (EntityType.RedBox,() => new RedBox()),
            (EntityType.Player, () => new Player())
        );
    }
}
