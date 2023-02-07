using Elves.Scenes.Battle;

namespace Elves.ElfData {
    /// <summary>
    /// Symmetric link between <see cref="BattleScript"/> and <see cref="ElfManifest"/>. Must be sorted in the order they appear in the carousel.
    /// </summary>
    public enum ElfID {
        None = -1,
        HarmlessElf,
        RedGirlElf,
        YellowElf,
        MagicElf,
        GreenGirlElf,
        NinjaElf,
        CaponeElf,
        BusinessElf,
        MechaElf,
        GodElf,
        SpaceElf
    }
}
