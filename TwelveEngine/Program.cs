using System;
using TwelveEngine.UI;
using TwelveEngine.UI.Elements;
using Microsoft.Xna.Framework;
using TwelveEngine.PuzzleGame;
using TwelveEngine.Serial;
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

                Button button = null;
                var random = new Random();

                button = new Button(Color.Orange) {
                    X = 10,
                    Y = 10,
                    Width = 200,
                    Height = 200,
                    Positioning = Positioning.Absolute
                };
                button.OnClick += () => {
                    var viewport = UI.Game.GraphicsDevice.Viewport;
                    button.X = random.Next(0,viewport.Width - button.Width);
                    button.Y = random.Next(0,viewport.Height - button.Height);

                    UI.Game.SetState(GetPuzzleGameTest());
                };

                UI.Root.AddChild(button);

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
