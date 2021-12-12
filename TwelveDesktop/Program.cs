using System;
using Microsoft.Xna.Framework;
using TwelveEngine;

namespace TwelveDesktop {
    internal static class Program {
        [STAThread]
        internal static void Main() {
            using var game = new GameManager();
            game.SetGameState(TwelveEngine.Program.GetStartState());
            game.Run(GameRunBehavior.Synchronous);
        }
    }
}
