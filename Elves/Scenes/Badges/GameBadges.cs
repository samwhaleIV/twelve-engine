using System;
using System.Collections.Generic;
using System.Text;

namespace Elves.Scenes.Badges {
    public sealed class GameBadges:BadgesScene {
        public GameBadges():base(new Badge[] {
            new Badge(Program.Textures.TwelveEngineLogo),
            new Badge(Program.Textures.MonoGameLogo),
            new Badge(Program.Textures.FMODLogo),
            new Badge(Program.Textures.ElvesLogo)
        }) {

        }
    }
}
