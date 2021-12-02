using TwelveEngine.PuzzleGame;

namespace TwelveEngine {
    public static class Program {

        public static GameState GetStartState() {
            MapDatabase.LoadMaps();
            var gameState = PuzzleFactory.GenerateState(factory => {
                return factory.Test();
            });
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
