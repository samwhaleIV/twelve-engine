using System;
using Microsoft.Xna.Framework;
using TwelveEngine;
using TwelveEngine.Config;
using TwelveEngine.TileGen;

namespace TwelveDesktop {

    internal static class Program {
        [STAThread]
        internal static void Main() {
            ConfigLoader.LoadEngineConfig(new TwelveConfigSet() {
                CPUTextures = new string[] { "patterns" }
            });
            using var game = new GameManager();
            game.OnLoad += game => game.SetState<TileGenViewer>();
            game.Run(GameRunBehavior.Synchronous);
        }
    }
}
