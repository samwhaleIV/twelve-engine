namespace TwelveEngine {
    public static class Program {

        private static GameState GetTestStartState() {
            var grid2D = new Grid2D(new TestTileRenderer());
            grid2D.Camera.Scale = 4;
            grid2D.Camera.EdgePadding = false;
            grid2D.Camera.X = 0;
            grid2D.Camera.Y = 0;
            return grid2D;
        }

        public static GameManager GetStartGame() {
            var startState = GetTestStartState();
            return new GameManager(startState);
        }
        public static void ConfigStartGame(GameManager game) {
            game.GameState = GetTestStartState();
        }
    }
}
