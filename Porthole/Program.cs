using Porthole.PuzzleGame;
using TwelveEngine;
using TwelveEngine.Serial.Map;
using TwelveEngine.Shell;

namespace Porthole {
    internal static class Program {
        private static bool loadedPuzzleGameData = false;

        private static void tryLoadPuzzleGameData() {
            if(loadedPuzzleGameData) {
                return;
            }
            MapDatabase.LoadMaps(@"C:\Users\pinks\Documents\twelve-engine\Porthole\maps.temdb"); //don't publish this, dumbass
            loadedPuzzleGameData = true;
        }

        public static GameState GetPuzzleGameTest() {
            tryLoadPuzzleGameData();
            return PuzzleFactory.GetLevel("CounterTest2");
        }
    }
}
