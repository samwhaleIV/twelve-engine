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
                    X = 70,
                    Y = 10,
                    Width = 100,
                    Height = 400,
                };

                var picture = new Picture("cat-test-picture") {
                    Padding = 0,
                    Sizing = Sizing.Fill,
                    Positioning = Positioning.Relative,
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
                    X = 10,
                    Y = 10,
                    Width = 50,
                    Height = 50,
                    Positioning = Positioning.Absolute,
                    IsInteractable = true
                };

                button.OnClick += () => {
                    var viewport = UI.Game.GraphicsDevice.Viewport;
                    button.X = random.Next(0,viewport.Width - button.Width);
                    button.Y = random.Next(0,viewport.Height - button.Height);

                    UI.Game.SetState(GetPuzzleGameTest());
                };

                UI.Root.AddChild(button);
                UI.Root.AddChild(pictureBackground);
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
