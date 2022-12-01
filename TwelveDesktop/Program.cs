﻿using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Shell;
using TwelveEngine.Shell.Config;

namespace TwelveDesktop {

    internal static class Program {
        [STAThread]
        internal static void Main() {
            ConfigLoader.LoadEngineConfig(new TwelveConfigSet());
            using var game = new GameManager();
            game.OnLoad += Game_OnLoad;
            game.Run(GameRunBehavior.Synchronous);
        }

        private static void Game_OnLoad(GameManager game) {
            Elves.Program.Main(game);
        }
    }
}
