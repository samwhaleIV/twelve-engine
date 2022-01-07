using System;
using Microsoft.Xna.Framework;
using TwelveEngine;

namespace TwelveDesktop {
    using static TwelveEngine.Program;
    using static Porthole.Program;
    internal static class Program {
        [STAThread]
        internal static void Main() {
            LoadEngineConfig();
            using var game = new GameManager();
            game.OnLoad += async game => {
                await game.SetState(GetPuzzleGameTest);
            };
            game.Run(GameRunBehavior.Synchronous);
        }
    }
}
