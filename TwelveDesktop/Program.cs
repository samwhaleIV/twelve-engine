using System;
using TwelveEngine;

namespace TwelveDesktop {
    public static class Program {
        [STAThread]
        static void Main() {
            using var game = new GameManager();
            game.Run();
        }
    }
}
