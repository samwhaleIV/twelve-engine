using System;
using Microsoft.Xna.Framework;
using TwelveEngine;

namespace TwelveDesktop {
    using static TwelveEngine.Program;
    internal static class Program {
        [STAThread]
        internal static void Main() {
            LoadEngineConfig();
            using var game = new GameManager();
            game.SetState(GetPuzzleGameTest);
            game.Run(GameRunBehavior.Synchronous);
        }
    }
}
