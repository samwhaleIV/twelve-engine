using System;
namespace TwelveDesktop {
    public static class Program {
        [STAThread]
        static void Main() {
            using var game = TwelveEngine.Program.GetStartGame();
            game.Run();
        }
    }
}
