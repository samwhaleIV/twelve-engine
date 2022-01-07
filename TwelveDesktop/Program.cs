using System;
using Microsoft.Xna.Framework;
using TwelveEngine;
using Porthole;

namespace TwelveDesktop {
    using static TwelveEngine.Program;
    using static Porthole.Program;
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
