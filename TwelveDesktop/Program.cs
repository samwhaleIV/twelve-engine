using System;
using TwelveEngine;

namespace TwelveDesktop {
    public static class Program {
        [STAThread]
        static void Main() {
            var grid2D = new Grid2D(new TestTileRenderer());
            grid2D.Camera.Scale = 4;
            grid2D.Camera.EdgePadding = false;
            grid2D.Camera.HorizontalEdgePadding = true;
            grid2D.Camera.VerticalEdgePadding = true;
            grid2D.Camera.X = 0;
            grid2D.Camera.Y = 1000;
            using var game = new GameManager(grid2D);
            game.Run();
        }
    }
}
