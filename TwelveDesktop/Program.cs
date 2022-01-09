using System;
using Microsoft.Xna.Framework;
using TwelveEngine;
using TwelveEngine.Config;
using TwelveEngine.TileGen;

namespace TwelveDesktop {

    using static Porthole.Program;

    internal static class Program {
        [STAThread]
        internal static void Main() {
            ConfigLoader.LoadEngineConfig();
            using var game = new GameManager();
            game.OnLoad += game => {
                game.SetState(GetPuzzleGameTest);
                game.ImpulseHandler.OnCancelDown += () => {
                    game.SetState(TwelveEngine.UI.Factory.Debug.GetNestedFrameTest);
                };
            };
            game.Run(GameRunBehavior.Synchronous);
        }
    }
}
