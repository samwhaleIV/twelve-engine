using Porthole.PuzzleGame;
using TwelveEngine;
using TwelveEngine.Serial.Map;

namespace Porthole {
    internal static class Program {
        private static bool loadedPuzzleGameData = false;

        private static void tryLoadPuzzleGameData() {
            if(loadedPuzzleGameData) {
                return;
            }
            MapDatabase.LoadMaps();
            loadedPuzzleGameData = true;
        }

        public static GameState GetPuzzleGameTest() {
            tryLoadPuzzleGameData();
            return PuzzleFactory.GetLevel("CounterTest2");
        }
    }
}
