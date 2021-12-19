using Microsoft.Xna.Framework;
using TwelveEngine.UI;
using TwelveEngine.UI.Elements;
using TwelveEngine.PuzzleGame;
using TwelveEngine.Serial.Map;
using TwelveEngine.Game2D;

namespace TwelveEngine {
    public static partial class Program {

        private static bool loadedPuzzleGameData = false;

        private static void tryLoadPuzzleGameData() {
            if(loadedPuzzleGameData) {
                return;
            }
            MapDatabase.LoadMaps();
            EntityFactory.InstallDefault();
            loadedPuzzleGameData = true;
        }

        public static GameState GetPuzzleGameTest() {
            tryLoadPuzzleGameData();
            return PuzzleFactory.GetLevel("CounterTest2");
        }

        public static GameState GetUITestState() {
            return UIGameState.Create(UI => {

                var masterScrollBox = new ScrollBox(UI) {
                    Width = 800,
                    Height = 800,
                    Positioning = Positioning.CenterParent
                };

                var panel1 = new Panel(Color.Orange) {
                    X = 0,
                    Y = 0,
                    Width = 800,
                    Height = 800
                };
                var panel2 = new Panel(Color.Red) {
                    X = 0,
                    Y = 800,
                    Width = 800,
                    Height = 800
                };

                masterScrollBox.AddChild(panel1,panel2);

                UI.AddChild(masterScrollBox);
            });
        }

        public static GameState GetStartState() {
            return GetUITestState();
        }

        public static void StartGame(GameManager game) {
            game.SetState(GetStartState());
        }
    }
}
