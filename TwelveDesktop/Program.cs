using System;
using Microsoft.Xna.Framework;
using TwelveEngine;
using TwelveEngine.Config;
using TwelveEngine.Game3D;

namespace TwelveDesktop {
        
    internal static class Program {
        [STAThread]
        internal static void Main() {

            ConfigLoader.LoadEngineConfig(new TwelveConfigSet() {
                CPUTextures = new string[] { "patterns" }
            });
            using var game = new GameManager();
            game.OnLoad += Game_OnLoad;
            game.Run(GameRunBehavior.Synchronous);
        }

        private static void Game_OnLoad(GameManager game) {
            game.SetState(new CRTTest());
        }
    }
}
