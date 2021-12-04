using TwelveEngine.Serial;
using TwelveEngine.Game2D;

namespace TwelveEngine {
    public static partial class Program {
        public static GameState GetStartState() {
            MapDatabase.LoadMaps();
            EntityFactory.InstallDefault();
            var gameState = Main();
            return gameState;
        }
        public static GameManager GetStartGame() {
            var startState = GetStartState();
            return new GameManager(startState);
        }
        public static void ConfigStartGame(GameManager game) {
            game.SetGameState(GetStartState());
        }
    }
}
