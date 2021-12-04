using System;
using Microsoft.Xna.Framework;

namespace TwelveDesktop {
    public static class Program {
        [STAThread]
        static void Main() {
            using var game = TwelveEngine.Program.GetStartGame();
            game.Run(GameRunBehavior.Synchronous);
        }
    }
}
