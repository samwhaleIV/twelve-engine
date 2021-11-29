using TwelveEngine.PuzzleGame;
using TwelveEngine.PuzzleGame.Levels;

namespace TwelveEngine {
    public static class Program {

        public static GameState GetStartState() {
            MapDatabase.LoadMaps();
            var gameState = LevelFactory.GenerateLevel(AlphaLevels.Level2);
            return gameState;
        }

        public static GameManager GetStartGame() {
            DefaultEntitiesList.Install();
            var startState = GetStartState();
            return new GameManager(startState);
        }
        public static void ConfigStartGame(GameManager game) {
            game.SetGameState(GetStartState());
        }
    }
}
