using System;
using Microsoft.Xna.Framework;
using TwelveEngine;
using TwelveEngine.Config;

namespace TwelveDesktop {

    internal static class Program {
        [STAThread]
        internal static void Main() {
            ConfigLoader.LoadEngineConfig();
            using var game = new GameManager();
            game.OnLoad += game => game.SetState(new ElfGame.FacialAnimationViewer());
            game.Run(GameRunBehavior.Synchronous);
        }
    }
}
