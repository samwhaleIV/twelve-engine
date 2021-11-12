using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Entities;

namespace TwelveEngine {
    public static class Program {

        private static GameState GetTestStartState() {
            var grid = new Grid2D<int>(new TestTileRenderer());
            grid.Camera.Scale = 4;
            grid.Camera.EdgePadding = true;
            grid.Camera.X = 0;
            grid.Camera.Y = 0;

            grid.OnLoad = () => {
                var redBox = new TheRedBox();
                grid.AddEntity(redBox);
            };

            return grid;
        }

        public static GameManager GetStartGame() {
            DefaultEntitiesList.Install();
            var startState = GetTestStartState();
            return new GameManager(startState);
        }
        public static void ConfigStartGame(GameManager game) {
            game.GameState = GetTestStartState();
        }
    }
}
