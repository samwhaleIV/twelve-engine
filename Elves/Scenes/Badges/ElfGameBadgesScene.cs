namespace Elves.Scenes.Badges {
    public sealed class ElfGameBadgesScene:BadgesScene {
        private static Badge[] GetBadges() => new Badge[] {
            new Badge(Program.Textures.TwelveEngineLogo),
            new Badge(Program.Textures.MonoGameLogo),
            new Badge(Program.Textures.FMODLogo),
            new Badge(Program.Textures.ElvesLogo)
        };
        public ElfGameBadgesScene():base(GetBadges()) {}
    }
}
