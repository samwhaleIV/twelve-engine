using TwelveEngine.Game2D.Entities;

namespace TwelveEngine.Game2D {
    public enum EntityType {
        Undefined = 0,
        RedBox = 1,
        Player = 2
    }
    public static partial class EntityFactory {
        internal static void InstallDefault() {
            SetType(EntityType.Undefined,() => null);
            SetType(EntityType.RedBox,() => new RedBox());
            SetType(EntityType.Player,() => new Player());
        }
    }
}

