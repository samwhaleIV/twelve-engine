using System;
using TwelveEngine.UI;
using TwelveEngine.UI.Elements;
using Microsoft.Xna.Framework;
using TwelveEngine.PuzzleGame;
using TwelveEngine.Serial;
using TwelveEngine.Game2D;


namespace TwelveEngine {
    public static partial class Program {

        public static GameState UITestState() {
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
                };

                UI.Root.AddChild(button);

            });
        }

        public static GameState GetStartState() {
            MapDatabase.LoadMaps();
            EntityFactory.InstallDefault();
            var gameState = PuzzleFactory.GetLevel("CounterTest2");
            return gameState;
        }

        public static void StartGame(GameManager game) {
            game.SetGameState(GetStartState());
        }
    }
}
