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

                Panel button = null;
                var random = new Random();

                var pictureBackground = new Panel(Color.White) {
                    Area = new Rectangle(70,10,400,300)
                };

                var picture = new Picture("cat-test-picture") {
                    Padding = 0,
                    Sizing = Sizing.Fill,
                    Mode = PictureMode.Cover,
                    IsInteractable = true,
                };

                picture.OnClick += () => {
                    pictureBackground.PauseLayout();

                    int buffer = pictureBackground.Width;
                    pictureBackground.Width = pictureBackground.Height;
                    pictureBackground.Height = buffer;

                    pictureBackground.StartLayout();
                };

                pictureBackground.AddChild(picture);

                button = new Panel(Color.Orange) {
                    Area = new Rectangle(10,10,50,50),
                    IsInteractable = true
                };

                button.OnClick += () => {
                    button.X = random.Next(0,UI.Width - button.Width);
                    button.Y = random.Next(0,UI.Height - button.Height);

                    //UI.Game.SetState(GetPuzzleGameTest());
                };

                UI.AddChild(button);
                UI.AddChild(pictureBackground);
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
