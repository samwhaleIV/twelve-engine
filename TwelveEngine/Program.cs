using System;
using TwelveEngine.Serial;
using TwelveEngine.Game2D;
using TwelveEngine.UI;
using TwelveEngine.UI.Elements;
using Microsoft.Xna.Framework;

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
            return UITestState();

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
